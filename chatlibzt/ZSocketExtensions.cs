﻿using System.Net;
using System.Net.Sockets;
using System.Text;
using ZeroTier.Sockets;
using ZTSocket = ZeroTier.Sockets.Socket;

namespace chatlibzt
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

        public static MessagePacket ReceiveMessagePacket(this ZTSocket socket)
        {
            List<byte> completeBuffer = new List<byte>();
            string? responseStr;
            MessagePacket receivedData = new MessagePacket();
            while (true)
            {
                var buffer = new byte[1024];
                var byteCounter = socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                if (byteCounter > 0)
                {
                    completeBuffer.AddRange(buffer.Take(byteCounter));
                    responseStr = Encoding.ASCII.GetString(buffer.ToArray());
                    if (responseStr.EndsWith(":EOM"))
                    {
                        receivedData = new MessagePacket(completeBuffer.ToArray());
                        break;
                    }
                }
                else
                {
                    receivedData = new MessagePacket() { MessageTypeIdentifier = MessageType.Exit };
                    break;
                }
            }
            // Initialise a packet object to store the received data
            
            return receivedData;
        }
    }
}
