using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat
{
    internal class ClientDiscovery
    {
        public ClientDiscovery(IPAddress myIpAddress, string myNodeId)
        {
            MyIpAddress = myIpAddress;
            MyNodeId = myNodeId;
        }
        bool _stopListening = false;

        public IPAddress MyIpAddress { get; }
        public string MyNodeId { get; }

        public void StartListening()
        {
            //var Server = new UdpClient(8888);
            var responseData = Encoding.ASCII.GetBytes("SomeResponseData");

            while (!_stopListening)
            {
                //var ClientEp = new IPEndPoint(IPAddress.Any, 0);
                //var ClientRequestData = Server.Receive(ref ClientEp);
                //var ClientRequest = Encoding.ASCII.GetString(ClientRequestData);

                //Console.WriteLine("Recived {0} from {1}, sending response", ClientRequest, ClientEp.Address.ToString());
                //Server.Send(ResponseData, ResponseData.Length, ClientEp);
                var server = new UdpClient(8888);
                var clientEp = new IPEndPoint(IPAddress.Any, 0);
                var clientRequestData = server.Receive(ref clientEp);
                var clientRequest = Encoding.ASCII.GetString(clientRequestData);

                Console.WriteLine($"Recived {clientRequest} from {clientEp.Address}, sending response: {responseData} ");
                server.Send(responseData, responseData.Length, clientEp);
                server.Close();
            }

        }
        public void StopListening()
        {
            _stopListening = true;
        }
        public void DiscoverMe()
        {

            IPAddress broadcast = IPAddress.Parse("10.10.12.255");

            var Client = new UdpClient();
            var RequestData = Encoding.ASCII.GetBytes("SomeRequestData");
            var ServerEp = new IPEndPoint(IPAddress.Any, 0);

            Client.EnableBroadcast = true;
            Client.Send(RequestData, RequestData.Length, new IPEndPoint(broadcast, 8888));

            var ServerResponseData = Client.Receive(ref ServerEp);
            var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
            Console.WriteLine("Recived {0} from {1}", ServerResponse, ServerEp.Address.ToString());

            Client.Close();
        }
        //public void DiscoverMe()
        //{
        //    var remotePort = 50001;
        //    for (int i = 1; i <= 255; i++)
        //    {
        //        var remoteIp = $"10.10.12.{i}";
        //        IPAddress broadcast = IPAddress.Parse(remoteIp);
        //        var Client = new Client(broadcast, remotePort, MyNodeId);
        //        Client.Send("Discover me", MessageType.Enter);
        //    }

        //}

        //public void Leaving()
        //{
        //    IPAddress broadcast = IPAddress.Parse("10.10.12.255");
        //    var Client = new Client(broadcast, 50001, MyNodeId);
        //    Client.Send("Leaving", MessageType.Exit);
        //}

        public async Task Run(TimeSpan period, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(period, cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                    DiscoverMe();
                //else
                //    Leaving();
            }
        }

        public Task SendDiscoverMessagePeriodically(TimeSpan period, CancellationToken cancellationToken)
        {
            return Run(period, cancellationToken);
        }

    }
}
