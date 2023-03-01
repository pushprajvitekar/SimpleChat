using chatlibzt;
using chatlibzt.Events;
using System.Net;
using System.Net.Sockets;
using ZTSocket = ZeroTier.Sockets.Socket;
using ZTSockets = ZeroTier.Sockets;
namespace ConsoleClient
{
    internal class Client
    {
        public event ChatMessageEventHandler MessageReceived;
        public event ChatAppErrorEventHandler ClientError;
        public event ZTSocketErrorEventHandler ClientSocketError;
        ZTSocket _sender;
        public Client(IPAddress remoteIpAddress, int remotePortNumber, string nodeId, string userName = null)
        {

            RemoteIpAddress = remoteIpAddress;
            PortNumber = remotePortNumber;
            RemoteEndPoint = new IPEndPoint(RemoteIpAddress, PortNumber);
            // Create a TCP/IP  socket.
            _sender = new ZTSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //_sender.SendTimeout = 10;
            //_sender.ReceiveTimeout = 10;
            //_sender.Blocking = false;
            NodeId = nodeId;
            UserName = userName;
        }
        public string NodeId { get; set; }
        public string UserName { get; }
        public IPEndPoint RemoteEndPoint { get; }
        public IPAddress RemoteIpAddress { get; }
        public int PortNumber { get; }
        public bool Connect()
        {
            _sender ??= new ZTSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                if (!_sender.Connected)
                    _sender.Connect(RemoteEndPoint);
            }
            catch (Exception e)
            {
                ClientError?.Invoke(new ChatAppErrorEventArgs($"Error: {e.Message}", RemoteIpAddress));
            }
            return _sender.Connected;
        }
        public void Disconnect()
        {
            if (_sender != null)
            {
                try
                {
                    // Release the socket.
                    _sender.Shutdown(SocketShutdown.Both);
                    _sender.Close();
                }
                catch
                {
                }

            }
        }
        public void Send(string message, MessageType messageType = MessageType.Message)
        {
            // Connect to a remote device.
            try
            {
                // Initialise a packet object to store the data to be sent
                MessagePacket sendData = new MessagePacket
                {
                    ChatName = UserName ?? NodeId,
                    ChatMessage = message,
                    MessageTypeIdentifier = messageType
                };

                // Get packet as byte array
                byte[] byteData = sendData.GetDataStream();

                // var socketFlag = messageType == MessageType.Message ? SocketFlags.None : SocketFlags.Broadcast;
                int bytesSent = _sender.Send(byteData, 0, byteData.Length, SocketFlags.None);

                if (bytesSent > 0)
                {
                    var response = _sender.ReceiveMessagePacket();
                    if (response.MessageTypeIdentifier == MessageType.Ack)
                    {
                        MessageReceived?.Invoke(new ChatMessageEventArgs($"Ack received", RemoteIpAddress, "system"));
                    }
                }
                else
                {
                    MessageReceived?.Invoke(new ChatMessageEventArgs($"Ack not received", RemoteIpAddress, "system"));
                }

            }
            catch (ArgumentNullException ane)
            {
                ClientError?.Invoke(new ChatAppErrorEventArgs($"Error: {ane.Message}", RemoteIpAddress));
            }
            catch (ZTSockets.SocketException e)
            {
                ClientSocketError?.Invoke(new ZTSocketErrorEventArgs($"Error: {e.Message}", RemoteIpAddress, e.ServiceErrorCode, e.SocketErrorCode));
            }
            catch (Exception ex)
            {
                ClientError?.Invoke(new ChatAppErrorEventArgs($"Error: {ex.Message}", RemoteIpAddress));
            }
        }
    }
}
