using PingPong.KUKA;

namespace PingPong.Commands {
    class MoveToCommand : ICommand {

        public string Name => "move";

        public string Usage => "move <robot> (<x>,<y>,<z>[,<a>,<b>,<c>]) <movementDuration>";

        public string Description => "Moves robot to specified position";

        public void Execute(CommandArgs args) {
            if (args.UserArgs.Length != 3) {
                args.CommandLine.Error($"3 arguments expected, get {args.UserArgs.Length}");
                return;
            }

            KUKARobot robot;

            switch (args.UserArgs[0]) {
                case "robot1":
                    robot = args.Robot1;
                    break;
                case "robot2":
                    robot = args.Robot2;
                    break;
                default:
                    args.CommandLine.Error($"'robot1' or 'robot2' expected, get '{args.UserArgs[0]}'");
                    return;
            }

            if (robot == null || !robot.IsInitialized()) {
                args.CommandLine.Error($"Selected robot is not initialized");
                return;
            }

            string[] positionSplit = args.UserArgs[1].Trim().Split(',');
            RobotVector targetPosition;

            if (positionSplit.Length == 3) {
                if (double.TryParse(positionSplit[0], out double x) &&
                    double.TryParse(positionSplit[1], out double y) &&
                    double.TryParse(positionSplit[2], out double z)) {
                    targetPosition = new RobotVector(x, y, z, robot.Position.ABC);
                } else {
                    args.CommandLine.Error($"Invalid XYZ position");
                    return;
                }
            } else if (positionSplit.Length == 6) {
                if (double.TryParse(positionSplit[0], out double x) &&
                    double.TryParse(positionSplit[1], out double y) &&
                    double.TryParse(positionSplit[2], out double z) &&
                    double.TryParse(positionSplit[3], out double a) &&
                    double.TryParse(positionSplit[4], out double b) &&
                    double.TryParse(positionSplit[5], out double c)) {
                    targetPosition = new RobotVector(x, y, z, a, b, c);
                } else {
                    args.CommandLine.Error($"Invalid XYZ position or ABC angles");
                    return;
                }
            } else {
                args.CommandLine.Error($"(x, y, z) or (x, y, z, a, b, c) position expected, get ({args.UserArgs[1]})");
                return;
            }

            if (double.TryParse(args.UserArgs[2], out double movementDuration)) {
                robot.MoveTo(targetPosition, RobotVector.Zero, movementDuration);
            } else {
                args.CommandLine.Error($"Invalid movement duration");
            }
        }

    }
}
