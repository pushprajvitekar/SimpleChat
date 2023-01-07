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
        public MessageEventArgs(string message, string sender)
        {
            Message = message;
            Sender = sender;
        }
    }
    public delegate void MessageEventHandler(MessageEventArgs e);
}
