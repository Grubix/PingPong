using MathNet.Numerics.LinearAlgebra;
using NatNetML;

namespace PingPong.OptiTrack {
    public class InputFrame {

        public Vector<double> Position { get; }

        public InputFrame(FrameOfMocapData data) {
            //TODO: Doc do ramki z optitracka https://v22.wiki.optitrack.com/index.php?title=NatNet:_Data_Types
            //TODO: Sensowne dane z ramki z optitracka

            Position = Vector<double>.Build.DenseOfArray(new double[] {
                data.OtherMarkers[0].x * 1000.0,
                data.OtherMarkers[0].y * 1000.0,
                data.OtherMarkers[0].z * 1000.0
            });
        }

    }
}
