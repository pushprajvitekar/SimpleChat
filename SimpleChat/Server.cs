using System.Net;
using System.Net.Sockets;
using System.Text;
using ZTSockets = ZeroTier.Sockets;
using ZTSocket = ZeroTier.Sockets.Socket;

namespace SimpleChat
{
    public class Server
    {
        public event MessageEventHandler OnMessageSending;
        public event MessageEventHandler OnError;
        public Server(IPAddress localIpAddress, int portNumber)
        {

            LocalIpAddress = localIpAddress;
            PortNumber = portNumber;
            LocalEndPoint = new IPEndPoint(LocalIpAddress, PortNumber);
        }

        public IPEndPoint LocalEndPoint { get; }
        public IPAddress LocalIpAddress { get; }
        public int PortNumber { get; }
        ZTSocket listener;

        bool runLoop = true;
        public void Start()
        {
            listener =
               new ZTSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and
            // listen for incoming connections.

            try
            {
                listener.Bind(LocalEndPoint);
                listener.Listen(10);
                while (runLoop)
                {
                    var xclient = listener.Accept();
                    HandleClient(xclient);
                }

            }
            catch (ZTSockets.SocketException e)
            {
                OnError?.Invoke(new MessageEventArgs($"Error: {e.Message}, Service Error Code: {e.ServiceErrorCode}, Socket Error Code: {e.SocketErrorCode} ", LocalIpAddress.ToString()));
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new MessageEventArgs($"Error: {ex.Message}", LocalIpAddress.ToString()));
            }
        }
        public void Stop()
        {
            if (listener != null)
            {
                runLoop = false;
                Thread.Sleep(1000);
                listener.Shutdown(SocketShutdown.Both);
                listener.Close();
            }
        }
        private void HandleClient(ZTSocket acceptedClient)
        {
            var clientEndpoint = (IPEndPoint)acceptedClient.RemoteEndPoint;
            var clientAddress = IPAddress.Parse(clientEndpoint.Address.ToString());
            string data = string.Empty;
            var ackMessage = "<|ACK|>:EOM";
            try
            {
                try
                {
                    data = acceptedClient.ReceiveMessage();
                    byte[] msg = Encoding.ASCII.GetBytes(ackMessage);
                    acceptedClient.Send(msg);
                }
                catch (ZTSockets.SocketException e)
                {
                    OnError?.Invoke(new MessageEventArgs($"Error: {e.Message}, Service Error Code: {e.ServiceErrorCode}, Socket Error Code: {e.SocketErrorCode} ", clientAddress.ToString()));
                }
                if (!string.IsNullOrEmpty(data))
                {

                    OnMessageSending?.Invoke(new MessageEventArgs(data, clientAddress.ToString()));
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new MessageEventArgs($"Error: {ex.Message}", clientAddress.ToString()));
            }
        }


    }
}
