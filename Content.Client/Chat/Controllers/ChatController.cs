using System.Linq;
using Content.Shared.Chat;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Player;

namespace Content.Client.Chat.Controllers;

public sealed class ChatController : UIController, IChatHandler
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    private readonly List<IChatHandler> _handlers = new();
    private EntityUid _currentChannel;
    

    public override void Initialize()
    {
        EntityManager.EventBus.SubscribeLocalEvent<UserComponent, PlayerAttachedEvent>(OnAttached);
        SubscribeNetworkEvent<ChatSendEvent>(OnChatSend);
        SubscribeNetworkEvent<ChannelListEvent>(OnChannelListSend);
    }

    private void OnChannelListSend(ChannelListEvent msg, EntitySessionEventArgs args)
    {
        SetListChannels(EntityManager.GetEntityArray(msg.AvailableChannels));
    }

    private void OnAttached(EntityUid uid, UserComponent component, PlayerAttachedEvent args)
    {
        if(!args.Player.Equals(_playerManager.LocalSession)) return;
        
        SetLocalUsername(component.UserName);
    }
    
    private void OnChatSend(ChatSendEvent ev, EntitySessionEventArgs args)
    {
        var channelEnt = EntityManager.GetEntity(ev.ChannelId);
        if (_currentChannel != channelEnt)
        {
            _currentChannel = channelEnt;
            Clear();
            SetChannel(_currentChannel);
        }
        
        foreach (var chatEntry in ev.Entries)
        {
            AddMessage(chatEntry);
        }
    }
    
    public void RegisterChatHandler(IChatHandler chatHandler)
    {
        _handlers.Add(chatHandler);
    }

    public void UnregisterChatHandler(IChatHandler chatHandler)
    {
        _handlers.Remove(chatHandler);
    }

    public void SetListChannels(IEnumerable<EntityUid> channels)
    {
        var list = channels.ToList();
        foreach (var handler in _handlers)
        {
            handler.SetListChannels(list);
        }
    }

    public void AddMessage(ChatEntry message)
    {
        foreach (var handler in _handlers)
        {
            handler.AddMessage(message);
        }
    }

    public void Clear()
    {
        foreach (var handler in _handlers)
        {
            handler.Clear();
        }
    }

    public void SetLocalUsername(string name)
    {
        foreach (var handler in _handlers)
        {
            handler.SetLocalUsername(name);
        }
    }

    public void SetChannel(EntityUid channelUid)
    {
        foreach (var handler in _handlers)
        {
            handler.SetChannel(channelUid);
        }
    }
}


public interface IChatHandler
{
    public void SetListChannels(IEnumerable<EntityUid> channels);
    public void AddMessage(ChatEntry message);
    public void Clear();
    public void SetLocalUsername(string name);
    public void SetChannel(EntityUid channelUid);
}