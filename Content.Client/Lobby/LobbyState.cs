using Content.Client.Chat.Controllers;
using Content.Client.StateHelper;
using Robust.Client.UserInterface;

namespace Content.Client.Lobby;

public sealed class LobbyState : State<LobbyUI>
{
    [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;


    private IChatHandler _currentHandler = default!;
    private ChatController _chatController = default!;

    protected override void Startup()
    {
        IoCManager.InjectDependencies(this);
        _currentHandler = (IChatHandler)_userInterfaceManager.ActiveScreen!;
        _chatController = _userInterfaceManager.GetUIController<ChatController>();
        _chatController.RegisterChatHandler(_currentHandler);
    }

    protected override void Shutdown()
    {
        _chatController.UnregisterChatHandler(_currentHandler);
    }
}