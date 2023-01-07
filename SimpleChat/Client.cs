using System.Net;
using System.Net.Sockets;
using System.Text;
using ZTSocket = ZeroTier.Sockets.Socket;
using ZTSockets = ZeroTier.Sockets;
namespace SimpleChat
{
    public class Client
    {
        public event MessageEventHandler OnMessageSending;
        public event MessageEventHandler OnError;
        ZTSocket _sender;
        public Client(IPAddress remoteIpAddress, int remotePortNumber)
        {

            RemoteIpAddress = remoteIpAddress;
            PortNumber = remotePortNumber;
            RemoteEndPoint = new IPEndPoint(RemoteIpAddress, PortNumber);
            // Create a TCP/IP  socket.
            _sender = new ZTSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public IPEndPoint RemoteEndPoint { get; }
        public IPAddress RemoteIpAddress { get; }
        public int PortNumber { get; }
        public bool Connect()
        {
            if (_sender == null)
            {
                _sender = new ZTSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            try
            {
                if (!_sender.Connected)
                    _sender.Connect(RemoteEndPoint);
            }
            catch (Exception e)
            {
                OnError?.Invoke(new MessageEventArgs($"Error: {e.Message}", RemoteIpAddress.ToString()));
            }
            return _sender.Connected;
        }
        public void Disconnect()
        {
            if (_sender != null)
            {
                // Release the socket.
                _sender.Shutdown(SocketShutdown.Both);
                _sender.Close();
                _sender = null;
            }
        }
        public void Send(string message)
        {
            // Connect to a remote device.
            try
            {
                Connect();
                var enc = new ASCIIEncoding();
                var msg = enc.GetBytes($"{message}:EOM");
                int bytesSent = _sender.Send(msg, 0, msg.Length, SocketFlags.None);

                if (bytesSent > 0)
                {
                    var response = _sender.ReceiveMessage();
                    if (response == "<|ACK|>")
                    {
                        OnMessageSending?.Invoke(new MessageEventArgs($"Ack received", RemoteEndPoint.ToString()));
                    }
                }
                else
                {
                    OnMessageSending?.Invoke(new MessageEventArgs($"Ack not received", RemoteEndPoint.ToString()));
                }
                Disconnect();
            }
            catch (ArgumentNullException ane)
            {
                OnError?.Invoke(new MessageEventArgs($"Error: {ane.Message}", RemoteIpAddress.ToString()));
            }
            catch (ZTSockets.SocketException e)
            {
                OnError?.Invoke(new MessageEventArgs($"Error: {e.Message}, Service Error Code: {e.ServiceErrorCode}, Socket Error Code: {e.SocketErrorCode} ", RemoteIpAddress.ToString()));
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new MessageEventArgs($"Error: {ex.Message}", RemoteIpAddress.ToString()));
            }

        }
    }
}
