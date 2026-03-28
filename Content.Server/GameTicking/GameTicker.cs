using Content.Server.Chat;
using Robust.Server.Console;
using Robust.Shared.Console;
using Robust.Shared.Enums;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Toolshed;
using Robust.Shared.Toolshed.Errors;

namespace Content.Server.GameTicking;


public sealed partial class GameTicker : EntitySystem
{
    [Dependency] private readonly IConGroupController _groupController = default!;
    [Dependency] private readonly ChannelSystem _channelSystem = default!;
    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;

    public ContentGroupController GroupController = new ContentGroupController();

    public EntityUid DefaultChannel = default!;

    public override void Initialize()
    {
        base.Initialize();
        _playerManager.PlayerStatusChanged += OnPlayerStatusChanged;
        _groupController.Implementation = GroupController;
    }

    public void PostInitialize()
    {
        DefaultChannel = _channelSystem.CreateChannel("default");
    }

    private void OnPlayerStatusChanged(object? sender, SessionStatusEventArgs args)
    {
        var session = args.Session;
        
        switch (args.NewStatus)
        {
            case SessionStatus.Connected:
                Timer.Spawn(0, () => _playerManager.JoinGame(session));
                Log.Info($"{session.Name} was connected");
                break;
            case SessionStatus.InGame:
                AddSession(session);
                break;
            case SessionStatus.Disconnected:
                RemoveSession(session);
                break;
        }
    }
}

public sealed class SetHostCommand : LocalizedCommands
{
    public override string Command { get; } = "sethost";
    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        IoCManager.Resolve<IEntityManager>().System<GameTicker>().GroupController.AdminName = args[1];
        shell.WriteLine("Host is updated!");
    }
}

// Смешное
public sealed class ContentGroupController : IConGroupControllerImplementation
{
    public string AdminName = "localhost@JoeGenero";
    
    public bool CheckInvokable(CommandSpec command, ICommonSession? user, out IConError? error)
    {
        error = null;
        return user?.Name == AdminName;
    }

    public bool CanCommand(ICommonSession session, string cmdName)
    {
        Logger.Debug(session.Name + " " + AdminName);
        return session.Name == AdminName;
    }

    public bool CanAdminPlace(ICommonSession session)
    {
        Logger.Debug(session.Name + " " + AdminName);
        return session.Name == AdminName;
    }

    public bool CanScript(ICommonSession session)
    {
        return session.Name == AdminName;
    }

    public bool CanAdminMenu(ICommonSession session)
    {
        return session.Name == AdminName;
    }

    public bool CanAdminReloadPrototypes(ICommonSession session)
    {
        return session.Name == AdminName;
    }
}