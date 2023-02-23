using System.Net;

namespace chatlibzt.Events
{
    public delegate void ChatAppErrorEventHandler(ChatAppErrorEventArgs e);
    public class ChatAppErrorEventArgs : EventArgs
    {
        public string Message { get; protected set; }
        public string Sender => SenderIpAddress != null ? SenderIpAddress.ToString() : string.Empty;
        public IPAddress SenderIpAddress { get; protected set; }
        public ChatAppErrorEventArgs(string message, IPAddress sender)
        {
            Message = message;
            SenderIpAddress = sender;
        }
    }

}
