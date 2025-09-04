using CommandSystem;
using LabApi.Features.Wrappers;
using PlayerRoles;
using SCP_Breach.Utils.Size;

namespace SCP_Breach.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class SizeCommand : ICommand
{
    public string Command => "size";
    public string[] Aliases => new[] {"small"};
    public string Description => "Change the size of the target";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.Noclip))
        {
            response = "You don't have permission to use this command";
            return false;
        }

        if (arguments.Count < 1 || arguments.Count > 4)
        {
            response = "Usage: size <all/reset/scp/playerId> <x> <y> <z>";
            Player.Get(sender)!.SendConsoleMessage("Usage: size <all/reset/scp/playerId> <x> <y> <z>");
            return false;
        }

        switch (arguments.Array?[1])
        {
            case "all":
            {
                if (!float.TryParse(arguments.Array?[2], out var x) || !float.TryParse(arguments.Array?[3], out var y) ||
                    !float.TryParse(arguments.Array?[4], out var z))
                {
                    response = "Valid scale has not been provided";
                    return false;
                }
            
                foreach (var player in Player.GetAll())
                {
                    SizeController.SetPlayerSize(player, x, y, z);
                }
            
                response = "All players scale has been adjusted";
                break;
            }
            case "reset":
            {
                foreach (var player in Player.GetAll())
                {
                    SizeController.ResetSize(player);
                }
            
                response = "All players scale has been reset";
                break;
            }
            case "scp":
            {
                if (!float.TryParse(arguments.Array?[2], out var x) || !float.TryParse(arguments.Array?[3], out var y) ||
                    !float.TryParse(arguments.Array?[4], out var z))
                {
                    response = "Valid scale has not been provided";
                    return false;
                }
            
                foreach (var player in Player.GetAll())
                {
                    if (player.Team != Team.SCPs) continue;
                    SizeController.SetPlayerSize(player, x, y, z);
                    response = "All SCPs have been reset";
                }

                break;
            }
            default:
            {
                if (!int.TryParse(arguments.Array?[1], out var playerId))
                {
                    response = "Valid playerId has not been provided";
                    return false;
                }

                var senderPlayer = Player.Get(playerId);
                if (senderPlayer == null)
                {
                    response = "Player not found";
                    return false;
                }

                if (!float.TryParse(arguments.Array?[2], out var x) || !float.TryParse(arguments.Array?[3], out var y) ||
                    !float.TryParse(arguments.Array?[4], out var z))
                {
                    response = "Valid scale has not been provided";
                    return false;
                }
            
                SizeController.SetPlayerSize(senderPlayer, x, y, z);
            
                response = "Player scale has been adjusted";
                break;
            }
        }

        response = null!;
        return true;
    }
}