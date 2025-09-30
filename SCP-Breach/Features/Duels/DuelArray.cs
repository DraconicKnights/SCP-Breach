using LabApi.Features.Wrappers;
using PlayerRoles;
using UnityEngine;

namespace SCP_Breach.Features.Duels;

public class DuelArray
{
    public static readonly Dictionary<Player, Player> DuelRequests = new();
    public static readonly Dictionary<Player, Player> InDuel = new();
    public static readonly Dictionary<Player, Vector3> OriginalPositions = new();
    public static readonly Dictionary<Player, RoleTypeId> OriginalRoles = new();
    public static readonly Dictionary<Player, IReadOnlyCollection<Item>> OriginalItems = new();
    public static readonly Dictionary<Player, List<Item>> PlayerDuelItems = new(); 
    public static List<Vector3> _locationsInUse = new();
}