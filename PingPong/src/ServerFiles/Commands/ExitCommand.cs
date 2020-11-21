namespace PingPong.Commands {
    class ExitCommand : ICommand {

        public string Name => "exit";

        public string Usage => "exit";

        public string Description => "Close the program";

        public void Execute(CommandArgs args) {
            args.CommandLine.Dispose();
        }

    }
}
