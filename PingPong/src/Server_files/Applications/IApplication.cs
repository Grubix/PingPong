using PingPong.OptiTrack;

namespace PingPong.Applications {
    interface IApplication {

        void Start();

        void Stop();

        void ProcessData(BallData ballData);

    }
}
