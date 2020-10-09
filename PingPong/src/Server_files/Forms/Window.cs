using PingPong.Tasks;
using PingPong.Devices.KUKA;
using PingPong.Devices.OptiTrack;
using System.Windows.Forms;
using System;

namespace PingPong {

    public partial class Window : Form {

        private readonly OptiTrackSystem optiTrack;

        private readonly KUKARobot robot1;

        private readonly KUKARobot robot2;

        public Window() {
            InitializeComponent();

            optiTrack = new OptiTrackSystem();
            robot1 = new KUKARobot(8081);
            robot2 = null; //TODO:

            optiTrack.OnFrameReceived += frameReceived => Console.WriteLine("Optitrack frame received");
            robot1.OnFrameReceived += frameReceived => Console.WriteLine($"\nKUKA1::Received: {frameReceived}");
            robot1.OnFrameSent += frameSent => Console.WriteLine($"KUKA1::Sent: {frameSent}\n");

            incXBtn.Click += (s, e) => robot1.TargetPosition.X++;
            decXBtn.Click += (s, e) => robot1.TargetPosition.X--;

            incYBtn.Click += (s, e) => robot1.TargetPosition.Y++;
            decYBtn.Click += (s, e) => robot1.TargetPosition.Y--;

            incZBtn.Click += (s, e) => robot1.TargetPosition.Z++;
            decZBtn.Click += (s, e) => robot1.TargetPosition.Z--;

            incABtn.Click += (s, e) => robot1.TargetPosition.A++;
            decABtn.Click += (s, e) => robot1.TargetPosition.A--;

            incBBtn.Click += (s, e) => robot1.TargetPosition.B++;
            decBBtn.Click += (s, e) => robot1.TargetPosition.B--;

            incCBtn.Click += (s, e) => robot1.TargetPosition.C++;
            decCBtn.Click += (s, e) => robot1.TargetPosition.C--;
        }

    }

}
