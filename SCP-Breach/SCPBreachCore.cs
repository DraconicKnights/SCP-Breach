using System.Reflection;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using SCP_Breach.Attributes;
using SCP_Breach.Config;
using SCP_Breach.Core;

namespace SCP_Breach;

public class SCPBreachCore : Plugin
{
    public override string Name => "SCP Breach";
    public override string Description => "A plugin for SCP SL";
    public override string Author => "Dragonoid";
    public override Version Version => new(1, 0, 0);
    public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

    private static SCPBreachCore _instance;
    private BreachConfig Config { get; set; }
    private EventManager EventManager { get; set; }
    
    public override void LoadConfigs()
    {
        base.LoadConfigs();
        Config = this.LoadConfig<BreachConfig>("config.yml")!;
    }

    public override void Enable()
    {
        _instance = this;
        Logger.Info("Plugins is being enabled");
        EventManager = new EventManager(Config);
    }

    public override void Disable()
    {
        Logger.Info("Plugins is being disabled");
        EventManager.UnregisterEvents();
    }
    
    public static SCPBreachCore Instance => _instance;
    
    public BreachConfig BreachConfig => Config;
}