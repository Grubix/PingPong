using System;
using System.Threading;
using System.Threading.Tasks;

namespace Resilio_Project {

    public class RSIServer {

        CancellationTokenSource ts;
        UdpListener udpListener;
        bool serverRunning = false;
        long IPOC;

        public RSIServer() {
        }

        public void StartServer() {
            if (serverRunning) {
                // Jakieś tam resetowanie timerów
            } else {
                udpListener = new UdpListener();
                ts = new CancellationTokenSource();
                serverRunning = true;
                mainLoop();
            }
        }

        public void StopServer() {
            serverRunning = false;
            ts.Cancel();
            udpListener.CloseSocket();
        }

        private void mainLoop() {
            Task.Factory.StartNew(async () => {
                while (serverRunning) {
                    var received = await udpListener.Receive();
                    ParseRequest(received.Message);
                    string response = ComposeResponse();
                    udpListener.Reply(response, received.Sender);
                }
            }, ts.Token);
        }

        private void ParseRequest(string data) {
            RequestString.setRequest(data);
            IPOC = RequestString.getIPOC();
        }

        private string ComposeResponse() {
            ResponseString.UpdateIPOC(IPOC + 4);
            return ResponseString.getString();
        }
    }
}