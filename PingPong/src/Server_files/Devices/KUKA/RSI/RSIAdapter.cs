using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PingPong.KUKA {
    public class RSIAdapter {

        private bool isConnected;

        private readonly UdpClient client;

        private readonly Stopwatch stopwatch; //TODO: zamiast timera na kompie, mozna uzyc znacznika IPOC, kuka ma najprawdopodobniej znacznie dokladniejszy timer, DO SPRAWDZENIA

        private IPEndPoint remoteEndPoint;

        /// <summary>
        /// Time in seconds between two last received frames
        /// </summary>
        public double DeltaTime { get; private set; }

        public RSIAdapter(int port) {
            client = new UdpClient(new IPEndPoint(IPAddress.Any, port));
            stopwatch = new Stopwatch();
        }

        /// <summary>
        /// Connects to robot and returns the first received frame
        /// </summary>
        /// <returns>First received frame</returns>
        public async Task<InputFrame> Connect() {
            if(isConnected) {
                throw new Exception("Connection has been already established");
            }

            UdpReceiveResult result = await client.ReceiveAsync();
            stopwatch.Start();

            remoteEndPoint = result.RemoteEndPoint;
            byte[] receivedBytes = result.Buffer;

            return new InputFrame(Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length));
        }

        /// <summary>
        /// Close connection
        /// </summary>
        public void Disconnect() {
            isConnected = false;
            client.Close();
        }

        /// <summary>
        /// Receives data from the remoteEndPoint asynchronously
        /// </summary>
        /// <returns>Parsed data as InputFrame</returns>
        public async Task<InputFrame> ReceiveDataAsync() {
            UdpReceiveResult result = await client.ReceiveAsync();
            stopwatch.Stop();
            
            DeltaTime = stopwatch.ElapsedMilliseconds / 1000.0;
            
            stopwatch.Reset();
            stopwatch.Start();
            byte[] receivedBytes = result.Buffer;

            return new InputFrame(Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length));
        }

        /// <summary>
        /// Sends data to the remoteEndPoint
        /// </summary>
        /// <param name="data">data to sent</param>
        public void SendData(OutputFrame data) {
            byte[] bytes = Encoding.ASCII.GetBytes(data.ToString());
            client.Send(bytes, bytes.Length, remoteEndPoint);
        }
 
    }
}