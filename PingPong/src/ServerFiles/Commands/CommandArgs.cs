using PingPong.Forms;
using PingPong.KUKA;
using PingPong.OptiTrack;
using System.Collections.Generic;

namespace PingPong.Commands {
    public class CommandArgs {

        public Dictionary<string, ICommand> RegisteredCommands { get; }

        public CmdWindow CommandLine { get; }

        public KUKARobot Robot1 { get; }

        public KUKARobot Robot2 { get; }

        public OptiTrackSystem OptiTrack { get; }

        public string[] UserArgs { get; }

        public CommandArgs(Dictionary<string, ICommand> registeredCommands, CmdWindow cmdWindow,
            KUKARobot robot1, KUKARobot robot2, OptiTrackSystem optiTrack, string[] userArgs) {
            RegisteredCommands = registeredCommands;
            CommandLine = cmdWindow;
            Robot1 = robot1;
            Robot2 = robot2;
            OptiTrack = optiTrack;
            UserArgs = userArgs;
        }

    }
}
