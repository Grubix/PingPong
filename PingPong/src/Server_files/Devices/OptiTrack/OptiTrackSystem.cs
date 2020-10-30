using MathNet.Numerics.LinearAlgebra;
using NatNetML;
using System;
using System.Threading;

namespace PingPong.OptiTrack {
    public class OptiTrackSystem : IDevice {

        private bool isInitialized = false;

        private readonly NatNetClientML natNetClient;

        private readonly ServerDescription serverDescription;

        public event InitializedEventHandler Initialized;

        public event FrameReceivedEventHandler FrameReceived;

        public delegate void InitializedEventHandler();

        public delegate void FrameReceivedEventHandler(InputFrame receivedFrame);

        public OptiTrackSystem(int connectionType = 0) {
            natNetClient = new NatNetClientML(connectionType);
            serverDescription = new ServerDescription();
        }

        public void Initialize() {
            if (isInitialized) {
                return;
            }

            int status = natNetClient.Initialize("127.0.0.1", "127.0.0.1");

            if (status != 0) {
                throw new InvalidOperationException("OptiTrack system initialization failed. Is Motive application running?");
            }

            status = natNetClient.GetServerDescription(serverDescription);

            if (status != 0) {
                throw new InvalidOperationException("Connection failed. Is Motive application running?");
            }

            natNetClient.OnFrameReady += (data, client) => {
                FrameReceived?.Invoke(new InputFrame(data));
            };

            isInitialized = true;
            Initialized?.Invoke();
        }

        public bool IsInitialized() {
            return isInitialized;
        }

        public void Uninitialize() {
            isInitialized = false;
            natNetClient.Uninitialize();
        }

        public Vector<double> GetAveragePosition(int samples) {
            if (!isInitialized) {
                throw new InvalidOperationException("OptiTrack system is not initialized");
            }

            ManualResetEvent getSamplesEvent = new ManualResetEvent(false);
            Vector<double> position = Vector<double>.Build.Dense(3);

            int currentSample = 0;

            void checkSample(InputFrame inputFrame) {
                position += inputFrame.Position;
                currentSample++;

                if (currentSample >= samples) {
                    FrameReceived -= checkSample;
                    getSamplesEvent.Set();
                }
            }

            FrameReceived += checkSample;
            getSamplesEvent.WaitOne();

            return position / samples;
        }

    }
}
