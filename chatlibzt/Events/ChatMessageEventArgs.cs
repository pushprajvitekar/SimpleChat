using System.Net;

namespace chatlibzt.Events
{
    public class ChatMessageEventArgs : EventArgs
    {
        public string Message { get; protected set; }
        public string Sender => SenderIpAddress != null ? SenderIpAddress.ToString() : string.Empty;
        public IPAddress SenderIpAddress { get; protected set; }
        public MessageType MessageType { get; }
        public string NodeId { get; set; }
        public string SenderName => $"{NodeId}";
        public ChatMessageEventArgs(string message, IPAddress sender, string nodeId)
        {
            Message = message;
            SenderIpAddress = sender;
            NodeId = nodeId;
        }
    }
    public delegate void ChatMessageEventHandler(ChatMessageEventArgs e);
}
