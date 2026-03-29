using Content.Server.Chat;
using Robust.Shared.Network;
using Robust.Shared.Player;
using UserComponent = Content.Shared.Chat.UserComponent;

namespace Content.Server.GameTicking;

public partial class GameTicker
{
    private Dictionary<NetUserId, EntityUid> _usersCache = [];
    
    private void AddSession(ICommonSession session)
    {
        if (!_usersCache.TryGetValue(session.UserId, out var userId) ||
            !TryComp<UserComponent>(userId, out var comp))
        {
            userId = Spawn();
            comp = AddComp<UserComponent>(userId);
            comp.UserName = session.Name;
            _usersCache[session.UserId] = userId;
            
            _playerManager.SetAttachedEntity(session, userId);
            _channelSystem.SendUserChannels(userId);
            _channelSystem.SetChannel(userId, DefaultChannel);
            Dirty(userId, comp);
        }
        
        _channelSystem.SendMessage(DefaultChannel, null, "Connected to the server! " + comp.UserName, null);
    }

    private void RemoveSession(ICommonSession session)
    {
        if(!_usersCache.TryGetValue(session.UserId, out var user) || 
           !TryComp<UserComponent>(user, out var userComp)) 
            return;
        
        _channelSystem.SendMessage(DefaultChannel, null, "Disconnected from the server! " + userComp.UserName, null);
    }
}