using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using SCP_Breach.Attributes;

namespace SCP_Breach.Utils.Size;

[Event("Size Event Fix",  0)]
public class SizeEvent : CustomEventsHandler
{
    public override void OnPlayerDeath(PlayerDeathEventArgs ev)
    {
        base.OnPlayerDeath(ev);
        foreach (var player in Player.GetAll())
        {
            SizeController.ResetSize(player);
        }
    }
    
    
    public override void OnServerRoundEnded(RoundEndedEventArgs ev)
    {
        base.OnServerRoundEnded(ev);
        foreach (var player in Player.GetAll())
        {
            SizeController.ResetSize(player);
        }
    }
}