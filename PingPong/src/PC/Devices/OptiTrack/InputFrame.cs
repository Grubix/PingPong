using MathNet.Numerics.LinearAlgebra;
using NatNetML;

namespace PingPong.OptiTrack {
    public class InputFrame {

        public Vector<double> Position { get; }

        public uint Timestamp { get; }

        public uint FrameDeltaTime { get; }

        public InputFrame(FrameOfMocapData data) {
            //TODO: Doc do ramki z optitracka https://v22.wiki.optitrack.com/index.php?title=NatNet:_Data_Types

            //TODO: data.LabeledMarkers !!!!!!!!!!!

            Position = Vector<double>.Build.DenseOfArray(new double[] {
                data.OtherMarkers[0].x * 1000.0,
                data.OtherMarkers[0].y * 1000.0,
                data.OtherMarkers[0].z * 1000.0
            });

            Timestamp = data.Timecode; //TODO: zobaczyc co to zwraca
            FrameDeltaTime = data.TimecodeSubframe; //TODO: zobaczyc co to zwraca
            //TODO: jezeli te timery nie pykna trzeba uzyc Stopwatcha albo czegos podobnego
        }

    }
}
