using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PingPong.RSI {
    public class RSIAdapter {

        readonly IPEndPoint remoteEndPoint;
        readonly UdpClient client;

        public RSIAdapter(string remoteIp, int port) {
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteIp), port);
            client = new UdpClient(remoteEndPoint);
        }

        public async Task<string> ReceiveAsync() {
             UdpReceiveResult result = await client.ReceiveAsync();
             byte[] bytes = result.Buffer;

             return Encoding.ASCII.GetString(bytes, 0, bytes.Length);
        }

        public int Send(string data) {
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            return client.Send(bytes, bytes.Length, remoteEndPoint);
        }

        public void Close() {
            client.Close();
        }
 
    }
}