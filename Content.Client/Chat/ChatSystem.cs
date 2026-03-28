using Content.Client.Chat.Controllers;
using Content.Shared.Chat;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Client.Chat;

public sealed class ChatSystem : EntitySystem
{
    [Dependency] private readonly INetManager _netManager = default!;
    
    public override void Initialize()
    {
        base.Initialize();
        _netManager.RegisterNetMessage<ChatClientServerMessage>();
    }

    public void Send(string message)
    {
        _netManager.ClientSendMessage(new ChatClientServerMessage()
        {
            Message = message
        });
    }
}