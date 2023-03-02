using CommandLine;

namespace ConsoleChat.CommandLine
{
    [Verb("client", isDefault: true, aliases: new[] { "c" }, HelpText = "Start app in client mode")]
    internal class ClientOptions
    {
        [Option('p',  HelpText = "Port number which binds to Server socket. If not specified, value will be picked from default settings.", Required = false)]
        public int Port { get; set; }

        [Option('i', Required = false, HelpText = "IP Address which binds to Server socket. If not specified, value will be picked from default settings.")]
        public string IpAddress { get; set; }

        [Option('n', Required = false, HelpText = "Network Id of the Zero Tier network to join to. If not specified, value will be picked from default settings.")]
        public string NetworkId { get; set; }
    }
}
