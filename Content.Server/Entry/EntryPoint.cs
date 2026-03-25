using Content.Server.Acz;
using Content.Server.GameTicking;
using Robust.Server.ServerStatus;
using Robust.Shared.ContentPack;

namespace Content.Server.Entry;

public sealed class EntryPoint : GameServer
{
    [Dependency] private readonly IStatusHost _host = default!;
    
    public override void Init()
    {
        Dependencies.InjectDependencies(this);
        var aczProvider = new ContentMagicAczProvider(Dependencies);
        _host.SetMagicAczProvider(aczProvider);
    }

    public override void PostInit()
    {
        base.PostInit();
        IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<GameTicker>().PostInitialize();
    }
}