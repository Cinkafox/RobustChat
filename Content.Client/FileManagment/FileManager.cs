using Content.Shared.ContentDependencies;
using Content.Shared.FileManagment;
using Robust.Shared.Network;

namespace Content.Client.FileManagment;

[RegisterDependency]
public sealed class FileManager : IInitializable
{
    [Dependency] private readonly INetManager _netManager = default!;
    
    private Dictionary<FileId, byte[]> _files = [];
    private Dictionary<FileId, Queue<Action<byte[]>>> _callbacks = new();
    private readonly HashSet<FileId> _pendingFiles = [];
    
    public void Initialize(IDependencyCollection collection)
    {
        collection.InjectDependencies(this);
        _netManager.RegisterNetMessage<FileClientServerFileRequiredMessage>();
        _netManager.RegisterNetMessage<FileServerFileResponseMessage>(OnResponse);
    }

    private void OnResponse(FileServerFileResponseMessage message)
    {
        if (message.Data.Length != 0)
            _files[message.File] = message.Data;
        
        _pendingFiles.Remove(message.File);

        if (_callbacks.TryGetValue(message.File, out var callback))
        {
            while (callback.TryDequeue(out var action))
            {
                action(message.Data);
            }
        }
    }

    public void GetFile(FileId file, Action<byte[]> callback)
    {
        if (_files.TryGetValue(file, out var data))
        {
            callback(data);
            return;
        }

        if (!_callbacks.TryGetValue(file, out var callbacks))
        {
            callbacks = new Queue<Action<byte[]>>();
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