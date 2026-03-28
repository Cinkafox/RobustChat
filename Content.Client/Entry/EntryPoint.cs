using Content.Client.Connection;
using Content.Shared.Input;
using Content.StyleSheetify.Client.StyleSheet;
using Robust.Client;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Stylesheets;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;

namespace Content.Client.Entry;

public sealed class EntryPoint : GameClient
{
    [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
    [Dependency] private readonly IStateManager _stateManager = default!;
    [Dependency] private readonly IInputManager _inputManager = default!;
    [Dependency] private readonly IGameController _gameController = default!;
    [Dependency] private readonly IBaseClient _baseClient = default!;
    [Dependency] private readonly IConfigurationManager _configManager = default!;
    [Dependency] private readonly IContentStyleSheetManager _contentStyleSheetManager = default!;
    [Dependency] private readonly IResourceCache _resourceManager = default!;
    
    
    public override void PreInit()
    {
        IoCManager.BuildGraph();
        IoCManager.InjectDependencies(this);
        
        //AUTOSCALING default Setup!
        _configManager.SetCVar("interface.resolutionAutoScaleUpperCutoffX", 1080);
        _configManager.SetCVar("interface.resolutionAutoScaleUpperCutoffY", 720);
        _configManager.SetCVar("interface.resolutionAutoScaleLowerCutoffX", 520);
        _configManager.SetCVar("interface.resolutionAutoScaleLowerCutoffY", 240);
        _configManager.SetCVar("interface.resolutionAutoScaleMinimum", 0.5f);
    }
    
    public override void PostInit()
    { 
        _userInterfaceManager.SetDefaultTheme("DefaultTheme");
        _userInterfaceManager.Stylesheet = _contentStyleSheetManager.MergeStyles(
            new DefaultStylesheet(_resourceManager, _userInterfaceManager).Stylesheet,
            "default");
       
        ContentContexts.SetupContexts(_inputManager.Contexts);
        _userInterfaceManager.MainViewport.Visible = false;
       
        _baseClient.RunLevelChanged += (_, args) =>
        {
            if (args.NewLevel == ClientRunLevel.Initialize)
            {
                SwitchState(args.OldLevel is ClientRunLevel.Connected or ClientRunLevel.InGame);
            }
        };
       
        SwitchState();
    }
    
    private void SwitchState(bool disconnected = false)
    {
        _stateManager.RequestStateChange<ConnectingState>();
        var state = (ConnectingState)_stateManager.CurrentState;
        
        if(disconnected)
        {
            state.Message("Disconnected..");
            return;
        }

        if (_gameController.LaunchState.FromLauncher) 
            return;

        state.ConnectToLocal();
    }
}