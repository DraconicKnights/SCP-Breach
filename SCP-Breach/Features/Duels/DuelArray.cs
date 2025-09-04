using LabApi.Features.Wrappers;
using PlayerRoles;
using UnityEngine;

namespace SCP_Breach.Features.Duels;

public class DuelArray
{
    public static Dictionary<Player, Player> DuelRequests = new();
    public static Dictionary<Player, Player> InDuel = new();
    public static Vector3 DuelLocation = new();
    public static Dictionary<Player, Vector3> OriginalPositions = new();
    public static Dictionary<Player, RoleTypeId> OriginalRoles = new();
    public static Dictionary<Player, IReadOnlyCollection<Item>> OriginalItems = new();
    public static Dictionary<Player, List<Item>> PlayerDuelItems = new(); 
    public static List<Vector3> _locationsInUse = new();
}