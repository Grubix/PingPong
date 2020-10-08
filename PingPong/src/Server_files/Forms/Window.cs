using PingPong.Tasks;
using PingPong.Devices;
using System.Windows.Forms;
using System;

namespace PingPong {

    public partial class Window : Form {

        private readonly Server server;

        private readonly KUKARobot robot1;

        //private readonly KUKARobot robot1;

        private readonly OptiTrack optiTrack;

        private readonly ManualMode task;

        public Window() {
            InitializeComponent();

            task = new ManualMode();
            robot1 = new KUKARobot(8081);
            optiTrack = new OptiTrack(OptiTrack.ConnetionType.Multicast);

            robot1.OnFrameReceived += inputFrame => {
                Console.WriteLine($"Received: {inputFrame}");
            };

            robot1.OnFrameSent += outputFrame => {
                Console.WriteLine($"Sent: {outputFrame}\n");
            };

            server = new Server(robot1, optiTrack, task);
            server.Start();

            incXBtn.Click += (s, e) => task.TargetPosition.X++;
            decXBtn.Click += (s, e) => task.TargetPosition.X--;

            incYBtn.Click += (s, e) => task.TargetPosition.Y++;
            decYBtn.Click += (s, e) => task.TargetPosition.Y--;

            incZBtn.Click += (s, e) => task.TargetPosition.Z++;
            decZBtn.Click += (s, e) => task.TargetPosition.Z--;

            incABtn.Click += (s, e) => task.TargetPosition.A++;
            decABtn.Click += (s, e) => task.TargetPosition.A--;

            incBBtn.Click += (s, e) => task.TargetPosition.B++;
            decBBtn.Click += (s, e) => task.TargetPosition.B--;

            incCBtn.Click += (s, e) => task.TargetPosition.C++;
            decCBtn.Click += (s, e) => task.TargetPosition.C--;
        }

    }

}
