using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using Content.Shared.ContentDependencies;
using Content.Shared.FileManagment;
using Robust.Shared.Network;

namespace Content.Server.FileManagment;

[RegisterDependency]
public sealed class FileManager : IInitializable
{
    [Dependency] private readonly INetManager _netManager = default!;
    
    private readonly Dictionary<FileId, ContentFile> _files = [];
    private readonly FileIdPool _fileIdPool = new();
    private readonly Dictionary<byte[], FileId> _contentHashToId = new();
    private readonly Dictionary<FileId, byte[]> _contentIdToHash = new();
    
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
            FileId = message.File,
            Data = file
        });
    }

    public FileId AddFile(string fileName, byte[] data)
    {
        var hash = SHA256.HashData(data);
        
        if (_contentHashToId.TryGetValue(hash, out var existingId))
            return existingId;
            
        using var outputStream = new MemoryStream();
        using (var compressor = new DeflateStream(outputStream, CompressionLevel.Optimal, leaveOpen: true))
        {
            compressor.Write(data, 0, data.Length);
        } 
            
        var compressedData = outputStream.ToArray();
        var file = _fileIdPool.Take();
        
        _files[file] = new ContentFile(fileName, compressedData);
        _contentHashToId[hash] = file;
        _contentIdToHash[file] = hash;
        
        return file;
    }

    public void RemoveFile(FileId file)
    {
        if(!_contentIdToHash.TryGetValue(file, out var hash))
            return;
        
        _contentHashToId.Remove(hash);
        _contentIdToHash.Remove(file);
        _files.Remove(file);
        _fileIdPool.Return(file);
    }
}