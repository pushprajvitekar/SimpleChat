using System.Net;
using System.Text;
using System.Threading;

namespace SimpleChat
{
    public partial class frmMain : Form
    {

        ZeroTierNodeManager manager;
        const string networkidstr = "416038decec8db58";
        IPAddress _myIpAddress;
        readonly int _myPort = 50001;
        Server _listener;
        Client _client;
        string _myNodeId;
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
            this.Invoke(new MethodInvoker(() => lstBoxMessages.Items.Add(message)));
        }
        private void AddItemToChatList(string message)
        {
            this.Invoke(new MethodInvoker(() => lstboxChat.Items.Add(message)));
        }
        private void AddItemToPeerList(string peerName)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                if (!lstBoxPeers.Items.Contains(peerName))
                {
                    lstBoxPeers.Items.Add(peerName);
                }
            }
            ));
        }

        private void RemoveItemToPeerList(string peerName)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                if (lstBoxPeers.Items.Contains(peerName))
                {
                    lstBoxPeers.Items.Remove(peerName);
                }
            }
             ));
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            manager = new ZeroTierNodeManager();
            manager.MessageReceivedEvent += Manager_MessageReceivedEvent;
            manager.NetworkUpdatedEvent += Manager_NetworkUpdatedEvent;
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
        private  void OnNetworkAuthenticated()
        {
            try
            {
                txtMyIp.Text = _myIpAddress.ToString();
                var epLocal = new IPEndPoint(IPAddress.Parse(txtMyIp.Text),
                 Convert.ToInt32(txtMyPort.Text));
                if (_listener != null)
                {
                    _listener.OnMessageSending -= Server_OnMessageSending;
                    _listener.OnError -= Server_OnError; ;
                    _listener.Stop();
                }
                _listener = new Server(IPAddress.Parse(txtMyIp.Text), Convert.ToInt32(txtMyPort.Text), _myNodeId);
                _listener.OnMessageSending += Server_OnMessageSending;
                _listener.OnError += Server_OnError; ;
                _ = Task.Factory.StartNew(_listener.Start, TaskCreationOptions.LongRunning);



                CancellationToken ct = ts.Token;
               
                StartUdpListener();
                _ = RunInBackground(ct);
                // release button to send message
                btnSend.Enabled = true;
                btnInitNetwork.Text = "Connected";
                btnInitNetwork.Enabled = false;
                txtMessage.Focus();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private async Task RunInBackground(CancellationToken ct)
        {
            var periodicTime = new PeriodicTimer(TimeSpan.FromSeconds(5));
            while (await periodicTime.WaitForNextTickAsync(ct))
            {
                SendAddMeMessage();
            }
        }

        private void Server_OnError(MessageEventArgs e)
        {
            this.Invoke(new MethodInvoker(() => MessageBox.Show(e.Message)));
        }

        private void Server_OnMessageSending(MessageEventArgs e)
        {
            switch (e.MessageType)
            {
                case MessageType.Message:
                    DisplayClientMessage(e.Message, e.Sender);
                    break;
                case MessageType.Enter:
                    AddItemToPeerList(e.Sender);
                    break;
                case MessageType.Exit:
                    RemoveItemToPeerList(e.Sender);
                    break;
                default: break;
            }
        }

        private void DisplayClientMessage(string message, string client)
        {
            try
            {
                AddItemToChatList($"friend[{client}]: {message}");
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

                if (_client == null)
                {
                    _client = new Client(IPAddress.Parse(txtFriendIp.Text), Convert.ToInt32(txtFriendPort.Text), _myNodeId);
                    _client.OnError += Client_OnError;
                    _client.OnMessageSending += _client_OnMessageSending;

                }
                var message = txtMessage.Text;

                if (!string.IsNullOrEmpty(message))
                {
                    var task = Task.Factory.StartNew(() => _client.Send(message));
                    AddItemToChatList($"Me[{_myNodeId}@{_myIpAddress}]: {message}");
                    txtMessage.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void _client_OnMessageSending(MessageEventArgs e)
        {
            DisplayClientMessage(e.Message, e.Sender);
        }

        private void Client_OnError(MessageEventArgs e)
        {
            this.Invoke(new MethodInvoker(() => MessageBox.Show(e.Message)));
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            ts.Cancel();
            if (_listener != null)
            {
                _listener.OnMessageSending -= Server_OnMessageSending;
                _listener.OnError -= Server_OnError; ;
                _listener.Stop();
            }

            manager.MessageReceivedEvent -= Manager_MessageReceivedEvent;
            manager.NetworkUpdatedEvent -= Manager_NetworkUpdatedEvent;
            if (_client != null)
            {
                _client.OnError -= Client_OnError;
                _client.OnMessageSending -= _client_OnMessageSending;
                _client.Disconnect();
            }
            if (udpClientWrapper != null)
            {

                udpClientWrapper.LeaveMulticastGroup($"{_myNodeId}@{_myIpAddress}");
                udpClientWrapper.UdpMessageReceived -= OnUdpMessageReceived;
            }
            manager.StopZeroTier();
        }

        private void btnInitNetwork_Click(object sender, EventArgs e)
        {

            ulong networkId = (ulong)Int64.Parse(networkidstr, System.Globalization.NumberStyles.HexNumber);
            manager.StartZeroTier(networkId);

        }

        private void lstBoxPeers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstBoxPeers.SelectedItem != null)
            {
                var senderDetails = lstBoxPeers.SelectedItem.ToString().Split("@").ToList();
                txtFriendIp.Text = senderDetails.Last();
            }
        }
        MulticastUdpClient udpClientWrapper;


        /// <summary>
        /// UDP Message received event
        /// </summary>
        void OnUdpMessageReceived(object sender, MulticastUdpClient.UdpMessageReceivedEventArgs e)
        {
            MessagePacket receivedData = new MessagePacket(e.Buffer);
            if (receivedData != null && receivedData.MessageTypeIdentifier != MessageType.Null)
            {
                var senderDetails = receivedData.ChatName.Split("@").ToList();
                var senderip = IPAddress.Parse(senderDetails.Last());
                var senderNodeID = senderDetails.First();
                if (senderip == _myIpAddress || senderNodeID == _myNodeId) return;
                switch (receivedData.MessageTypeIdentifier)
                {
                    case MessageType.Message:
                        DisplayClientMessage(receivedData.ChatMessage, receivedData.ChatName);
                        break;
                    case MessageType.Enter:
                        AddItemToPeerList(receivedData.ChatName);
                        break;
                    case MessageType.Exit:
                        RemoveItemToPeerList(receivedData.ChatName);
                        break;
                    default: break;
                }
            }
        }


        private void btnStartUdpClient_Click(object sender, EventArgs e)
        {
            StartUdpListener();
        }

        private void StartUdpListener()
        {
            // Create address objects
            int port = 2222;//Int32.Parse(txtPort.Text);
            IPAddress multicastIPaddress = IPAddress.Parse("239.0.0.222");
            IPAddress localIPaddress = IPAddress.Any;

            // Create MulticastUdpClient
            udpClientWrapper = new MulticastUdpClient(multicastIPaddress, port, localIPaddress);
            udpClientWrapper.UdpMessageReceived += OnUdpMessageReceived;
            DisplayClientMessage("UDP Client started", _myIpAddress.ToString());
        }

        private void btnSendUdpMessage_Click(object sender, EventArgs e)
        {
            SendAddMeMessage();
        }

        private void SendAddMeMessage()
        {
            var msg = new MessagePacket() { ChatMessage = "Add Me", ChatName = $"{_myNodeId}@{_myIpAddress}", MessageTypeIdentifier = MessageType.Enter };
            // Send
            udpClientWrapper.SendMulticast(msg.GetDataStream());
            DisplayClientMessage("Sent message: " + $"{msg.ChatMessage}", $"{_myNodeId}@{_myIpAddress}");
        }
    }
}