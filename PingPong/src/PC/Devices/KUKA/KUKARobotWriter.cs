using System.IO;
using System.Text.RegularExpressions;

namespace PingPong.KUKA {

    static class KUKARobotWriter {

        public static void Save(KUKARobot robot, string destPath) {
            double[,] transformation;

            if (robot.OptiTrackTransformation != null) {
                var matrix = robot.OptiTrackTransformation.Matrix;
                transformation = new double[,] {
                    { matrix[0,0], matrix[0,1], matrix[0,2], matrix[0,3] },
                    { matrix[1,0], matrix[1,1], matrix[1,2], matrix[1,3] },
                    { matrix[2,0], matrix[2,1], matrix[2,2], matrix[2,3] },
                    { matrix[3,0], matrix[3,1], matrix[3,2], matrix[3,3] }
                };
            } else {
                transformation = new double[,] {
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 }
                };
            }

            (double X, double Y, double Z) wp0 = robot.Limits.LowerWorkspacePoint;
            (double X, double Y, double Z) wp1 = robot.Limits.UpperWorkspacePoint;

            string jsonString = 
            $@"{{
                ""port"": {robot.Port},
                ""limits"": {{
                    ""lowerWorkspacePoint"": [{wp0.X}, {wp0.Y}, {wp0.Z}],
                    ""upperWorkspacePoint"": [{wp1.X}, {wp1.Y}, {wp1.Z}],
                    ""A1"": [{robot.Limits.A1AxisLimit.Min}, {robot.Limits.A1AxisLimit.Max}],
                    ""A2"": [{robot.Limits.A2AxisLimit.Min}, {robot.Limits.A2AxisLimit.Max}],
                    ""A3"": [{robot.Limits.A3AxisLimit.Min}, {robot.Limits.A3AxisLimit.Max}],
                    ""A4"": [{robot.Limits.A4AxisLimit.Min}, {robot.Limits.A4AxisLimit.Max}],
                    ""A5"": [{robot.Limits.A5AxisLimit.Min}, {robot.Limits.A5AxisLimit.Max}],
                    ""A6"": [{robot.Limits.A6AxisLimit.Min}, {robot.Limits.A6AxisLimit.Max}],
                    ""maxCorrection"": {{
                        ""xyz"": {robot.Limits.CorrectionLimit.XYZ},
                        ""abc"": {robot.Limits.CorrectionLimit.ABC}
                    }}
                }},
                ""transformation"": [
                    [{transformation[0, 0]}, {transformation[0, 1]}, {transformation[0, 2]}, {transformation[0, 3]}],
                    [{transformation[1, 0]}, {transformation[1, 1]}, {transformation[1, 2]}, {transformation[1, 3]}],
                    [{transformation[2, 0]}, {transformation[2, 1]}, {transformation[2, 2]}, {transformation[2, 3]}],
                    [{transformation[3, 0]}, {transformation[3, 1]}, {transformation[3, 2]}, {transformation[3, 3]}]
                ]
            }}";

            //TODO: C# ogolnie jest spoko, ale to jest jakies uposledzone i nwm jak to zrobic inaczej ¯\_(ツ)_/¯
            File.WriteAllText(destPath, Regex.Replace(jsonString, @"\n( {4}){3}", "\n"));
        }

    }
}
