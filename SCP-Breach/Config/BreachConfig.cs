using System.ComponentModel;
using PlayerRoles;
using SCP_Breach.Utils.Data;

namespace SCP_Breach.Config;

public class BreachConfig
{
    [Description("Dev Events")] public DevTestEventsSettings DevEvents { get; set; } = new();
    [Description("SCP 914 Events")] public SCP914EventSettings Scp914Events { get; set; } = new();
    [Description("Keycard Events")] public KeycardEventSettings KeycardEvents { get; set; } = new();
    [Description("Tesla Gate Settings")] public TeslaGateSettings TeslaGateEvents { get; set; } = new();
    [Description("Duel Settings")] public DuelSettings DuelSettingsValue { get; set; } = new ();
    
    public class DevTestEventsSettings
    {
        [Description("Dev Test Events")]
        public bool DevEvents { get; set; } = false;
    }
    
    public class SCP914EventSettings
    {
        [Description("SCP 914 Events")]
        public bool Scp914Events { get; set; } = true;
    }

    public class KeycardEventSettings
    {
        [Description("Keycard Events")]
        public bool KeycardEvents { get; set; } = true;
    }
    
    public class TeslaGateSettings
    {
        [Description("Tesla Gate Bypass Config")]
        public bool TeslaGateSettingsEnabled { get; set; } = true;
    }
    
    public class DuelSettings
    {
        [Description("Determines if duels are enabled or not")]
        public bool DuelEvents { get; set; } = false;
        
        [Description("List of locations for duels")]
        public List<SerializableVector3> DuelLocations { get; set; } = new List<SerializableVector3>
        {
            new SerializableVector3(39.4f, 314.9f, -32.5f),
            new SerializableVector3(-16.5f, 314.9f, -31.6f),
            new SerializableVector3(-0.0f, 296.9f, -7.8f)
        };

        [Description("List of weapons available in the duel weapon pool")]
        public List<ItemType> WeaponPool { get; set; } = new List<ItemType>()
        {
            ItemType.GunA7,
            ItemType.GunCOM15,
            ItemType.GunCOM18,
            ItemType.GunE11SR,
            ItemType.GunAK,
            ItemType.GunRevolver
        };

        [Description("The duel player will only spawn as the Tutorial Role")]
        public bool TutorialOnly { get; set; } = false;

        [Description("The role that will be used for the duel")]
        public List<RoleTypeId> DuelRoles { get; set; } = new List<RoleTypeId>()
        {
            RoleTypeId.ClassD,
            RoleTypeId.Scientist,
            RoleTypeId.ChaosConscript,
            RoleTypeId.NtfPrivate,
            RoleTypeId.FacilityGuard,
            RoleTypeId.NtfCaptain,
            RoleTypeId.Tutorial
        };
            
        [Description("If true, only spectators can use duels")]
        public bool SpectatorOnly { get; set; } = false;
        [Description("If true, both players receive the same weapon in duels")]
        public bool GetSameWeapon { get; set; } = false;
    }
}