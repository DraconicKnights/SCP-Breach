using CommandSystem;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using SCP_Breach.Features.Duels;

namespace SCP_Breach.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class DuelCommand : ICommand
{
    public string Command => "duel";
    public string[] Aliases => new[] {"dp"};
    public string Description => "Create a duel request with another player";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!SCPBreachCore.Instance.BreachConfig.DuelSettingsValue.DuelEvents)
        {
            response = "Duel events are disabled";
            return false;
        }
        
        var requestingplayer = Player.Get(sender);

        if (SCPBreachCore.Instance.BreachConfig.DuelSettingsValue.SpectatorOnly)
        {
            if (requestingplayer.Role != RoleTypeId.Spectator)
            {
                response = "You must be a spectator to use this command";
                return false;
            }
        }

        if (arguments.Count < 1)
        {
            response = "Usage: duel <name>";
            return false;
        }

        var targetName = arguments.ElementAt(0);
        Player.TryGetPlayersByName(targetName, out List<Player> players);

        Player target = null;
        
        foreach (var player in players.Where(player => player.DisplayName == targetName))
        {
            target = player;
            break;
        }

        if (SCPBreachCore.Instance.BreachConfig.DuelSettingsValue.SpectatorOnly)
        {
            if (target.Role != RoleTypeId.Spectator)
            {
                response = "Target must be a spectator to duel";
                return false;
            }
        }
        
        if (target == null)
        {
            response = "Target not found";
            return false;
        }
        
        if (target == requestingplayer)
        {
            response = "You cannot duel yourself";
            return false;
        }

        if (DuelArray.DuelRequests.ContainsKey(requestingplayer) || DuelArray.InDuel.ContainsKey(target))
        {
            response = "One or both players are already in a duel";
            return false;
        }
        
        DuelArray.DuelRequests[target] = requestingplayer;
        target.SendHint($"{requestingplayer.Nickname} has challenged you to a duel! Type `.duel accept` to accept or `.duel decline` to decline.", 10);
        response = $"Duel request sent to player: {target.Nickname}";

        Timing.CallDelayed(25f, () =>
        {
            if (DuelArray.DuelRequests.ContainsKey(target) &&
                DuelArray.DuelRequests[target] == requestingplayer)
            {
                DuelArray.DuelRequests.Remove(target);

                target.SendHint("Duel request timed out.", 5);
                target.SendConsoleMessage($"Duel request by player: {requestingplayer.Nickname} has timed out.", "red");
                requestingplayer.SendHint($"Your duel request to {target.Nickname} has timed out.", 5);
                requestingplayer.SendConsoleMessage($"Duel request to player: {target.Nickname} has timed out.", "red");
            }
        });

        return true;
    }
}

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class DuelSubCommand : ICommand
{
    public string Command => "accept";
    public string[] Aliases => [];
    public string Description => "Create a duel request with another player";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!SCPBreachCore.Instance.BreachConfig.DuelSettingsValue.DuelEvents)
        {
            response = "Duel events are disabled";
            return false;
        }
        
        var acceptingPlayer = Player.Get(sender);

        if (acceptingPlayer.IsAlive)
        {
            response = "You can't use this while alive.";
            return false;
        }

        if (!DuelArray.DuelRequests.ContainsKey(acceptingPlayer))
        {
            response = "You have no pending duel requests.";
            return false;
        }

        var requestingPlayer = DuelArray.DuelRequests[acceptingPlayer];
        DuelArray.DuelRequests.Remove(acceptingPlayer);

        // Start the duel
        DuelControl.StartDuel(requestingPlayer, acceptingPlayer);

        response = $"You accepted the duel request from {requestingPlayer.Nickname}.";
        return true;
    }
}

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class DuelSubCommand2 : ICommand
{
    public string Command => "decline";
    public string[] Aliases => [];
    public string Description => "Create a duel request with another player";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!SCPBreachCore.Instance.BreachConfig.DuelSettingsValue.DuelEvents)
        {
            response = "Duel events are disabled";
            return false;
        }
        
        var decliningPlayer = Player.Get(sender);

        if (!DuelArray.DuelRequests.ContainsKey(decliningPlayer))
        {
            response = "You have no pending duel requests.";
            return false;
        }

        var requestingPlayer = DuelArray.DuelRequests[decliningPlayer];
        DuelArray.DuelRequests.Remove(decliningPlayer);

        requestingPlayer.SendHint($"{decliningPlayer.Nickname} has declined your duel request.", 5);
        response = $"You declined the duel request from {requestingPlayer.Nickname}.";
        return true;
    }
}
[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class ForceDuelControl : ICommand
{
    public string Command => "forceduel";
    public string[] Aliases => new[] {"fd"};
    public string Description => "Force a duel between two players";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!SCPBreachCore.Instance.BreachConfig.DuelSettingsValue.DuelEvents)
        {
            response = "Duel events are disabled";
            return false;
        }
        
        if (!sender.CheckPermission(PlayerPermissions.Noclip))
        {
            response = "You do not have permission to use this command.";
            return false;
        }
                
        if (arguments.Count < 2)
        {
            response = "Usage: forceduel <Player1Name/ID> <Player2Name/ID> [OptionalChoice]";
            return false;
        }

        var player1Identifier = arguments.ElementAt(0);
        var player2Identifier = arguments.ElementAt(1);

        var player1 = TryGetPlayer(player1Identifier);
        var player2 = TryGetPlayer(player2Identifier);
                
        if (player1 == null || player2 == null)
        {
            response = "One or both players not found.";
            return false;
        }

        if (player1 == player2)
        {
            response = "You cannot force a duel with the same player.";
            return false;
        }

        if (DuelArray.InDuel.ContainsKey(player1) || DuelArray.InDuel.ContainsValue(player2))
        {
            response = "One or both players are already in a duel.";
            return false;
        }

        DuelControl.StartDuel(player1, player2);
        response = $"Duel forced between {player1.Nickname} and {player2.Nickname}.";
        return true;
    }
    
    private static Player TryGetPlayer(string identifier)
    {
        var player = int.TryParse(identifier, out int playerId) ? Player.Get(playerId) :
            Player.Get(identifier);
        return player;
    }
}