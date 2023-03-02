using chatlibzt;
using System.Net;

namespace ConsoleChat.Client
{
    internal class ConsoleClient
    {
        ChatMember chatMember;
        internal async Task<int> StartClient(string nodeId, IPAddress serveripaddr, int serverport)
        {
            try
            {
                ConsoleUtils.WriteToConsole($"Enter user name to display in chat...");
                var userName = Console.ReadLine();
                chatMember = new ChatMember(serveripaddr, serverport, nodeId, userName: userName ?? nodeId);
                chatMember.ClientSocketError += ChatMember_ClientSocketError;
                chatMember.ClientError += ChatMember_ClientError;
                chatMember.MessageReceived += ChatMember_MessageReceived;
                ConsoleUtils.WriteToConsole("Connecting to server...");
                var isConnected = chatMember.Connect();
                if (isConnected)
                {
                    ConsoleUtils.WriteToConsole("Connected to server.");
                    chatMember.Send("Join chat", MessageType.Enter);
                    await RunChatLoopAsync();
                    return 0;
                }
                else
                {
                    ConsoleUtils.WriteToConsole("Unable to connect to server.");
                    return -1;
                }

            }
            catch (Exception ex)
            {
                ConsoleUtils.WriteToConsole(ex.ToString());
                return -1;
            }
            finally
            {
                Disconnect();
            }
        }
        async Task RunChatLoopAsync()
        {
            while (true)
            {
                var consoleInput = ConsoleUtils.ReadFromConsole();
                if (string.IsNullOrWhiteSpace(consoleInput)) continue;
                if (string.Equals(consoleInput, ":q", StringComparison.OrdinalIgnoreCase))
                {
                    ConsoleUtils.WriteToConsole("Exiting chat....");
                    break;
                }
                await Task.Run(() => { chatMember.Send(consoleInput); });
                ConsoleUtils.WriteMessage("Message sent.", "system");
            }
        }
        private void ChatMember_MessageReceived(chatlibzt.Events.ChatMessageEventArgs e)
        {
            ConsoleUtils.WriteMessage(e.Message, e.Sender);
        }

        private void ChatMember_ClientError(chatlibzt.Events.ChatAppErrorEventArgs e)
        {
            ConsoleUtils.WriteMessage(e.Message, e.Sender);
        }

        internal void Disconnect()
        {
            if (chatMember != null)
            {
                chatMember.ClientSocketError -= ChatMember_ClientSocketError;
                chatMember.ClientError -= ChatMember_ClientError;
                chatMember.MessageReceived -= ChatMember_MessageReceived;
                chatMember.Disconnect();
            }
        }
        private void ChatMember_ClientSocketError(chatlibzt.Events.ZTSocketErrorEventArgs e)
        {
            ConsoleUtils.WriteMessage(e.Message, e.Sender);
        }
    }
}
