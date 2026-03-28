using Robust.Shared.GameStates;

namespace Content.Shared.Chat;

[RegisterComponent, AutoGenerateComponentState, NetworkedComponent]
public sealed partial class ChatChannelComponent: Component
{
    [DataField, AutoNetworkedField] public string ChannelName;
    [DataField] public List<ChatEntry> ChatEntries = [];
}