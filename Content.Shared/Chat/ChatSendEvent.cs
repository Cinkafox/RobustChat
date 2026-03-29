using Content.Shared.FileManagment;
using Robust.Shared.Serialization;

namespace Content.Shared.Chat;

[DataDefinition, Serializable, NetSerializable]
public sealed partial class ChatSendEvent : EntityEventArgs
{
    [DataField] public ChatEntry[] Entries = [];
    [DataField] public NetEntity ChannelId;
}

[DataDefinition, Serializable, NetSerializable]
public sealed partial class ChatEntry
{
    [DataField] public string? Message;
    [DataField] public FileId? File;
    [DataField] public NetEntity? Sender;
    [DataField] public DateTime SendTime;
}