using Content.Shared.Chat;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;

namespace Content.Client.Chat.Controllers;

public sealed class ChatController : UIController, IChatHandler
{
    [UISystemDependency] private readonly ChatSystem _chatSystem = default!;

    public List<IChatHandler> Handlers = new List<IChatHandler>();
    
    public void RegisterChatHandler(IChatHandler chatHandler)
    {
        Handlers.Add(chatHandler);
    }

    public void UnregisterChatHandler(IChatHandler chatHandler)
    {
        Handlers.Remove(chatHandler);
    }

    public void AddMessage(UserComponent component, string message)
    {
        foreach (var handler in Handlers)
        {
            handler.AddMessage(component, message);
        }
    }

    public void Clear()
    {
        foreach (var handler in Handlers)
        {
            handler.Clear();
        }
    }

    public void SetLocalUsername(string name)
    {
        foreach (var handler in Handlers)
        {
            handler.SetLocalUsername(name);
        }
    }
}


public interface IChatHandler
{
    public void AddMessage(UserComponent component, string message);
    public void Clear();
    public void SetLocalUsername(string name);
}