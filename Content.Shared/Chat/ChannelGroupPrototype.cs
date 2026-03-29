using Robust.Shared.Prototypes;

namespace Content.Shared.Chat;

[Prototype]
public sealed partial class ChannelGroupPrototype : IPrototype
{
    [IdDataField] public string ID { get; set; } = default!;
    [DataField] public List<string> Channels = [];
}