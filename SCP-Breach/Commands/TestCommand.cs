using CommandSystem;

namespace SCP_Breach.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class TestCommand : ICommand
{
    public string Command => "test";
    public string[] Aliases => new[] {"t"};
    public string Description => "Test command";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        response = "Test command";
        return true;
    }
}