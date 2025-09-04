using CommandSystem;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace SCP_Breach.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]   
public class LocationCommand : ICommand
{
    public string Command => "location";
    public string[] Aliases => new[] {"loc"};
    public string Description => "Get the current Vector 3 location of the player";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.Noclip))
        {
            response = "You don't have permission to use this command";
            return false;
        }
        
        Vector3 loc = Player.Get(sender).ReferenceHub.transform.position;
        
        response = $"Location: {loc.x}, {loc.y}, {loc.z}";
        return true;   
    }
}