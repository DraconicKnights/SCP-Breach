using Interactables.Interobjects;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using SCP_Breach.Attributes;
using SCP_Breach.Config;
using SCP_Breach.Core;
using CheckpointDoor = LabApi.Features.Wrappers.CheckpointDoor;

namespace SCP_Breach.Events;

[Event("Keycard Inventory Event", 0)]
public class KeycardInventoryHandler : ConditionalEventHandler<BreachConfig.KeycardEventSettings>
{
    public override void OnPlayerInteractedDoor(PlayerInteractedDoorEventArgs ev)
    {
        base.OnPlayerInteractedDoor(ev);
        
        if (ev.CanOpen)
            return;
        
        DoorEventExecution(ev.Player, ev.Door);;
    }

    private void DoorEventExecution(Player player, Door door)
    {
        switch (door)
        {
            case CheckpointDoor checkpointDoor:
                foreach (var item in player.Items)
                {
                    switch (item.Type)
                    {
                        case ItemType.KeycardO5:
                            if (!door.Base.IsConsideredOpen())
                                checkpointDoor.Base.NetworkTargetState = true;
                            if (door.Base.IsConsideredOpen())
                                checkpointDoor.Base.NetworkTargetState = false;
                            break;
                    }
                }
                break;
        }

        if (door.Base is PryableDoor)
        {
            var pryable = door.Base as PryableDoor;
            
            foreach (var item in player.Items)
            {
                switch (item.Type)
                {
                    case ItemType.KeycardO5:
                        if (!door.Base.IsConsideredOpen())
                            pryable.NetworkTargetState = true;
                        if (door.Base.IsConsideredOpen())
                            pryable.NetworkTargetState = false;
                        break;
                }
            }
        }

        if (door.Base is not PryableDoor || door is not CheckpointDoor) return;
        {
            foreach (var item in player.Items)
            {
                switch (item.Type)
                {
                    case ItemType.KeycardO5:
                        if (!door.Base.IsConsideredOpen())
                            door.Base.NetworkTargetState = true;
                        if (door.Base.IsConsideredOpen())
                            door.Base.NetworkTargetState = false;
                        break;
                }
            }
        }
    }

    public override bool IsEnabled(BreachConfig.KeycardEventSettings section)
    {
        return section.KeycardEvents;
    }
}