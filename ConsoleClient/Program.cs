using System.Net;
using chatlibzt;
using ConsoleClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


using IHost host = Host.CreateDefaultBuilder(args).Build();
ManualResetEvent waitHandle = new ManualResetEvent(false);
// Ask the service provider for the configuration abstraction.
IConfiguration config = host.Services.GetRequiredService<IConfiguration>();
ZeroTierNodeManager _manager;
// Get values from the config given their key and their target type.
var serverip = config.GetValue<string>("Settings:ServerIpAddress");
var serverport = config.GetValue<int>("Settings:ServerPortNumber");
var networkid = config.GetValue<string>("Settings:NetworkId");
Console.WriteLine($"networkid = {networkid}");
Console.WriteLine($"serverip = {serverip}");
Console.WriteLine($"serverport = {serverport}");

// Application code which might rely on the config could start here.
CancellationTokenSource ts = new();
CancellationToken ct = ts.Token;
if (string.IsNullOrEmpty(networkid)) {
    Console.WriteLine($"Network Id not found!!!. Please enter network id in appsettings.json file.");
    return; 
}
if (args.Length == 2)
{
    serverip = args[0];
    serverport = Convert.ToInt32(args[1]);
}
if (serverport < 0 || string.IsNullOrEmpty(serverip))
{
    Console.WriteLine($"Usage: ConsoleClient.dll ipaddress portnumber");
    ts.Cancel();
    return;
}
Console.WriteLine("Starting Chat Client......");

var serveripaddr = IPAddress.Parse(serverip);

JoinNetwork(networkid);
waitHandle.WaitOne();
Console.WriteLine("Stopping Chat client...");
Disconnect();
Console.WriteLine("Chat client stoppped!");
Console.ReadKey();

void JoinNetwork(string networkIdStr)
{
    _manager = new ZeroTierNodeManager();
    _manager.MessageReceivedEvent += OnManagerMessageReceived; ;
    _manager.NetworkUpdatedEvent += OnNetworkUpdatedEvent;
    ulong networkId = (ulong)long.Parse(networkIdStr, System.Globalization.NumberStyles.HexNumber);
    _manager.StartZeroTier(networkId);
    Console.WriteLine("Waiting for authentication...");
}

void OnManagerMessageReceived(object sender, ZeroTierManagerMessageEventArgs e)
{
    DisplayMessage(e.Message, "system");
}

void OnNetworkUpdatedEvent(object sender, NetworkUpdatedEventArgs e)
{
    if (e.IPAddresses != null && e.IPAddresses.Any())
    {
        DisplayMessage("Joined network successfully", "system");
        var myIpAddress = e.IPAddresses.First();
        DisplayMessage($"NodeId: {_manager.NodeId}", "system");
        DisplayMessage($"IpAddress: {myIpAddress}", "system");
        StartClient(_manager.NodeId, myIpAddress);
    }
}



void StartClient(string nodeId, IPAddress myIpAddress)
{

    try
    {
        Console.WriteLine($"Enter user name to display in chat...");
        var userName = Console.ReadLine();
        var client = new Client(serveripaddr, serverport, nodeId, userName);
        Console.WriteLine("Connecting to server...");
        var isConnected = client.Connect();
        if (isConnected)
        {
            Console.WriteLine("Connected to server.");
            client.Send("Join chat", MessageType.Enter);
            Console.WriteLine("Enter message...");
            do
            {
                var message = Console.ReadLine();
                if (!string.IsNullOrEmpty(message))
                {
                    client.Send(message);
                    DisplayMessage("Message sent.", "system");
                }
                Console.WriteLine($"To end client Type \"exit\"");

            }
            while (Console.ReadLine() != "exit");
        }
        else
        {
            Console.WriteLine("Unable to connect to server.");
        }

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
    finally {
        waitHandle.Set();
    }
}
void DisplayMessage(string message, string client)
{
    try
    {
        Console.WriteLine($"{DateTime.Now.ToShortTimeString()}:[{client}]: {message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}

void Disconnect()
{
    ts.Cancel();

    if (_manager != null)
    {
        _manager.MessageReceivedEvent -= OnManagerMessageReceived;
        _manager.NetworkUpdatedEvent -= OnNetworkUpdatedEvent;
        _manager.StopZeroTier();
    }
}