﻿using chatlibzt.Events;
using System.Net;
using System.Net.Sockets;
using ZTSocket = ZeroTier.Sockets.Socket;
using ZTSockets = ZeroTier.Sockets;

namespace chatlibzt
{
    public class Server
    {
        public event ChatMessageEventHandler OnMessageSending;
        public event ChatAppErrorEventHandler OnError;
        public event ZTSocketErrorEventHandler OnSocketError;
        private readonly ZTSocket listener;
        public Server(IPAddress localIpAddress, int portNumber, string nodeId)
        {

            LocalIpAddress = localIpAddress;
            PortNumber = portNumber;
            NodeId = nodeId;
            LocalEndPoint = new IPEndPoint(LocalIpAddress, PortNumber);
            listener = new ZTSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //listener.SendTimeout = 10;
            //listener.ReceiveTimeout = 10;
            //listener.Blocking = false;

        }

        public IPEndPoint LocalEndPoint { get; }
        public IPAddress LocalIpAddress { get; }
        public int PortNumber { get; }
        public string NodeId { get; }



        bool runLoop = true;
        public void Start()
        {


            // Bind the socket to the local endpoint and
            // listen for incoming connections.

            try
            {
                listener.Bind(LocalEndPoint);
                listener.Listen(100);
                while (runLoop)
                {
                    var xclient = listener.Accept();
                    var task = Task.Run(() => HandleClient(xclient));
                    task.Wait();
                }

            }
            catch (ZTSockets.SocketException e)
            {
                OnSocketError?.Invoke(new ZTSocketErrorEventArgs($"Error: {e.Message}", LocalIpAddress, e.ServiceErrorCode, e.SocketErrorCode));
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new ChatAppErrorEventArgs($"Error: {ex.Message}", LocalIpAddress));
            }
        }
        public void Stop()
        {
            if (listener != null)
            {
                runLoop = false;
                Thread.Sleep(1000);
                listener.Shutdown(SocketShutdown.Both);
                listener.Close();
            }
        }
        private void HandleClient(ZTSocket acceptedClient)
        {
            var clientEndpoint = (IPEndPoint)acceptedClient.RemoteEndPoint;
            var clientAddress = IPAddress.Parse(clientEndpoint.Address.ToString());
            string data = string.Empty;
            //var ackMesPacket = new MessagePacket() { MessageTypeIdentifier = MessageType.Ack, ChatName = NodeId };
            //var acKMsg = ackMesPacket.GetDataStream();
            var packet = new MessagePacket();
            try
            {
                try
                {
                    packet = acceptedClient.ReceiveMessagePacket();
                    //acceptedClient.Send(acKMsg, 0, acKMsg.Length, SocketFlags.None);
                    acceptedClient.Shutdown(SocketShutdown.Both);
                    acceptedClient.Close();
                }
                catch (ZTSockets.SocketException e)
                {
                    OnSocketError?.Invoke(new ZTSocketErrorEventArgs($"Error: {e.Message}" ,clientAddress,e.ServiceErrorCode, e.SocketErrorCode));
                }
                if (packet.MessageTypeIdentifier != MessageType.Null)
                {

                    OnMessageSending?.Invoke(new ChatMessageEventArgs(packet.ChatMessage,clientAddress, $"{packet.ChatName}"));
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new ChatAppErrorEventArgs($"Error: {ex.Message}", clientAddress));
            }
        }


    }
}
