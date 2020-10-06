using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Resilio_Project {

    public struct Received {
        public IPEndPoint Sender;
        public string Message;
    }

    class UdpListener {
        protected UdpClient Client;
        private IPEndPoint _listenOn;

        public UdpListener() : this(new IPEndPoint(IPAddress.Any, 8081)) {
        }

        public UdpListener(IPEndPoint endpoint) {
            _listenOn = endpoint;
            Client = new UdpClient(_listenOn);
        }

        public void CloseSocket() {
            Client.Close();
        }

        public void Reply(string message, IPEndPoint endpoint) {
            var datagram = Encoding.ASCII.GetBytes(message);
            Client.Send(datagram, datagram.Length, endpoint);
        }

        public async Task<Received> Receive() {
            var result = await Client.ReceiveAsync();
            return new Received() {
                Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
                Sender = result.RemoteEndPoint
            };
        }
    }
}
