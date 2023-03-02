
using CommandLine;
using ConsoleChat;
using ConsoleChat.Client;
using ConsoleChat.CommandLine;
using System.Net;
internal class Program
{
    readonly static NodeController controller = new NodeController();
    private static async Task<int> Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            controller.Disconnect();
        }

        var result = await Parser.Default.ParseArguments<ClientOptions, ServerOptions>(args)
          .MapResult<ClientOptions, ServerOptions, Task<int>>(RunClient, RunServer, errors => Task.FromResult(-1));
        Console.WriteLine($"retValue= {result}");
        return result;
        static async Task<int> RunClient(ClientOptions options)
        {
            Console.WriteLine("Welcome to chat....");
            var settings = ConfigSettings.AppSettings;
            var networkId = string.IsNullOrEmpty(options?.NetworkId) ? settings.NetworkId : options.NetworkId;
            var ipaddress = string.IsNullOrEmpty(options?.IpAddress) ? settings.ServerIpAddress : options.IpAddress;
            var port = options != null && options.Port > 0 ? options.Port : settings.ServerPort;
            if (string.IsNullOrEmpty(networkId))
            {
                Console.WriteLine($"Network Id not found!!!. Please enter network id in appsettings.json file or use commandline options.");
                return -1;
            }

            if (port < 0 || string.IsNullOrEmpty(ipaddress))
            {
                Console.WriteLine($"Server IpAddress / Port number not specified.");
                return -1;
            }
            if (!IPAddress.TryParse(ipaddress, out var ip))
            {
                Console.WriteLine($"Invalid Server IpAddress.");
                return -1;
            }

            controller.JoinNetwork(networkId).WaitOne();
            var client = new ConsoleClient();
            await client.StartClient(controller.NodeId, ip, port);
            Console.WriteLine("RunClient After Task");
            controller.Disconnect();
            return 0;
        }

        static async Task<int> RunServer(ServerOptions options)
        {

            Console.WriteLine(" RunServer Before Task");
            Console.Write($"Port:{options.Port}, Ip:{options.IpAddress}, Id:{options.NetworkId}");
            await Task.Delay(20); //simulate async method
            Console.WriteLine("RunServer After Task");
            return 0;
        }
    }
}