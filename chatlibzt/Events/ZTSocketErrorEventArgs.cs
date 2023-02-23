using System.Net;

namespace chatlibzt.Events
{
    public class ZTSocketErrorEventArgs : ChatAppErrorEventArgs
    {

        public int SocketErrorCode { get; protected set; }
        public int ServiceErrorCode { get; protected set; }
        public ZTSocketErrorEventArgs(string message, IPAddress sender, int serviceErrorCode, int socketErrorCode) : base(message, sender)
        {
            ServiceErrorCode = serviceErrorCode;
            SocketErrorCode = socketErrorCode;
            Message = $"{Message}, Service error code: {ServiceErrorCode}, Socket error code: {SocketErrorCode}";
        }
    }
    public delegate void ZTSocketErrorEventHandler(ZTSocketErrorEventArgs e);
}
