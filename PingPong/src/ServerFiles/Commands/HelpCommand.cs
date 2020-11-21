namespace PingPong.Commands {
    class HelpCommand : ICommand {

        public string Name => "help";

        public string Usage => "help [<command>]";

        public string Description => "Prints all available commands or displays specified command description";

        public void Execute(CommandArgs args) {
            if (args.UserArgs.Length == 0) {
                args.CommandLine.Log("Available commands: " + string.Join(", ", args.RegisteredCommands.Keys));
                return;
            }

            if (args.RegisteredCommands.ContainsKey(args.UserArgs[0])) {
                ICommand command = args.RegisteredCommands[args.UserArgs[0]];
                args.CommandLine.Log($"Usage: {command.Usage}");
                args.CommandLine.Log($"Description: {command.Description}");
            } else {
                args.CommandLine.Error($"Unknown command '{args.UserArgs[0]}'");
            }
        }

    }
}
