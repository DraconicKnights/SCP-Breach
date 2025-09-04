using LabApi.Events.Arguments.PlayerEvents;
using SCP_Breach.Attributes;
using SCP_Breach.Config;
using SCP_Breach.Core;

namespace SCP_Breach.Events;

[Event("Door Interaction Event",  0)]
public class DoorInteractEventHandler : ConditionalEventHandler<BreachConfig.DevTestEventsSettings>
{
    public override void OnPlayerInteractedDoor(PlayerInteractedDoorEventArgs ev)
    {
        base.OnPlayerInteractedDoor(ev);
        ev.Player.SendConsoleMessage($"Door Interacted: {ev.Door.NameTag} by player: {ev.Player.DisplayName}", "red");
    }

    public override bool IsEnabled(BreachConfig.DevTestEventsSettings section)
    {
        return section.DevEvents;
    }
}