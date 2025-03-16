using System.Net.Sockets;
using System.Net;
using System.Text;

namespace AttorneyBotV2
{
    public class GBAEmuConnection
    {
        Socket socket;
        public GBAEmuConnection(string addr="127.0.0.1", int port=8888) 
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAdd = System.Net.IPAddress.Parse(addr);
            IPEndPoint remoteEP = new IPEndPoint(ipAdd, port);
            socket.Connect(remoteEP);
        }
        public void SendKey(string key)
        {
            socket.Send(Encoding.ASCII.GetBytes($"{key}"));
        }
        public void Dispose()
        {
            socket.Disconnect(false);
            socket.Close();
        }


    }
}
