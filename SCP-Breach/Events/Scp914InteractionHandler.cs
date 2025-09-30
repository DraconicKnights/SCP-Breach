using LabApi.Events.Arguments.Scp914Events;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Console;
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
    private readonly Dictionary<Player, IReadOnlyCollection<ItemType>> _itemsToGive = new();
    public override void OnServerRoundEnded(RoundEndedEventArgs ev)
    {
        base.OnServerRoundEnded(ev);
        _itemsToGive.Clear();
    }

    public override void OnScp914ProcessedPlayer(Scp914ProcessedPlayerEventArgs ev)
    {
        base.OnScp914ProcessedPlayer(ev);
        
        PlayerRoleCheck(ev.Player, ev.Player.Role, ev.KnobSetting);
    }

    public override void OnScp914ProcessedPickup(Scp914ProcessedPickupEventArgs ev)
    {
        base.OnScp914ProcessedPickup(ev);
        
        if (ev.Pickup == null) return;
    }

    public override void OnScp914ProcessingInventoryItem(Scp914ProcessingInventoryItemEventArgs ev)
    {
        base.OnScp914ProcessingInventoryItem(ev);
        
        ev.IsAllowed = false;
        
        PlayerItemCheck(ev.Player, ev.KnobSetting);
    }

    private void PlayerItemCheck(Player player, Scp914KnobSetting knobSetting)
    {
        if (!GetConfig().Scp914Events.Scp914KnobSettings.KnobSettingEnabled.TryGetValue(knobSetting, out var enabled)) return;

        Logger.Info($"Knob setting {knobSetting.ToString()} is enabled");
        var transformations = GetItemTransformationDictionary(knobSetting);
        
        var itemsToCheck = player.Items.ToList();
        
        Logger.Info($"Checking {itemsToCheck.Count} items for transformations");
        
        var newItemsArray = new List<ItemType>();
        
        foreach (var playerItem in itemsToCheck)
        {
            Logger.Info($"Checking item {playerItem.Type.ToString()} for transformations");

            if (!transformations.TryGetValue(playerItem.Type, out var newItem))
            {
                Logger.Info($"Item {playerItem.Type.ToString()} has no transformation");
                continue;
            }
            
            newItemsArray.Add(newItem);
        
            Logger.Info($"Item {playerItem.Type.ToString()} has been found");
            
            player.RemoveItem(playerItem);
                
            Logger.Info($"Item {playerItem.Type.ToString()} has been transformed to {newItem.ToString()}");
        }

        if (newItemsArray.Count > 0)
        {
            _itemsToGive.Add(player, newItemsArray.ToArray());
            
            MEC.Timing.CallDelayed(2f, () =>
            {
                foreach (var item in _itemsToGive[player])
                {
                    player.AddItem(item);
                }

                _itemsToGive.Remove(player);
            });
        }
    }

    private void PlayerRoleCheck(Player player, RoleTypeId roleTypeId, Scp914KnobSetting knobSetting)
    {
        if (!GetConfig().Scp914Events.Scp914KnobSettings.KnobSettingEnabled.TryGetValue(knobSetting, out var enabled)) return;
        
        var transformations = GetRoleTransformationDictionary(knobSetting);
        
        if (!transformations.TryGetValue(roleTypeId, out var newRole)) return;
        
        player.SetRole(newRole, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
    }
    
    private Dictionary<RoleTypeId, RoleTypeId> GetRoleTransformationDictionary(Scp914KnobSetting knobSetting)
    {
        return (knobSetting switch
        {
            Scp914KnobSetting.Rough => GetConfig().Scp914Events.Scp914KnobSettings.RoleRoughTransformations,
            Scp914KnobSetting.VeryFine => GetConfig().Scp914Events.Scp914KnobSettings.RoleVeryFineTransformations,
            _ => null
        })!;
    }
    
    private Dictionary<ItemType, ItemType> GetItemTransformationDictionary(Scp914KnobSetting knobSetting)
    {
        return (knobSetting switch
        {
            Scp914KnobSetting.Rough => GetConfig().Scp914Events.Scp914KnobSettings.ItemRoughTransformations,
            Scp914KnobSetting.VeryFine => GetConfig().Scp914Events.Scp914KnobSettings.ItemVeryFineTransformations,
            _ => null
        })!;
    }
    
    public override bool IsEnabled(BreachConfig.SCP914EventSettings section)
    {
        return section.Scp914Events;
    }
}