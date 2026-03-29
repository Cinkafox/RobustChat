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
        _netManager.RegisterNetMessage<ChatClientServerSelectChannelMessage>();
        _netManager.RegisterNetMessage<ChatClientServerSendFileMessage>();
    }

    public void SendFile(string fileName, byte[] file)
    {
        _netManager.ClientSendMessage(new ChatClientServerSendFileMessage()
        {
            File = file, FileName = fileName
        });
    }

    public void SendSelectedChannel(EntityUid channel)
    {
        _netManager.ClientSendMessage(new ChatClientServerSelectChannelMessage()
        {
            Channel = GetNetEntity(channel)
        });
    }

    public void Send(string message)
    {
        if(string.IsNullOrEmpty(message)) 
            return;
        
        _netManager.ClientSendMessage(new ChatClientServerMessage()
        {
            Message = message
        });
    }
}