using Robust.Shared.GameStates;

namespace Content.Shared.Chat;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class UserComponent : Component
{
    [DataField, AutoNetworkedField] public string UserName;
}