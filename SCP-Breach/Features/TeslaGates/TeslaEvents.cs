using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using SCP_Breach.Attributes;
using SCP_Breach.Config;
using SCP_Breach.Core;

namespace SCP_Breach.Features.TeslaGates;

[Event("Tesla Gate Control Handler", 0)]
public class TeslaEvents : ConditionalEventHandler<BreachConfig.TeslaGateSettings>
{
    public override void OnPlayerSpawned(PlayerSpawnedEventArgs ev)
    {
        base.OnPlayerSpawned(ev);
        
        Timing.CallDelayed(10f, () =>
        {
            foreach (var player in Player.GetAll())
            {
                if (player.Role != RoleTypeId.Scp096) return;
                
                TeslaControl.SCP096Active = true;
            }
            
        });
    }

    public override void OnPlayerTriggeringTesla(PlayerTriggeringTeslaEventArgs ev)
    {
        base.OnPlayerTriggeringTesla(ev);

        if (TeslaControl.SCP096Active)
        {
            if (ev.Player.Team != Team.SCPs) return;
            ev.IsAllowed = false;
            ev.Player.SendHint("Tesla Gate been disabled for passage by remote override by 096");
            ev.Player.SendConsoleMessage("Tesla Gate been disabled for passage by remote override by 096", "red");
        }
        else
        {
            if (ev.Player.Team != Team.FoundationForces) return;
            ev.IsAllowed = false;
            ev.Player.SendHint("Tesla Gate Disabled for NTF Passage");
            ev.Player.SendConsoleMessage("Tesla Gate Disabled for NTF Passage", "red");
        }
    }

    public override void OnPlayerDeath(PlayerDeathEventArgs ev)
    {
        base.OnPlayerDeath(ev);
        
        if (!TeslaControl.SCP096Active) return;
        
        if (ev.Player.Role != RoleTypeId.Scp096) return;
        
        TeslaControl.SCP096Active = false;
    }
    
    public override bool IsEnabled(BreachConfig.TeslaGateSettings section)
    {
        return section.TeslaGateSettingsEnabled;
    }
}