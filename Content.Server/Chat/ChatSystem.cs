
using Robust.Server.Console.Commands;
using Robust.Server.Player;

namespace Content.Server.Chat;

public sealed class ChatSystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    public override void Initialize()
    {
        base.Initialize();
    }

    
}