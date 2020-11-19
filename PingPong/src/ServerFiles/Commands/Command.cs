using System;

namespace PingPong.Commands {
    class Command {

        public string Name { get; set; }

        public string Template { get; set; }

        public string Description { get; set; }

        public Action<CommandArgs> Action { get; set; }

        public void Execute(CommandArgs args) {
            Action.Invoke(args);
        }

    }
}
