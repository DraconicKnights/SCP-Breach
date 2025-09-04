using LabApi.Events.Arguments.Scp914Events;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using PlayerRoles;
using SCP_Breach.Attributes;
using SCP_Breach.Config;
using SCP_Breach.Core;
using Scp914;

namespace SCP_Breach.Events;

[Event("SCP 914 Interaction Event", 1)]
public class Scp914InteractionHandler : ConditionalEventHandler<BreachConfig.SCP914EventSettings>
{
    public override void OnScp914ProcessedPlayer(Scp914ProcessedPlayerEventArgs ev)
    {
        base.OnScp914ProcessedPlayer(ev);
        
        var player = ev.Player; 
        
        PlayerRoleCheck(player, player.Role, ev.KnobSetting);
    }

    public override void OnScp914ProcessingInventoryItem(Scp914ProcessingInventoryItemEventArgs ev)
    {
        base.OnScp914ProcessingInventoryItem(ev);

        foreach (var item in ev.Player.Items)
        {
            
        }
    }

    private void PlayerRoleCheck(Player player, RoleTypeId roleTypeId, Scp914KnobSetting knobSetting)
    {
        switch (knobSetting)
        {
            case Scp914KnobSetting.Rough:
                switch (roleTypeId)
                {
                    case RoleTypeId.ClassD:
                        player.SetRole(RoleTypeId.Scp0492, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                        break;
            
                    case RoleTypeId.ChaosConscript:
                        player.SetRole(RoleTypeId.ClassD, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                        break;
                    case RoleTypeId.ChaosRifleman:
                        player.SetRole(RoleTypeId.ChaosConscript, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                        break;
                    case RoleTypeId.ChaosMarauder:
                        player.SetRole(RoleTypeId.ChaosRifleman,  RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                        break;
                    case RoleTypeId.ChaosRepressor:
                        player.SetRole(RoleTypeId.ChaosMarauder,  RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                        break;
            
                    case RoleTypeId.FacilityGuard:
                        player.SetRole(RoleTypeId.Scientist,  RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                        break;
                    case RoleTypeId.NtfPrivate:
                        player.SetRole(RoleTypeId.FacilityGuard,  RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                        break;
                    case RoleTypeId.NtfSergeant:
                        player.SetRole(RoleTypeId.NtfPrivate,  RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                        break;
                    case RoleTypeId.NtfCaptain:
                        player.SetRole(RoleTypeId.NtfSergeant,  RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                        break;
            
                    case RoleTypeId.Scientist:
                        player.SetRole(RoleTypeId.ClassD,  RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                        break;
                }
                break;
            case Scp914KnobSetting.Coarse:
                break;
            case Scp914KnobSetting.OneToOne:
                break;
            case Scp914KnobSetting.Fine:
                break;
            case Scp914KnobSetting.VeryFine:
                switch (roleTypeId)
                {
                    case RoleTypeId.ClassD:
                        player.SetRole(RoleTypeId.Scientist,  RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                        break;
            
                    case RoleTypeId.ChaosConscript:
                        player.SetRole(RoleTypeId.ChaosRifleman,  RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                        break;
                    case RoleTypeId.ChaosRifleman:
                        player.SetRole(RoleTypeId.ChaosConscript,  RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                        break;
                    case RoleTypeId.ChaosMarauder:
                        player.SetRole(RoleTypeId.ChaosRepressor,  RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                        break;
            
                    case RoleTypeId.FacilityGuard:
                        player.SetRole(RoleTypeId.NtfPrivate,  RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                        break;
                    case RoleTypeId.NtfPrivate:
                        player.SetRole(RoleTypeId.NtfSergeant,  RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                        break;
                    case RoleTypeId.NtfSergeant:
                        player.SetRole(RoleTypeId.NtfCaptain,  RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                        break;
                }
                break;
        }
        
    }
    
    public override bool IsEnabled(BreachConfig.SCP914EventSettings section)
    {
        return section.Scp914Events;
    }
}