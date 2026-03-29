using System.Collections;
using System.Security.Cryptography;
using Content.Shared.ContentDependencies;
using Content.Shared.FileManagment;
using Robust.Shared.Network;

namespace Content.Server.FileManagment;

[RegisterDependency]
public sealed class FileManager : IInitializable
{
    [Dependency] private readonly INetManager _netManager = default!;
    
    private Dictionary<FileId, byte[]> _files = [];
    private FileIdPool _fileIdPool = new();
    private readonly Dictionary<byte[], FileId> _contentHashToId = new();
    
    public void Initialize(IDependencyCollection collection)
    {
        collection.InjectDependencies(this);
        _netManager.RegisterNetMessage<FileClientServerFileRequiredMessage>(OnClientFileRequired);
        _netManager.RegisterNetMessage<FileServerFileResponseMessage>();
    }

    private void OnClientFileRequired(FileClientServerFileRequiredMessage message)
    {
        _files.TryGetValue(message.File, out var file);
        
        message.MsgChannel.SendMessage(new FileServerFileResponseMessage()
        {
            File = message.File,
            Data = file ?? []
        });
    }

    public FileId AddFile(byte[] data)
    {
        byte[] hash = SHA256.HashData(data);
        
        if (_contentHashToId.TryGetValue(hash, out var existingId))
        {
            return existingId;
        }
        
        var file = _fileIdPool.Take();
        _files[file] = data;
        _contentHashToId[hash] = file;
        return file;
    }

    public void RemoveFile(FileId file)
    {
        if(!_files.TryGetValue(file, out var data))
            return;
        
        _contentHashToId.Remove(SHA256.HashData(data));
        _files.Remove(file);
        _fileIdPool.Return(file);
    }
}