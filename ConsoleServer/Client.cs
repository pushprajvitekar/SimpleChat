using chatlibzt.Events;
using System.Net.Sockets;
using System.Net;
using ZTSockets = ZeroTier.Sockets;
using ZTSocket = ZeroTier.Sockets.Socket;
using chatlibzt;

namespace ConsoleServer
{
    internal class Client
    {
        public event ChatMessageEventHandler MessageReceived;
        public event ChatAppErrorEventHandler ClientError;
        public event ZTSocketErrorEventHandler ClientSocketError;
        public event ChatMessageEventHandler Left;
        public event ChatMessageEventHandler Joined;

        readonly ZTSocket _sender;
        public Client(ZTSocket zTSocket)
        {
            _sender = zTSocket;
            LocalIpAddress = IPAddress.Parse(((IPEndPoint)_sender.LocalEndPoint).Address.ToString());
            LocalPort = ((IPEndPoint)_sender.LocalEndPoint).Port;

            RemoteIpAddress = IPAddress.Parse(((IPEndPoint)_sender.RemoteEndPoint).Address.ToString());
            RemotePort = ((IPEndPoint)_sender.RemoteEndPoint).Port;

        }
        public int RemotePort { get; }
        public IPAddress RemoteIpAddress { get; }
        public IPAddress LocalIpAddress { get; }
        public int LocalPort { get; }

        public string UserId { get; set; }
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
        public void Start()
        {
            while (true)
            {
                string data = string.Empty;
                var packet = new MessagePacket();
                try
                {
                    packet = _sender.ReceiveMessagePacket();
                    if (packet.MessageTypeIdentifier == MessageType.Enter)
                    {
                        Joined?.Invoke(new ChatMessageEventArgs(packet.ChatMessage, LocalIpAddress, $"{packet.ChatName}"));
                    }
                    if (packet.MessageTypeIdentifier == MessageType.Message)
                    {

                        MessageReceived?.Invoke(new ChatMessageEventArgs(packet.ChatMessage, LocalIpAddress, $"{packet.ChatName}"));
                    }
                    if (packet.MessageTypeIdentifier == MessageType.Exit)
                    {
                        Left?.Invoke(new ChatMessageEventArgs(packet.ChatMessage, LocalIpAddress, $"{packet.ChatName}"));
                        break;
                    }
                }

                catch (ZTSockets.SocketException e)
                {
                    ClientSocketError?.Invoke(new ZTSocketErrorEventArgs($"Error: {e.Message}", LocalIpAddress, e.ServiceErrorCode, e.SocketErrorCode));
                    Left?.Invoke(new ChatMessageEventArgs(packet.ChatMessage, LocalIpAddress, $"{packet.ChatName}"));
                    break;
                }
                catch (Exception ex)
                {
                    ClientError?.Invoke(new ChatAppErrorEventArgs($"Error: {ex.Message}", RemoteIpAddress));
                    Left?.Invoke(new ChatMessageEventArgs(packet.ChatMessage, LocalIpAddress, $"{packet.ChatName}"));
                    break;
                }
            }
        }

        public void Send(string message)
        {

            try
            {
                // Initialise a packet object to store the data to be sent
                MessagePacket sendData = new MessagePacket
                {
                    ChatName = UserId,
                    ChatMessage = message,
                    MessageTypeIdentifier = MessageType.Message
                };

                byte[] byteData = sendData.GetDataStream();

                int bytesSent = _sender.Send(byteData, 0, byteData.Length, SocketFlags.None);


            }
           
            catch (ZTSockets.SocketException e)
            {
                ClientSocketError?.Invoke(new ZTSocketErrorEventArgs($"Error: {e.Message}", LocalIpAddress, e.ServiceErrorCode, e.SocketErrorCode));
            }
            catch (Exception ex)
            {
                ClientError?.Invoke(new ChatAppErrorEventArgs($"Error: {ex.Message}", LocalIpAddress));
            }
        }
    }
}
