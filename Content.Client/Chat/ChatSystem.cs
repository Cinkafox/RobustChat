using Content.Client.Chat.Controllers;
using Content.Shared.Chat;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Client.Chat;

public sealed class ChatSystem : EntitySystem
{
    [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
    [Dependency] private readonly INetManager _netManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    
    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<ChatSendEvent>(OnChatSend);
        SubscribeLocalEvent<UserComponent, PlayerAttachedEvent>(OnAttached);
        _netManager.RegisterNetMessage<ChatClientServerMessage>();
    }

    private void OnAttached(Entity<UserComponent> ent, ref PlayerAttachedEvent args)
    {
        if(!args.Player.Equals(_playerManager.LocalSession)) return;
        
        _userInterfaceManager.GetUIController<ChatController>().SetLocalUsername(ent.Comp.UserName);
    }

    public void Send(string message)
    {
        _netManager.ClientSendMessage(new ChatClientServerMessage()
        {
            Message = message
        });
    }

    private void OnChatSend(ChatSendEvent ev)
    {
        if (ev.Sender is not null && TryComp<UserComponent>(GetEntity(ev.Sender.Value), out var userComponent))
        {
            _userInterfaceManager.GetUIController<ChatController>().AddMessage(userComponent, ev.Message);
        }
    }
}