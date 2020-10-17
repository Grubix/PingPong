using System;
using System.Collections.Generic;
using System.Threading;
using MathNet.Numerics.LinearAlgebra;
using NatNetML;
using PingPong.KUKA;

namespace PingPong.OptiTrack {
    class OptiTrackSystem : IDevice {

        public enum ConnetionType : int {
            Multicast = 0,
            Unicast = 1
        }

        private bool isInitialized = false;

        private readonly NatNetClientML natNetClient;

        private readonly ServerDescription serverDescription;

        public event InitializedEventHandler Initialized;

        public event FrameReceivedEventHandler FrameReceived;

        public delegate void InitializedEventHandler();

        public delegate void FrameReceivedEventHandler(InputFrame receivedFrame);

        public OptiTrackSystem(ConnetionType connetionType = ConnetionType.Multicast) {
            natNetClient = new NatNetClientML((int) connetionType);
            serverDescription = new ServerDescription();
        }

        public void Initialize() {
            if (isInitialized) {
                return;
            }

            int status = natNetClient.Initialize("127.0.0.1", "127.0.0.1");

            if (status != 0) {
                throw new Exception("Optitrack initialization failed. Is Motive application running?");
            }

            status = natNetClient.GetServerDescription(serverDescription);

            if (status != 0) {
                throw new Exception("Optitrack connection failed. Is Motive application running?");
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

        private List<E6POS> GetCalibrationPoints(E6POS startPosition, E6POS endPosition, int iterations) {
            List<E6POS> points = new List<E6POS>();

            E6POS deltaPosition = new E6POS(
                (endPosition.X - startPosition.X) / iterations,
                (endPosition.Y - startPosition.Y) / iterations,
                (endPosition.Z - startPosition.Z) / iterations,
                (endPosition.A - startPosition.A) / iterations,
                (endPosition.B - startPosition.B) / iterations,
                (endPosition.C - startPosition.C) / iterations
            );

            for (int i = 0; i <= iterations; i++) {
                points.Add(startPosition + new E6POS(
                    deltaPosition.X * i,
                    deltaPosition.Y * i,
                    deltaPosition.Z * i,
                    deltaPosition.A * i,
                    deltaPosition.B * i,
                    deltaPosition.C * i
                ));
            }

            return points;
        }

        //TODO: najlepiej bedzie przeniesc cala ta metode do KUKARobot

        public void Calibrate(KUKARobot robot, E6POS startPosition, E6POS endPosition, int iterations) {
            if (!isInitialized || !robot.IsInitialized()) {
                throw new Exception("Optitrack and KUKA robot must be initialized");
            }

            var calibrationPoints = GetCalibrationPoints(startPosition, endPosition, iterations);
            var KUKARobotPoints = new List<Vector<double>>();
            var optiTrackPoints = new List<Vector<double>>();

            int samplesPerPoint = 200;

            for (int i = 0; i < calibrationPoints.Count; i++) {
                robot.TargetPosition = calibrationPoints[i]; //TODO: ustawienie czasu przejazdu, trzeba czekac az bedzie gotowy generator trajektorii

                // Wait for robot to reach the target position (current calibration point)
                while (robot.CurrentPosition != robot.TargetPosition) {
                    Thread.Sleep(300);
                }

                int currentSample = 0;
                var optiTrackPoint = Vector<double>.Build.Dense(3);

                void ProcessFrame(InputFrame receivedFrame) {
                    //optitrackPoint += ściagniecie danych z receivedFrame
                    //TODO: zobaczyc co dostajemy od optitracka

                    currentSample++;
                }

                FrameReceived += ProcessFrame;

                // Wait for { samplesPerPoint } received optitrack frames (samples)
                while (currentSample != samplesPerPoint) {
                    Thread.Sleep(300);
                }

                FrameReceived -= ProcessFrame;

                KUKARobotPoints.Add(calibrationPoints[i].XYZ);
                optiTrackPoints.Add(optiTrackPoint / samplesPerPoint);
            }

            //TODO: gdzie trzymac wyznaczone macierze rotacji i wekt translacji ? w kuce czy w optitracku ?
        }

    }
}
