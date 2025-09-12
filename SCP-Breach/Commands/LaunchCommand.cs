using CommandSystem;
using LabApi.Features.Wrappers;

namespace SCP_Breach.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class LaunchCommand : ICommand
{
    public string Command => "launch";
    public string[] Aliases => new[] {"la"};
    public string Description => "Launch the player like a rocket or entier server";
    
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

        if (arguments[1].ToLower() == "all")
        {
            foreach (var player in Player.GetAll())
            {
                player.Velocity.Set(0, 10, 0);
                
                player.IsGodModeEnabled = true;
                
                player.SendHint("You have a short grace period", 4);
                
                player.SendConsoleMessage("You have a short grace period", "red");

                MEC.Timing.CallDelayed(10f, () =>
                {
                    player.IsGodModeEnabled = false;
                });
            }
            response = "All players have been launched";
        }
        else
        {
            int.TryParse(arguments[1], out var playerId); // parsing of playerID for use
            
            var player = Player.Get(playerId);

            if (player == null)
            {
                response = "Player not found, Make sure you have the correct playerId or use 'all' to launch all players";
                return false;       
            }

            player.Velocity.Set(0, 10, 0);
            
            player.IsGodModeEnabled = true;
                
            player.SendHint("You have a short grace period", 4);
                
            player.SendConsoleMessage("You have a short grace period", "red");

            MEC.Timing.CallDelayed(10f, () =>
            {
                player.IsGodModeEnabled = false;
            });
            
            response = "Player has been launched";       
        }
        
        return true;
    }
}