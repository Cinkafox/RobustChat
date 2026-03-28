using System.Linq;
using Content.Shared.Chat;
using Robust.Server.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server.Chat;

public sealed class ChannelSystem : EntitySystem
{
    [Dependency] private readonly MapSystem _mapSystem = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;
    [Dependency] private readonly INetManager _netManager = default!;
    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;

    public override void Initialize()
    {
        base.Initialize();
        _netManager.RegisterNetMessage<ChatClientServerMessage>(OnMessageResieved);
        _netManager.RegisterNetMessage<ChatClientServerSelectChannelMessage>(OnSelectedChannel);
    }

    private void OnSelectedChannel(ChatClientServerSelectChannelMessage message)
    {
        // TODO: ADD VALIDATION LATER
        
        var session = _playerManager.GetSessionByChannel(message.MsgChannel);

        if (session.AttachedEntity is null)
        {
            Log.Error("Unhandled session " + session.Name);
            return;
        }
        
        SetChannel(session.AttachedEntity.Value, GetEntity(message.Channel));
    }

    public void SendUserChannels(EntityUid userId)
    {
        var query = EntityQueryEnumerator<ChatChannelComponent>();
        var ev = new ChannelListEvent();
        var channels = new List<NetEntity>();
        
        while (query.MoveNext(out var uid, out var chatChannelComponent))
        {
            channels.Add(GetNetEntity(uid));
        }
        
        ev.AvailableChannels = channels.ToArray();
        RaiseNetworkEvent(ev, userId);
    }

    private void OnMessageResieved(ChatClientServerMessage message)
    {
        var session = _playerManager.GetSessionByChannel(message.MsgChannel);

        if (session.AttachedEntity is null)
        {
            Log.Error("Unhandled session " + session.Name);
            return;
        }

        var currChannel = Transform(session.AttachedEntity.Value).ParentUid;
        SendMessage(currChannel, session.AttachedEntity.Value, message.Message);
    }

    public EntityUid CreateChannel(string name)
    {
        var entityUid = _mapSystem.CreateMap();
        var comp = AddComp<ChatChannelComponent>(entityUid);
        comp.ChannelName = name;
        Dirty(entityUid, comp);
        return entityUid;
    }

    public void SetChannel(EntityUid entityUid, Entity<ChatChannelComponent?> channel)
    {
        if (!Resolve(channel.Owner, ref channel.Comp))
            throw new Exception();
        
        _transformSystem.SetParent(entityUid, channel);
        
        RaiseNetworkEvent(new ChatSendEvent()
        {
            Entries = channel.Comp.ChatEntries.ToArray(), ClearRequired = true
        }, entityUid);
    }
    
    public void SendMessage(Entity<ChatChannelComponent?, MapComponent?> channel, EntityUid? userId, string message)
    {
        if (!Resolve(channel.Owner, ref channel.Comp1, ref channel.Comp2))
        {
            Log.Error("Unhandled channel!");
            return;
        }

        var entry = new ChatEntry()
        {
            Message = message,
            Sender = GetNetEntity(userId),
            SendTime = DateTime.Now
        };
        
        channel.Comp1.ChatEntries.Add(entry);
        
        RaiseNetworkEvent(new ChatSendEvent()
        {
            Entries = [entry]
        }, Filter.BroadcastMap(channel.Comp2.MapId));
    }
}