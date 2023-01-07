using System.Net;
using System.Net.Sockets;
using System.Text;
using ZTSocket = ZeroTier.Sockets.Socket;

namespace SimpleChat
{
    public static class ZSocketExtensions
    {

        //public static string ReceiveMessage(this ZTSocket socket)
        //{
        //    var responseStr = string.Empty;

        //    var response = socket.ReceiveAll();
        //    if (response != null && response.Length > 0)
        //    {
        //        responseStr += Encoding.ASCII.GetString(response);
        //    }
        //    if (responseStr.EndsWith(":EOM"))
        //    {
        //        responseStr = responseStr.Remove(responseStr.Length - 4);
        //    }
        //    return responseStr;
        //}

        //private static byte[] ReceiveAll(this ZTSocket socket)
        //{
        //    var buffer = new List<byte>();
        //    var byteCounter = 1;
        //    while (byteCounter > 0)
        //    {
        //        var currByte = new byte[1];
        //        byteCounter = socket.Receive(currByte, 0, currByte.Length, SocketFlags.None);

        //        if (byteCounter.Equals(1))
        //        {
        //            buffer.Add(currByte[0]);
        //        }
        //    }

        //    return buffer.ToArray();
        //}
        public static string ReceiveMessage(this ZTSocket socket)
        {
            string? responseStr;
            while (true)
            {
                List<byte> bufferList = new List<byte>();
                var buffer = new byte[1024];
                var byteCounter = socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                if (byteCounter > 0)
                {
                    bufferList.AddRange(buffer.Take(byteCounter));
                }
                responseStr = Encoding.ASCII.GetString(bufferList.ToArray());
                if (responseStr.EndsWith(":EOM"))
                {
                    responseStr = responseStr.Remove(responseStr.Length - 4);
                    break;
                }
            }
            return responseStr;
        }
    }
}
