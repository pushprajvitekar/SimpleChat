using System.Net;
using System.Net.Sockets;
using System.Text;
using ZTSocket = ZeroTier.Sockets.Socket;
using ZTSockets = ZeroTier.Sockets;
namespace SimpleChat
{
    public class Client
    {
        // public event MessageEventHandler OnMessageSending;
        public event MessageEventHandler OnError;
        ZTSocket _sender;
        public Client(IPAddress localIpAddress, int portNumber)
        {

            LocalIpAddress = localIpAddress;
            PortNumber = portNumber;
            LocalEndPoint = new IPEndPoint(LocalIpAddress, PortNumber);
            // Create a TCP/IP  socket.
            _sender = new ZTSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public IPEndPoint LocalEndPoint { get; }
        public IPAddress LocalIpAddress { get; }
        public int PortNumber { get; }
        public bool Connect()
        {
            if (_sender == null)
            {
                _sender = new ZTSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            try
            {
                _sender.Connect(LocalEndPoint);
            }
            catch (Exception e)
            {
                OnError?.Invoke(new MessageEventArgs($"Error: {e.Message}", LocalIpAddress.ToString()));
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
            }
        }
        public void Send(string message)
        {
            // Connect to a remote device.
            try
            {
                try
                {
                    var enc = new ASCIIEncoding();
                    var msg = enc.GetBytes($"{message}:EOM");
                    int bytesSent = _sender.Send(msg, 0, msg.Length, SocketFlags.None);
                    if (bytesSent > 0)
                    {
                        var response = _sender.ReceiveMessage();
                        //if (response == "<|ACK|>")
                        //{
                        //    _sender.Close();
                        //}
                    }

                }
                catch (ArgumentNullException ane)
                {
                    OnError?.Invoke(new MessageEventArgs($"Error: {ane.Message}", LocalIpAddress.ToString()));
                }
                catch (ZTSockets.SocketException e)
                {
                    OnError?.Invoke(new MessageEventArgs($"Error: {e.Message}, Service Error Code: {e.ServiceErrorCode}, Socket Error Code: {e.SocketErrorCode} ", LocalIpAddress.ToString()));
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new MessageEventArgs($"Error: {ex.Message}", LocalIpAddress.ToString()));
            }
        }
    }
}
