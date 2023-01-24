using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZeroTier.Core;

namespace SimpleChat
{
    public partial class frmMain : Form
    {

        ZeroTierNodeManager manager;
        const string networkidstr = "416038decec8db58";
        IPAddress _myIpAddress;
        readonly int _myPort = 50001;
        Server _listener;
        string _myNodeId;
        ConcurrentDictionary<string, Client> _clientList = new ConcurrentDictionary<string, Client>();
        public frmMain()
        {
            InitializeComponent();
        }

        private void Manager_NetworkUpdatedEvent(object sender, NetworkUpdatedEventArgs e)
        {
            if (e.IPAddresses != null && e.IPAddresses.Any())
            {
                var recvdIp = e.IPAddresses.First();
                if (recvdIp != _myIpAddress)
                {
                    _myIpAddress = e.IPAddresses.First();
                    this.Invoke(new MethodInvoker(() => OnNetworkAuthenticated()));
                    _myNodeId = manager.NodeId;
                }
            }
            //if (e.Routes != null && e.Routes.Any())
            //{
            //    foreach (var route in e.Routes)
            //    {
            //        var target = route.Target;
            //        peers.TryAdd(target.ToString(), target);

            //        var via = route.Via;
            //        peers.TryAdd(via.ToString(), via);
            //    }
            //    this.Invoke(new MethodInvoker(() =>
            //    {
            //        lstBoxPeers.Items.Clear();
            //        foreach (var ip in peers.Keys)
            //        {
            //            lstBoxPeers.Items.Add(ip);
            //        }
            //    }));
            //}
        }

