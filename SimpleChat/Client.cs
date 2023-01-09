﻿using System.Net;
using System.Net.Sockets;
using ZTSocket = ZeroTier.Sockets.Socket;
using ZTSockets = ZeroTier.Sockets;
namespace SimpleChat
{
    public class Client
    {
        public event MessageEventHandler OnMessageSending;
        public event MessageEventHandler OnError;
        ZTSocket _sender;
        public Client(IPAddress remoteIpAddress, int remotePortNumber, string nodeId)
        {

            RemoteIpAddress = remoteIpAddress;
            PortNumber = remotePortNumber;
            RemoteEndPoint = new IPEndPoint(RemoteIpAddress, PortNumber);
            // Create a TCP/IP  socket.
            _sender = new ZTSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    _sender.SendTimeout = 50;
         //   _sender.ReceiveTimeout = 50;
            NodeId = nodeId;
        }
        public string  NodeId { get; set; }
        public IPEndPoint RemoteEndPoint { get; }
        public IPAddress RemoteIpAddress { get; }
        public int PortNumber { get; }
        public bool Connect()
        {
            if (_sender == null)
            {
                _sender = new ZTSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            try
            {
                if (!_sender.Connected)
                    _sender.Connect(RemoteEndPoint);
            }
            catch (Exception e)
            {
                OnError?.Invoke(new MessageEventArgs($"Error: {e.Message}", RemoteIpAddress.ToString()));
            }
            return _sender.Connected;
        }
        public void Disconnect()
        {
            if (_sender != null)
            {
                // Release the socket.
                _sender.Shutdown(SocketShutdown.Both);
                _sender.Close();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _sender = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }
        public void Send(string message, MessageType messageType = MessageType.Message)
        {
            // Connect to a remote device.
            try
            {
                Connect();

                // Initialise a packet object to store the data to be sent
                MessagePacket sendData = new MessagePacket
                {
                    ChatName = NodeId,
                    ChatMessage = message,
                    MessageTypeIdentifier = messageType
                };

                // Get packet as byte array
                byte[] byteData = sendData.GetDataStream();

               // var socketFlag = messageType == MessageType.Message ? SocketFlags.None : SocketFlags.Broadcast;
                int bytesSent = _sender.Send(byteData, 0, byteData.Length, SocketFlags.None);

                if (bytesSent > 0)
                {
                    var response = _sender.ReceiveMessagePacket();
                    if (response.MessageTypeIdentifier == MessageType.Ack)
                    {
                        OnMessageSending?.Invoke(new MessageEventArgs($"Ack received", RemoteEndPoint.ToString()));
                    }
                }
                else
                {
                    OnMessageSending?.Invoke(new MessageEventArgs($"Ack not received", RemoteEndPoint.ToString()));
                }
                Disconnect();
            }
            catch (ArgumentNullException ane)
            {
                OnError?.Invoke(new MessageEventArgs($"Error: {ane.Message}", RemoteIpAddress.ToString()));
            }
            catch (ZTSockets.SocketException e)
            {
                OnError?.Invoke(new MessageEventArgs($"Error: {e.Message}, Service Error Code: {e.ServiceErrorCode}, Socket Error Code: {e.SocketErrorCode} ", RemoteIpAddress.ToString()));
            }
            catch (Exception ex)
            {
                OnError?.Invoke(new MessageEventArgs($"Error: {ex.Message}", RemoteIpAddress.ToString()));
            }

        }
    }
}
