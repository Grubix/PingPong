using PingPong.Utils;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PingPong.Devices.KUKA {
    public class RSIAdapter {

        private bool isConnected;

        private readonly Timer timer;

        private readonly UdpClient client;

        private IPEndPoint remoteEndPoint;

        /// <summary>
        /// Time in seconds between two last received frames
        /// </summary>
        public double TimeDelta { get; private set; }

        public RSIAdapter(int port) {
            timer = new Timer();
            client = new UdpClient(new IPEndPoint(IPAddress.Any, port));
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
           
            timer.Start();

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

            timer.Stop();
            TimeDelta = timer.Seconds;
            timer.Start();

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