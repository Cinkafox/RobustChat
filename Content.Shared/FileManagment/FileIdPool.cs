using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.FileManagment;

public sealed class FileIdPool
{
    private readonly Queue<FileId> _freeIds = new();
    private int _nextId = 1;

    public FileId Take()
    {
        if (_freeIds.TryDequeue(out var id))
            return id;
        return new FileId(_nextId++);
    }
    
    public void Return(FileId id)
    {
        if(_freeIds.Contains(id)) 
            return;
        
        _freeIds.Enqueue(id);
    }
}

public sealed class FileClientServerFileRequiredMessage : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;
    public FileId File;
    
    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        File = new FileId(buffer.ReadInt32());
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(File.Id);
    }
}

public sealed class FileServerFileResponseMessage : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;
    public FileId File;
    public byte[] Data = [];
    
    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        File = new FileId(buffer.ReadInt32());
        Data = buffer.ReadBytes(buffer.ReadInt32());
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(File.Id);
        buffer.Write(Data.Length);
        buffer.Write(Data);
    }
}