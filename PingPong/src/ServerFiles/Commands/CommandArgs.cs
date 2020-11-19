using PingPong.Forms;
using PingPong.KUKA;
using PingPong.OptiTrack;

namespace PingPong.Commands {
    class CommandArgs {

        public MainWindow MainWindow { get; }

        public KUKARobot Robot1  { get; }

        public KUKARobot Robot2 { get; }

        public OptiTrackSystem Optitrack { get; }

        public string[] UserArgs { get; }

        public CommandArgs(MainWindow mainWindow, KUKARobot robot1, KUKARobot robot2, OptiTrackSystem optiTrack, string[] userArgs) {
            MainWindow = mainWindow;
            Robot1 = robot1;
            Robot2 = robot2;
            Optitrack = optiTrack;
            UserArgs = userArgs;
        }

    }
}
