using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PingPong.Devices {
    public class RSIAdapter {

        private readonly UdpClient client;

        private IPEndPoint remoteEndPoint;

        public RSIAdapter(int port) {
            client = new UdpClient(new IPEndPoint(IPAddress.Any, port));
        }

        /// <summary>
        /// Establish connection with KUKA robot, set remote endpoint
        /// </summary>
        /// <returns>First received frame</returns>
        public async Task<InputFrame> Initialize() {
            UdpReceiveResult result = await client.ReceiveAsync();
            remoteEndPoint = result.RemoteEndPoint;
            byte[] receivedBytes = result.Buffer;

            return new InputFrame(Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length));
        }

        public async Task<InputFrame> ReceiveDataAsync() {
             UdpReceiveResult result = await client.ReceiveAsync();
             byte[] receivedBytes = result.Buffer;

             return new InputFrame(Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length));
        }

        public void SendData(OutputFrame data) {
            byte[] bytes = Encoding.ASCII.GetBytes(data.ToString());
           
            int bytesSent = client.Send(bytes, bytes.Length, remoteEndPoint);

            Console.WriteLine($"Sent ({bytesSent}): {data}");
        }

        public void CloseConnection() {
            client.Close();
        }
 
    }
}