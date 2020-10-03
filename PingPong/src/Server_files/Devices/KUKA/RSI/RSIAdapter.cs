using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PingPong.Devices {
    public class RSIAdapter {

        private readonly IPEndPoint remoteEndPoint;

        private readonly UdpClient client;

        private RSIAdapter(IPEndPoint remoteEndPoint) {
            this.remoteEndPoint = remoteEndPoint;
            client = new UdpClient(remoteEndPoint);
        }

        public RSIAdapter(string remoteIp, int port) : 
            this(new IPEndPoint(IPAddress.Parse(remoteIp), port)) {
        }

        public RSIAdapter(int port) : 
            this(new IPEndPoint(IPAddress.Any, port)) {
        }

        //TODO: mozna jakos uzyc clent.BeginReceive()
        public async Task<InputFrame> ReceiveData() {
             UdpReceiveResult result = await client.ReceiveAsync();
             byte[] receivedBytes = result.Buffer;

             return new InputFrame(Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length));
        }

        public void SendData(OutputFrame data) {
            byte[] bytes = Encoding.ASCII.GetBytes(data.ToString());
            client.Send(bytes, bytes.Length, remoteEndPoint);
        }

        public void CloseConnection() {
            client.Close();
        }
 
    }
}