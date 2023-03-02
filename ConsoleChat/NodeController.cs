using chatlibzt;

namespace ConsoleChat
{
    internal class NodeController
    {
        ZeroTierNodeManager _manager;
        readonly ManualResetEvent waitHandle = new ManualResetEvent(false);

        public string NodeId => _manager?.NodeId ?? string.Empty;
        internal WaitHandle JoinNetwork(string networkIdStr)
        {
            _manager = new ZeroTierNodeManager();
            _manager.MessageReceivedEvent += OnManagerMessageReceived; ;
            _manager.NetworkUpdatedEvent += OnNetworkUpdatedEvent;
            ulong networkId = (ulong)long.Parse(networkIdStr, System.Globalization.NumberStyles.HexNumber);
            _manager.StartZeroTier(networkId);
            ConsoleUtils.WriteMessage("Waiting for authentication...", "system");
            //Task.Run(() => { Thread.Sleep(5000); waitHandle.Set(); });
            return waitHandle;
        }
        internal void Disconnect()
        {
            if (_manager != null)
            {
                _manager.MessageReceivedEvent -= OnManagerMessageReceived;
                _manager.NetworkUpdatedEvent -= OnNetworkUpdatedEvent;
                _manager.StopZeroTier();
            }
        }
        void OnManagerMessageReceived(object sender, ZeroTierManagerMessageEventArgs e)
        {
            ConsoleUtils.WriteMessage(e.Message, "system");
        }

        void OnNetworkUpdatedEvent(object sender, NetworkUpdatedEventArgs e)
        {
            if (e.IPAddresses != null && e.IPAddresses.Any())
            {
                ConsoleUtils.WriteMessage("Joined network successfully", "system");
                var myIpAddress = e.IPAddresses.First();
                ConsoleUtils.WriteMessage($"NodeId: {_manager.NodeId}", "system");
                ConsoleUtils.WriteMessage($"IpAddress: {myIpAddress}", "system");
                waitHandle.Set();
            }
        }
    }
}
