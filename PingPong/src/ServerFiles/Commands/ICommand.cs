namespace PingPong.Commands {
    public interface ICommand {

        string Name { get; }

        string Usage { get; }

        string Description { get; }

        void Execute(CommandArgs args);

    }
}
