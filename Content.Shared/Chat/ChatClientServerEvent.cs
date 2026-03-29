using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Chat;

public sealed class ChatClientServerMessage : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;
    public string Message = string.Empty;
    
    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        Message = buffer.ReadString();
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(Message);
    }
}

public sealed class ChatClientServerSelectChannelMessage : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;
    public NetEntity Channel;
    
    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        Channel = buffer.ReadNetEntity();
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(Channel);
    }
}

public sealed class ChatClientServerSendFileMessage : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;
    public byte[] File = [];
    public string FileName = "";
    
    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        File = new byte[buffer.ReadInt32()];
        FileName = buffer.ReadString();
        buffer.ReadBytes(File);
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(File.Length);
        buffer.Write(FileName);
        buffer.Write(File);
    }
}