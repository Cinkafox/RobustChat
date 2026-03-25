using Robust.Shared.Serialization;

namespace Content.Shared.Chat;

[DataDefinition, Serializable, NetSerializable]
public sealed partial class ChatSendEvent : EntityEventArgs
{
    [DataField] public string Message;
    [DataField] public NetEntity? Sender;
    [ViewVariables(VVAccess.ReadOnly)] public string MessageHash => Message.GetHashCode().ToString();
}