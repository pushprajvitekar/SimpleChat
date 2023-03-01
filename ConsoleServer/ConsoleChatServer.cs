using chatlibzt.Events;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using ZTSocket = ZeroTier.Sockets.Socket;
using ZTSockets = ZeroTier.Sockets;

namespace ConsoleServer
{
    public class ConsoleChatServer
    {
        public event ChatMessageEventHandler OnMessageSending;
        public event ChatAppErrorEventHandler OnError;
        public event ZTSocketErrorEventHandler OnSocketError;
        private readonly ZTSocket listener;
        readonly ConcurrentDictionary<string, Client> _clientList = new ConcurrentDictionary<string, Client>();
        public ConsoleChatServer(IPAddress localIpAddress, int portNumber)
        {

            LocalIpAddress = localIpAddress;
            PortNumber = portNumber;
            LocalEndPoint = new IPEndPoint(LocalIpAddress, PortNumber);
            listener = new ZTSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //listener.SendTimeout = 10;
            //listener.ReceiveTimeout = 10;
            //listener.Blocking = false;

        }

        public IPEndPoint LocalEndPoint { get; }
        public IPAddress LocalIpAddress { get; }
        public int PortNumber { get; }


        bool runLoop = true;
        public void Start()
        {
            try
            {
                listener.Bind(LocalEndPoint);
                listener.Listen(100);
                while (runLoop)
                {
                    var zclient = listener.Accept();
                    var client = new Client(zclient);
                    AddClient(client);
                    client.Start();
                }

            }
            catch (ZTSockets.SocketException e)
            {
                OnSocketError?.Invoke(new ZTSocketErrorEventArgs($"Error: {e.Message}", LocalIpAddress, e.ServiceErrorCode, e.SocketErrorCode));
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new ChatAppErrorEventArgs($"Error: {ex.Message}", LocalIpAddress));
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
        private void BroadcastMessage(string userId, string message)
        {
            lock (this)
            {
                for (int i = 0; i < _clientList.Count; i++)
                {
                    if (_clientList.TryGetValue(userId, out Client client))
                    {
                        try
                        {
                            client.Send(message);
                        }
                        catch (Exception)
                        {
                            RemoveClient(userId);
                        }
                    }
                }
            }
        }


        private void AddClient(Client client)
        {
            lock (this)
            {

                var ip = client.LocalIpAddress.ToString();
                if (!_clientList.ContainsKey(ip))
                {
                    client.ClientError += OnClientError;
                    client.ClientSocketError += OnClientSocketError;
                    client.MessageReceived += OnClientMessageReceived;
                    client.Joined += OnClientJoined;
                    client.Left += OnClientLeft;
                    _clientList.TryAdd(ip, client);
                }
            }
        }
        private void RemoveClient(string clientId)
        {
            lock (this)
            {

                if (_clientList.TryRemove(clientId, out Client client))
                {
                    RemoveClient(client);
                }
            }
        }
        private void RemoveClient(Client client)
        {
            if (client != null)
            {
                client.ClientError -= OnClientError;
                client.ClientSocketError -= OnClientSocketError;
                client.MessageReceived -= OnClientMessageReceived;
                client.Joined -= OnClientJoined;
                client.Left -= OnClientLeft;
                client.Disconnect();
            }
        }
        private void OnClientLeft(ChatMessageEventArgs e)
        {
            RemoveClient(e.NodeId);
        }

        private void OnClientJoined(ChatMessageEventArgs e)
        {
        }

        private void OnClientMessageReceived(ChatMessageEventArgs e)
        {
            BroadcastMessage(e.NodeId, e.Message);
        }

        private void OnClientSocketError(ZTSocketErrorEventArgs e)
        {
        }

        private void OnClientError(ChatAppErrorEventArgs e)
        {
        }
    }
}
