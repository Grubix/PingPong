namespace PingPong.Commands {
    class ClearCommand : ICommand {

        public string Name => "clear";

        public string Usage => "clear";

        public string Description => "Clears commands history";

        public void Execute(CommandArgs args) {
            args.CommandLine.Clear();
        }

    }
}
