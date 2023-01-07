using System.Net;

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
                    this.Invoke(new MethodInvoker(() => txtMyIp.Text = _myIpAddress.ToString()));
                    this.Invoke(new MethodInvoker(() => btnStart_Click(this, new EventArgs())));
                    ;
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
        //private void AddItemToPeerList(string peerName)
        //{
        //    this.Invoke(new MethodInvoker(() => lstBoxPeers.Items.Add(peerName)));
        //}
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

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                var epLocal = new IPEndPoint(IPAddress.Parse(txtMyIp.Text),
                 Convert.ToInt32(txtMyPort.Text));
                if (_listener != null)
                {
                    _listener.OnMessageSending -= Server_OnMessageSending;
                    _listener.OnError -= Server_OnError; ;
                    _listener.Stop();
                }
                _listener = new Server(IPAddress.Parse(txtMyIp.Text), Convert.ToInt32(txtMyPort.Text));
                _listener.OnMessageSending += Server_OnMessageSending;
                _listener.OnError += Server_OnError; ;
                Task.Factory.StartNew(_listener.Start, TaskCreationOptions.LongRunning);

                // release button to send message
                btnSend.Enabled = true;
                btnStart.Text = "Connected";
                btnStart.Enabled = false;
                txtMessage.Focus();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Server_OnError(MessageEventArgs e)
        {
            this.Invoke(new MethodInvoker(() => MessageBox.Show(e.Message)));
        }

        private void Server_OnMessageSending(MessageEventArgs e)
        {
            DisplayClientMessage(e.Message, e.Sender);
        }

        private void DisplayClientMessage(string message, string client)
        {
            try
            {
                AddItemToChatList($"client: {client} says: {message}");
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
                    _client = new Client(IPAddress.Parse(txtFriendIp.Text), Convert.ToInt32(txtFriendPort.Text));
                    _client.OnError += Client_OnError;
                    _client.OnMessageSending += _client_OnMessageSending;

                }
                var message = txtMessage.Text;
                if (!string.IsNullOrEmpty(message))
                {
                    var task = Task.Factory.StartNew(() => _client.Send(message));
                    AddItemToChatList("You: " + message);
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
            manager.StopZeroTier();
        }

        private void btnInitNetwork_Click(object sender, EventArgs e)
        {

            ulong networkId = (ulong)Int64.Parse(networkidstr, System.Globalization.NumberStyles.HexNumber);
            manager.StartZeroTier(networkId);

        }
    }
}