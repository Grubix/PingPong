using MathNet.Numerics.LinearAlgebra;
using NatNetML;

namespace PingPong.OptiTrack {
    public class InputFrame {

        public Vector<double> Position { get; } = Vector<double>.Build.Dense(3); //TODO: do wywalenia

        public InputFrame(FrameOfMocapData data) {
            //TODO: Doc do ramki z optitracka https://v22.wiki.optitrack.com/index.php?title=NatNet:_Data_Types
            //TODO: Sensowne dane z ramki z optitracka
        }

    }
}
