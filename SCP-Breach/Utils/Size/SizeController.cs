using System.Reflection;
using LabApi.Features.Wrappers;
using Mirror;
using UnityEngine;

namespace SCP_Breach.Utils.Size;

public abstract class SizeController
{
    public static void SetPlayerSize(Player player, float x, float y, float z)
    {
        var netIdentity = player.ReferenceHub.networkIdentity;
        player.ReferenceHub.gameObject.transform.localScale = new Vector3(1 * x, 1 * y, 1 * z);

        foreach (var connection in Player.GetAll().Select(serverPlayer => serverPlayer.ReferenceHub.connectionToClient))
        {
            typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.NonPublic | BindingFlags.Static)
                ?.Invoke(null, [netIdentity, connection]);
        }
    }
        
    public static void ResetSize(Player player)
    {
        var nId = player.ReferenceHub.networkIdentity;
        player.ReferenceHub.gameObject.transform.localScale = new Vector3(1, 1, 1);

        foreach (var nConn in Player.GetAll().Select(serverPlayer => serverPlayer.ReferenceHub.connectionToClient))
        {
            typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null,
                [nId, nConn]);
        }
    }
}