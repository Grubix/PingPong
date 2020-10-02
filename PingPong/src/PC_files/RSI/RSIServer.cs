using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PingPong.RSI {
    public class RSIServer {

        private bool running = false;
        private readonly RSIAdapter rsiAdapter;
        private readonly CancellationTokenSource tokenSource;

        public RSIServer(string ip, int port) {
            rsiAdapter = new RSIAdapter(ip, port);
            tokenSource = new CancellationTokenSource();
        }

        public void Start() {
            if(!running) {
                running = true;

                Task.Factory.StartNew(async () => {
                    while(running) {
                        string receivedData = await rsiAdapter.ReceiveAsync();
                        Console.WriteLine("Received: " + receivedData);
                        ProcessRequest(receivedData);
                    }
                }, tokenSource.Token);
            }
        }

        public void Stop() {
            running = false;
            tokenSource.Cancel();
            rsiAdapter.Close();
        }

        public void ProcessRequest(string data) {
            InputFrame frame = new InputFrame(data);
        }

        public void Send(OutputFrame data) {
            rsiAdapter.Send(data.ToString());
        }

        public void printAvailableIps() {
            //FIXME do wywalenia / wrzucenia gdzies jako opcja w menu
            IPAddress[] availableIps = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress ip in availableIps) {
                Console.WriteLine(ip);
            }
        }

    }
}
