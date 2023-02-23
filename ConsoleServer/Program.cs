// See https://aka.ms/new-console-template for more information


using System.Net;
using ConsoleServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


using IHost host = Host.CreateDefaultBuilder(args).Build();

// Ask the service provider for the configuration abstraction.
IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

// Get values from the config given their key and their target type.
string? serverip = config.GetValue<string>("Settings:ServerIpAddress");
int serverport = config.GetValue<int>("Settings:ServerPortNumber");

// Write the values to the console.
Console.WriteLine($"serverip = {serverip}");
Console.WriteLine($"serverport = {serverport}");

// Application code which might rely on the config could start here.
CancellationTokenSource ts = new();
CancellationToken ct = ts.Token;


if (args.Length == 2)
{
    serverip = args[0];
    serverport = Convert.ToInt32(args[1]);
}
if (serverport < 0 || string.IsNullOrEmpty(serverip))
{
    Console.WriteLine($"Usage: ConsoleServer.dll ipaddress portnumber");
    ts.Cancel();
    return;
}
Console.WriteLine("Starting Chat Server......");

IPAddress serveripaddr = IPAddress.Parse(serverip);
ConsoleChatServer listener;
OnStart(serveripaddr, serverport, ct);
Console.WriteLine("Chat Server started!");
do
{
    Console.WriteLine($"Type exit to quit");
}
while (Console.ReadLine() != "exit");
OnExit();
Console.WriteLine("Chat server stoppped!");
await host.RunAsync(ct);
void OnStart(IPAddress address, int port, CancellationToken cancellationToken)
{
    listener = new ConsoleChatServer(address, port);
    listener.OnMessageSending += OnMessageSending; ;
    listener.OnError += OnError;
    listener.OnSocketError += OnSocketError;
    _ = Task.Factory.StartNew(listener.Start, TaskCreationOptions.LongRunning, ct);
}

void OnSocketError(chatlibzt.Events.ZTSocketErrorEventArgs e)
{
    DisplayMessage(e.Message, e.Sender);
}

void OnError(chatlibzt.Events.ChatAppErrorEventArgs e)
{
    DisplayMessage(e.Message, e.Sender);
}

void OnMessageSending(chatlibzt.Events.ChatMessageEventArgs e)
{
    DisplayMessage(e.Message, e.Sender);
}

void OnExit()
{
    listener.OnMessageSending += OnMessageSending; ;
    listener.OnError += OnError;
    listener.OnSocketError += OnSocketError;
    listener.Stop();
    ts.Cancel();
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

