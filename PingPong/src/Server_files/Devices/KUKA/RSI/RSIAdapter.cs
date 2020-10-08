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
        /// Connects to robot and returns the first received frame.
        /// </summary>
        /// <returns>First received frame</returns>
        public async Task<InputFrame> Connect() {
            UdpReceiveResult result = await client.ReceiveAsync();
            remoteEndPoint = result.RemoteEndPoint;
            byte[] receivedBytes = result.Buffer;

            return new InputFrame(Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length));
        }

        /// <summary>
        /// Close connection.
        /// </summary>
        public void Disconnect() {
            client.Close();
        }

        public async Task<InputFrame> ReceiveDataAsync() {
             UdpReceiveResult result = await client.ReceiveAsync();
             byte[] receivedBytes = result.Buffer;

             return new InputFrame(Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length));
        }

        public void SendData(OutputFrame data) {
            byte[] bytes = Encoding.ASCII.GetBytes(data.ToString());
            client.Send(bytes, bytes.Length, remoteEndPoint);
        }
 
    }
}