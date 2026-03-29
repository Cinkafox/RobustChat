using Content.Server.FileManagment;
using Content.Shared.Chat;
using Content.Shared.FileManagment;
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
    [Dependency] private readonly FileManager _fileManager = default!;

    public override void Initialize()
    {
        base.Initialize();
        _netManager.RegisterNetMessage<ChatClientServerMessage>(OnMessageResieved);
        _netManager.RegisterNetMessage<ChatClientServerSelectChannelMessage>(OnSelectedChannel);
        _netManager.RegisterNetMessage<ChatClientServerSendFileMessage>(OnSendFile);
    }

    private void OnSendFile(ChatClientServerSendFileMessage message)
    {
        if(!TryGetEntity(message.MsgChannel, out var attachedEnt, out var currChannel)) 
            return;
        
        SendMessage(currChannel, attachedEnt,  null, _fileManager.AddFile(message.File));
    }

    private void OnSelectedChannel(ChatClientServerSelectChannelMessage message)
    {
        // TODO: ADD VALIDATION LATER
        
        if(!TryGetEntity(message.MsgChannel, out var attachedEnt, out var currChannel)) 
            return;
        
        SetChannel(attachedEnt, GetEntity(message.Channel));
    }
    
    private void OnMessageResieved(ChatClientServerMessage message)
    {
        if(!TryGetEntity(message.MsgChannel, out var attachedEnt, out var currChannel)) 
            return;
        
        SendMessage(currChannel, attachedEnt, message.Message, null);
    }

    private bool TryGetEntity(INetChannel channel, out EntityUid entityUid, out EntityUid channelUid)
    {
        var session = _playerManager.GetSessionByChannel(channel);
        entityUid = default!;
        channelUid = default!;

        if (session.AttachedEntity is null)
        {
            Log.Error("Unhandled session " + session.Name);
            return false;
        }

        entityUid = session.AttachedEntity.Value;
        channelUid = Transform(entityUid).ParentUid;
        return true;
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
            Entries = channel.Comp.ChatEntries.ToArray(), 
            ChannelId = GetNetEntity(channel)
        }, entityUid);
    }
    
    public void SendMessage(Entity<ChatChannelComponent?, MapComponent?> channel, EntityUid? userId, string? message, FileId? file)
    {
        if (!Resolve(channel.Owner, ref channel.Comp1, ref channel.Comp2))
        {
            Log.Error("Unhandled channel!");
            return;
        }

        var entry = new ChatEntry()
        {
            Message = message,
            File = file,
            Sender = GetNetEntity(userId),
            SendTime = DateTime.Now
        };
        
        channel.Comp1.ChatEntries.Add(entry);
        
        RaiseNetworkEvent(new ChatSendEvent()
        {
            Entries = [entry],
            ChannelId = GetNetEntity(channel)
        }, Filter.BroadcastMap(channel.Comp2.MapId));
    }
}