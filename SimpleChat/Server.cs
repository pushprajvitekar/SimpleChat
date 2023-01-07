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
        private readonly ZTSocket listener;
        public Server(IPAddress localIpAddress, int portNumber, string nodeId)
        {

            LocalIpAddress = localIpAddress;
            PortNumber = portNumber;
            NodeId = nodeId;
            LocalEndPoint = new IPEndPoint(LocalIpAddress, PortNumber);
            listener = new ZTSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public IPEndPoint LocalEndPoint { get; }
        public IPAddress LocalIpAddress { get; }
        public int PortNumber { get; }
        public string NodeId { get; }



        bool runLoop = true;
        public void Start()
        {


            // Bind the socket to the local endpoint and
            // listen for incoming connections.

            try
            {
                listener.Bind(LocalEndPoint);
                listener.Listen(100);
                while (runLoop)
                {
                    var xclient = listener.Accept();
                    var task = Task.Factory.StartNew(() => HandleClient(xclient));
                    task.Wait();
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
           // var ackMessage = "<|ACK|>:EOM";
            var ackMesPacket = new MessagePacket() { MessageTypeIdentifier = MessageType.Ack, ChatName = NodeId };
            var acKMsg = ackMesPacket.GetDataStream();
            var packet = new MessagePacket();
            try
            {
                try
                {
                     packet = acceptedClient.ReceiveMessagePacket();
                    acceptedClient.Send(acKMsg, 0, acKMsg.Length, SocketFlags.None);
                    acceptedClient.Shutdown(SocketShutdown.Both);
                    acceptedClient.Close();
                }
                catch (ZTSockets.SocketException e)
                {
                    OnError?.Invoke(new MessageEventArgs($"Error: {e.Message}, Service Error Code: {e.ServiceErrorCode}, Socket Error Code: {e.SocketErrorCode} ", clientAddress.ToString()));
                }
                if (/*!string.IsNullOrEmpty(data)*/ packet.MessageTypeIdentifier!= MessageType.Null)
                {

                    OnMessageSending?.Invoke(new MessageEventArgs(packet.ChatMessage, $"{packet.ChatName}@{clientAddress}"));
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new MessageEventArgs($"Error: {ex.Message}", clientAddress.ToString()));
            }
        }


    }
}
