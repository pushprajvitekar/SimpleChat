using CommandLine;

namespace ConsoleChat.CommandLine
{
    [Verb("server", isDefault: false, aliases: new[] { "s" }, HelpText = "Start app in server mode")]
    internal class ServerOptions
    {
        [Option('p', HelpText = "Port number which binds to Server socket. If not specified, value will be picked from default settings.", Required = false)]
        public int Port { get; set; }

        [Option('i', SetName ="ip", Required = false, HelpText = "IP Address which binds to Server socket. If not specified, value will be picked from default settings.")]
        public string IpAddress { get; set; }

        [Option('n',SetName ="nw", Required = false, HelpText = "Network Id of the Zero Tier network to join to. If not specified, value will be picked from default settings.")]
        public string NetworkId { get; set; }
       // [Verb("createnode", isDefault: false, aliases: new[] { "n" }, HelpText = "Create node in the Zero Tier network, IP Address will be auto assigned to server.")]
        //class CreateNode
        //{
        //    [Value(0, Required = false, HelpText = "Network Id of the Zero Tier network to join to. If not specified, value will be picked from default settings.")]
        //    public string NetworkId { get; set; }


        //    [Verb("UseExistingIp", isDefault: false, aliases: new[] { "i" }, HelpText = "Use existing IP address in the ")]
        //    class UseExistingIp
        //    {
        //        [Value(0, Required = false, HelpText = "Network Id of the Zero Tier network to join to. If not specified, value will be picked from default settings.")]
        //        public string IpAddress { get; set; }
        //    }
        //}
    }
}


