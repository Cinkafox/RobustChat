using Robust.Shared.Serialization;

namespace Content.Shared.Chat;

[DataDefinition, Serializable, NetSerializable]
public sealed partial class ChatSendEvent : EntityEventArgs
{
    [DataField] public ChatEntry[] Entries = [];
    [DataField] public bool ClearRequired = false;
}

[DataDefinition, Serializable, NetSerializable]
public sealed partial class ChatEntry
{
    [DataField] public string Message;
    [DataField] public NetEntity? Sender;
    [DataField] public DateTime SendTime;
    [ViewVariables(VVAccess.ReadOnly)] public string MessageHash => Message.GetHashCode().ToString();
}