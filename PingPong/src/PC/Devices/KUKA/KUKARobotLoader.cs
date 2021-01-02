using Newtonsoft.Json.Linq;
using PingPong.Maths;

namespace PingPong.KUKA {
    static class KUKARobotLoader {

        public static KUKARobot Load(string jsonData) { //TODO: jak bedzie trzeba to dorobic metode ze sciezka do pliku
            var data = JObject.Parse(jsonData);

            int port = (int) data["port"];
            RobotLimits limits = ParseLimits(data["limits"]);
            Transformation optiTrackTransformation = ParseTransformation(data["transformation"]);

            KUKARobot robot = new KUKARobot(port, limits) {
                OptiTrackTransformation = optiTrackTransformation
            };

            return robot;
        }

        private static RobotLimits ParseLimits(JToken limitsToken) {
            //TODO: sparsowanie limitow
            return null;
        }

        private static Transformation ParseTransformation(JToken transformationToken) {
            //TODO: sparsowanie transformacji
            return null;
        }

    }
}
