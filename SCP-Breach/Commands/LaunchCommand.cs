using CommandSystem;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace SCP_Breach.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class LaunchCommand : ICommand
{
    public string Command => "launch";
    public string[] Aliases => new[] {"la"};
    public string Description => "Launch the player like a rocket or entier server";
    
    // Demonstration command. will move the players y coordinates up by 10 floats
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.Noclip))
        {
            response = "You don't have permission to use this command";
            return false;
        }
        
        if (arguments.Count < 1)
        {
            response = "Usage: launch <playerId> OR <all>";
            return false;       
        }
        
        var targetCall = arguments.ElementAt(0);
        
        var launchValue = arguments.ElementAt(1);

        var launch = 10f;

        if (launchValue != null)
        {
            float.TryParse(launchValue, out var resultValue);
            launch = resultValue;       
        }

        if (targetCall.ToLower() == "all")
        {
            foreach (var player in Player.GetAll())
            {
                LaunchPlayer(player, launch);       
            }
            response = "All players have been launched";
        }
        else
        {
            int.TryParse(arguments.ElementAt(0), out var playerId); // parsing of playerID for use
            
            var player = Player.Get(playerId);

            if (player == null)
            {
                response = "Player not found, Make sure you have the correct playerId or use 'all' to launch all players";
                return false;       
            }
            
            LaunchPlayer(player, launch);       
            
            response = "Player has been launched";       
        }
        
        return true;
    }
    
    private void LaunchPlayer(Player player, float launch)
    {
        var currenPos = player.Position;
        player.Position = new Vector3(currenPos.x, currenPos.y + launch, currenPos.z);
        
        player.IsGodModeEnabled = true;
                
        player.SendHint("You have a short grace period", 4);
                
        player.SendConsoleMessage("You have a short grace period", "red");

        MEC.Timing.CallDelayed(10f, () =>
        {
            player.IsGodModeEnabled = false;
        });
    }
}