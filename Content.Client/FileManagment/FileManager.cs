using System.IO;
using System.IO.Compression;
using Content.Shared.ContentDependencies;
using Content.Shared.FileManagment;
using Robust.Shared.Network;

namespace Content.Client.FileManagment;

[RegisterDependency]
public sealed class FileManager : IInitializable
{
    [Dependency] private readonly INetManager _netManager = default!;
    
    private Dictionary<FileId, ContentFile> _files = [];
    private Dictionary<FileId, Queue<Action<ContentFile>>> _callbacks = new();
    private readonly HashSet<FileId> _pendingFiles = [];
    
    public void Initialize(IDependencyCollection collection)
    {
        collection.InjectDependencies(this);
        _netManager.RegisterNetMessage<FileClientServerFileRequiredMessage>();
        _netManager.RegisterNetMessage<FileServerFileResponseMessage>(OnResponse);
    }

    private void OnResponse(FileServerFileResponseMessage message)
    {
        _pendingFiles.Remove(message.FileId);
        if (!_callbacks.TryGetValue(message.FileId, out var callback)) return;
        
        if (message.Data.Length == 0)
        {
            while (callback.TryDequeue(out var action))
            {
                action(message.Data);
            }
            return;
        }
        
        using var inputStream = new MemoryStream(message.Data.Data);
        using var outputStream = new MemoryStream();
        
        using (var decompressor = new DeflateStream(inputStream, CompressionMode.Decompress, leaveOpen: true))
        {
            decompressor.CopyTo(outputStream);
        }
        
        var decompressed = new ContentFile(message.Data.Name, message.Data.Extension, outputStream.ToArray());
            
        _files[message.FileId] = decompressed;
        
        while (callback.TryDequeue(out var action))
        {
            action(decompressed);
        }
    }

    public void GetFile(FileId file, Action<ContentFile> callback)
    {
        if (_files.TryGetValue(file, out var data))
        {
            callback(data);
            return;
        }

        if (!_callbacks.TryGetValue(file, out var callbacks))
        {
            callbacks = new Queue<Action<ContentFile>>();
            _callbacks[file] = callbacks;
        }
        
        callbacks.Enqueue(callback);

        if (_pendingFiles.Add(file))
        {
            _netManager.ClientSendMessage(new FileClientServerFileRequiredMessage()
            {
                File = file
            });
        }
    }
}