        private void Manager_MessageReceivedEvent(object sender, ZeroTierManagerMessageEventArgs e)
        {
            AddItemToMessagesList($"Network Message: {e.Message}");
        }
        private void AddItemToMessagesList(string message)
        {
            this.Invoke(new MethodInvoker(() => lstBoxMessages.Items.Insert(0, message)));
        }
        private void AddItemToChatList(string message)
        {
            this.Invoke(new MethodInvoker(() => lstboxChat.Items.Insert(0, message)));
        }
        private void AddItemToPeerList(string nodeId, IPAddress iPAddress, int port)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                var peer = $"{nodeId}@{iPAddress}";
                if (lstBoxPeers.FindStringExact(nodeId) == ListBox.NoMatches)
                {
                    lstBoxPeers.Items.Add(new Peer { NodeId = nodeId, IpAddress = iPAddress, Port = port });
                    lstboxChat.Items.Insert(0, $"{DateTime.Now.ToShortTimeString()}: {peer} joined the chat.");
                }
            }
            ));
        }

        private void RemoveItemToPeerList(string nodeId, IPAddress iPAddress, int port)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                RemoveClient(nodeId);
                var peer = $"{nodeId}@{iPAddress}";
                var idx = lstBoxPeers.FindStringExact(nodeId);
                if (idx != ListBox.NoMatches)
                {
                    lstBoxPeers.Items.Remove(lstBoxPeers.Items[idx]);
                    lstboxChat.Items.Insert(0, $"{DateTime.Now.ToShortTimeString()}: {peer} left the chat.");
                }
            }
             ));
        }
        private void frmMain_Load(object sender, EventArgs e)
        {

            var port = Convert.ToString(_myPort);
            txtMyPort.Text = port;
            txtFriendPort.Text = port;


        }

        //// Return your own IP
        //private string GetLocalIP()
        //{
        //    IPHostEntry host;
        //    host = Dns.GetHostEntry(Dns.GetHostName());
        //    foreach (IPAddress ip in host.AddressList)
        //    {
        //        if (ip.AddressFamily == AddressFamily.InterNetwork)
        //        {
        //            return ip.ToString();
        //        }
        //    }
        //    return "127.0.0.1";
        //}
        CancellationTokenSource ts = new CancellationTokenSource();
        private void OnNetworkAuthenticated()
        {
            try
            {
                CancellationToken ct = ts.Token;
                txtMyIp.Text
                    = _myIpAddress.ToString();
                var epLocal = new IPEndPoint(IPAddress.Parse(txtMyIp.Text),
                 Convert.ToInt32(txtMyPort.Text));
                if (_listener != null)
                {
                    _listener.OnMessageSending -= Server_OnMessageSending;
                    _listener.OnError -= Server_OnError; ;
                    _listener.OnSocketError -= _listener_OnSocketError;
                    _listener.Stop();
                }
                _listener = new Server(IPAddress.Parse(txtMyIp.Text), Convert.ToInt32(txtMyPort.Text), _myNodeId);
                _listener.OnMessageSending += Server_OnMessageSending;
                _listener.OnError += Server_OnError; ;
                _listener.OnSocketError += _listener_OnSocketError;
                _ = Task.Factory.StartNew(_listener.Start, TaskCreationOptions.LongRunning);

                StartUdpListener();
                _ = RunInBackground(ct);
                // release button to send message
                btnSend.Enabled = true;
                btnInitNetwork.Text = "Connected";
                btnInitNetwork.Enabled = false;
                //btnDisconnectNetwork.Enabled = true;
                txtMessage.Focus();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void _listener_OnSocketError(SocketErrorMessageEventArgs e)
        {
            this.Invoke(new MethodInvoker(() => MessageBox.Show(e.Message, "Server socket error")));
        }

        private async Task RunInBackground(CancellationToken ct)
        {
            var periodicTime = new PeriodicTimer(TimeSpan.FromSeconds(5));
            while (await periodicTime.WaitForNextTickAsync(ct))
            {
                SendAddMeMessage();
            }
        }

        private void Server_OnError(ErrorMessageEventArgs e)
        {
            this.Invoke(new MethodInvoker(() => MessageBox.Show(e.Message, "Server error")));
        }

        private void Server_OnMessageSending(MessageEventArgs e)
        {
            DisplayClientMessage(e.Message, e.SenderName);
        }

        private void DisplayClientMessage(string message, string client)
        {
            try
            {
                AddItemToChatList($"{DateTime.Now.ToShortTimeString()}:[{client}]: {message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                var peer = lstBoxPeers.SelectedItem as Peer;
                if (peer == null)
                {
                    MessageBox.Show("Select any friend to send message");
                    return;
                }
                var remoteAddress = peer.IpAddress;
                var remotePort = peer.Port;
                //var validIp = IPAddress.TryParse(txtFriendIp.Text, out IPAddress? remoteAddress);
                //var validPort = int.TryParse(txtFriendPort.Text, out int remotePort);
                //if (!validIp || !validPort || remoteAddress == null)
                //{
                //    MessageBox.Show("Select any friend to send message");
                //    return;
                //}
                var client = CreateClient(peer.NodeId, remoteAddress, remotePort);

                var message = txtMessage.Text;

                if (!string.IsNullOrEmpty(message))
                {
                    var task = Task.Factory.StartNew(() => client.Send(message));
                    AddItemToChatList($"{DateTime.Now.ToShortTimeString()}:[{_myNodeId}]: {message}");
                    txtMessage.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private Client CreateClient(string nodeId, IPAddress remoteAddress, int remotePort)
        {

            if (!_clientList.TryGetValue(nodeId, out Client client))
            {
                client = new Client(remoteAddress, remotePort, _myNodeId);
                client.OnError += Client_OnError;
                client.OnSocketError += _client_OnSocketError;
                client.OnMessageSending += _client_OnMessageSending;
                _clientList.TryAdd(nodeId, client);
            }
            return client;
        }

        private void _client_OnSocketError(SocketErrorMessageEventArgs e)
        {
            this.Invoke(new MethodInvoker(() => MessageBox.Show(e.Message, "Server socket error")));
        }

        private void _client_OnMessageSending(MessageEventArgs e)
        {
            AddItemToMessagesList($"Client Message: {e.Message}, Sender: {e.Sender}");
        }

        private void Client_OnError(ErrorMessageEventArgs e)
        {
            this.Invoke(new MethodInvoker(() => MessageBox.Show(e.Message, "Client error")));
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            OnDisconnect();
        }

        private void OnDisconnect()
        {
            ts.Cancel();
            if (_listener != null)
            {
                _listener.OnMessageSending -= Server_OnMessageSending;
                _listener.OnError -= Server_OnError;
                _listener.OnSocketError -= _listener_OnSocketError;
                _listener.Stop();
            }
            if (manager != null)
            {
                manager.MessageReceivedEvent -= Manager_MessageReceivedEvent;
                manager.NetworkUpdatedEvent -= Manager_NetworkUpdatedEvent;
            }
            RemoveAllClients();
            if (_udpClientWrapper != null)
            {

                _udpClientWrapper.LeaveMulticastGroup($"{_myNodeId}@{_myIpAddress}:{_myPort}");
                _udpClientWrapper.UdpMessageReceived -= OnUdpMessageReceived;
            }
            manager?.StopZeroTier();
            manager = null;
        }
        private void RemoveAllClients()
        {
            foreach (var nodeid in _clientList.Keys.ToList())
            {
                RemoveClient(nodeid);
            }
        }
        private void RemoveClient(string nodeid)
        {
            if (_clientList.TryRemove(nodeid, out Client client))
            {
                RemoveClient(client);
            }
        }

        private void RemoveClient(Client client)
        {
            if (client != null)
            {
                client.OnError -= Client_OnError;
                client.OnMessageSending -= _client_OnMessageSending;
                client.OnSocketError -= _client_OnSocketError;
                client.Disconnect();
            }
        }

        private void btnInitNetwork_Click(object sender, EventArgs e)
        {
            manager = new ZeroTierNodeManager();
            manager.MessageReceivedEvent += Manager_MessageReceivedEvent;
            manager.NetworkUpdatedEvent += Manager_NetworkUpdatedEvent;
            ulong networkId = (ulong)Int64.Parse(networkidstr, System.Globalization.NumberStyles.HexNumber);
            manager.StartZeroTier(networkId);

        }

        private void lstBoxPeers_SelectedIndexChanged(object sender, EventArgs e)
        {

            var item = lstBoxPeers.SelectedItem as Peer;
            if (item != null)
            {
                txtFriendIp.Text = item.IpAddress.ToString();
                lblFriendNodeId.Text = item.NodeId;
                txtFriendPort.Text = item.Port.ToString();
            }
        }



        /// <summary>
        /// UDP Message received event
        /// </summary>
        void OnUdpMessageReceived(object sender, MulticastUdpClient.UdpMessageReceivedEventArgs e)
        {
            if (e != null && e.MessageType != MessageType.Null)
            {
                var senderip = IPAddress.Parse(e.IPAddress);
                var senderNodeID = e.NodeId;
                if (senderip == _myIpAddress || senderNodeID == _myNodeId) return;
                switch (e.MessageType)
                {

                    case MessageType.Enter:
                        AddItemToPeerList(senderNodeID, senderip, e.Port);
                        break;
                    case MessageType.Exit:
                        RemoveItemToPeerList(senderNodeID, senderip, e.Port);
                        break;
                    default: break;
                }
            }
        }

        MulticastUdpClient _udpClientWrapper;
        const int udpPort = 2222;
        const string udpAddress = "239.0.0.222";
        private void StartUdpListener()
        {
            // Create address objects
            IPAddress multicastIPaddress = IPAddress.Parse(udpAddress);
            IPAddress localIPaddress = IPAddress.Any;

            // Create MulticastUdpClient
            _udpClientWrapper = new MulticastUdpClient(multicastIPaddress, udpPort, localIPaddress);
            _udpClientWrapper.UdpMessageReceived += OnUdpMessageReceived;
            AddItemToMessagesList($"UDP Client started at {_myIpAddress}");
        }

        private void SendAddMeMessage()
        {
            var msg = new UdpPacket() { IpAddress = _myIpAddress.ToString(), NodeId = _myNodeId, Port = _myPort, MessageTypeIdentifier = MessageType.Enter };
            // Send
            _udpClientWrapper.SendMulticast(msg.GetDataStream());
            //DisplayClientMessage("Sent message: " + $"{msg.ChatMessage}", $"{_myNodeId}@{_myIpAddress}");
        }

        //private void btnDisconnectNetwork_Click(object sender, EventArgs e)
        //{
        //    OnDisconnect();

        //    btnInitNetwork.Enabled = true;
        //    btnInitNetwork.Text = "Connect";
        //    btnDisconnectNetwork.Enabled = false;
        //}
    }

    public class Peer
    {
        public string NodeId { get; set; }

        public IPAddress IpAddress { get; set; }

        public int Port { get; set; }
    }
}