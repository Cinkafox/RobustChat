using Content.Server.Chat;
using Robust.Shared.Player;
using UserComponent = Content.Shared.Chat.UserComponent;

namespace Content.Server.GameTicking;

public partial class GameTicker
{
    private void AddSession(ICommonSession session)
    {
        var userId = Spawn();
        var comp = AddComp<UserComponent>(userId);
        comp.UserName = session.Name;
        _playerManager.SetAttachedEntity(session, userId);
        _channelSystem.SetChannel(userId, DefaultChannel);
        Dirty(userId, comp);
        _channelSystem.SendMessage(DefaultChannel, null, "WELCOME! " + comp.UserName);
    }
}