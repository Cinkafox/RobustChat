using Robust.Shared.Serialization;

namespace Content.Shared.Chat;

[DataDefinition, Serializable, NetSerializable]
public sealed partial class ChannelListEvent : EntityEventArgs
{
    [DataField] public NetEntity[] AvailableChannels;
}