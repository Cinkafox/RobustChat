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