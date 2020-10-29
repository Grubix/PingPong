using PingPong.KUKA;
using PingPong.Maths;
using System.Collections.Generic;

namespace PingPong.OptiTrack {
    class BallData {

        private readonly Dictionary<KUKARobot, Transformation> transformations;

        public void Update(InputFrame receivedFrame) {
            //TODO: odpalane w window.cs po otrzymaniu ramki z optitracka
            //TODO: apdejt predkosci, przyspieszenie, predykcja nastepnej pozycji etc.
        }

        //TODO: potem przykladowo:
        //public Vector<double> GetPosition(KUKARobot robot) {
        //    return transformations[robot].ConvertPoint([np. aktualna pozycja pileczki w ukladzie optitracka]);
        //}

    }
}
