using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat
{
    public class MessageEventArgs : EventArgs
    {

        public string Message { get; protected set; }
        public string Sender { get; protected set; }
        public MessageType MessageType { get; }

        public MessageEventArgs(string message, string sender, MessageType messageType = MessageType.Message)
        {
            Message = message;
            Sender = sender;
            MessageType = messageType;
        }
    }
    public delegate void MessageEventHandler(MessageEventArgs e);
}
