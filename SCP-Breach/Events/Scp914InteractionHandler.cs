using InventorySystem.Items;
using LabApi.Events.Arguments.Scp914Events;
using LabApi.Events.CustomHandlers;
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
   
    public override void OnScp914ProcessedPlayer(Scp914ProcessedPlayerEventArgs ev)
    {
        base.OnScp914ProcessedPlayer(ev);
        
        PlayerRoleCheck(ev.Player, ev.Player.Role, ev.KnobSetting);
    }

    public override void OnScp914ProcessingInventoryItem(Scp914ProcessingInventoryItemEventArgs ev)
    {
        base.OnScp914ProcessingInventoryItem(ev);

        foreach (var item in ev.Player.Items)
        {
            PlayerItemCheck(ev.Player, item, ev.KnobSetting);;
        }
    }

    private void PlayerItemCheck(Player player, Item item, Scp914KnobSetting knobSetting)
    {
        if (!GetConfig().Scp914Events.Scp914KnobSettings.KnobSettingEnabled.TryGetValue(knobSetting, out var enabled)) return;

        var transformations = GetItemTransformationDictionary(knobSetting);
        
        if (transformations.TryGetValue(item.Type, out var newItem))
        {
            try
            {
                player.RemoveItem(item);
                player.AddItem(newItem);
                
                Logger.Info($"Item {item.Type.ToString()} has been transformed to {newItem.ToString()}");
            }
            catch (Exception e)
            {
                Logger.Error($"Error while destroying item {item.Type.ToString()}: {e.Message}\n{e.StackTrace}");
                throw;
            }
        }
    }

    private void PlayerRoleCheck(Player player, RoleTypeId roleTypeId, Scp914KnobSetting knobSetting)
    {
        if (!GetConfig().Scp914Events.Scp914KnobSettings.KnobSettingEnabled.TryGetValue(knobSetting, out var enabled)) return;
        
        var transformations = GetRoleTransformationDictionary(knobSetting);
        
        if (transformations.TryGetValue(roleTypeId, out var newRole))
        {
            player.SetRole(newRole, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
        }
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