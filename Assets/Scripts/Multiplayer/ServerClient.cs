using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;


public class ServerClient
{
    public static int bufferSize = 1024;
    public int id;
    public TCP tcp;
    public UDP udp;

    public ServerClient(int _clientId)
    {
        id = _clientId;
        tcp = new TCP(id);
        udp = new UDP(id);
    }
    #region TCP Class
    public class TCP
    {
        public TcpClient socket;

        private readonly int id;
        private NetworkStream stream;
        private Packet recievedData;
        private byte[] buffer;

        public TCP(int _id)
        {
            id = _id;
        }

        public void Connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = bufferSize;
            socket.SendBufferSize = bufferSize;

            stream = socket.GetStream();

            recievedData = new Packet();
            buffer = new byte[bufferSize];

            stream.BeginRead(buffer, 0, bufferSize, RecieveCallBack, null);

            ServerSend.welcome(id, NetworkManager.instance.username);
        }
        public void sendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Error sending data to player {id} via TCP: {ex}");
            }
        }
        private bool HandleData(byte[] data)
        {
            int packetLength = 0;

            recievedData.SetBytes(data);

            if (recievedData.UnreadLength() >= 4)
            {
                packetLength = recievedData.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= recievedData.UnreadLength())
            {
                byte[] packetbytes = recievedData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetbytes))
                    {
                        int _packetId = packet.ReadInt();
                        ServerHost.packetHandlers[_packetId](id, packet);
                    }
                });

                packetLength = 0;
                if (recievedData.UnreadLength() >= 4)
                {
                    packetLength = recievedData.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }
            }
            if (packetLength <= 1)
            {
                return true;
            }
            return false;
        }
        private void RecieveCallBack(IAsyncResult _result)
        {
            try
            {
                int byteLength = stream.EndRead(_result);
                if (byteLength <= 0)
                {
                    return;
                }
                byte[] data = new byte[byteLength];
                Array.Copy(buffer, data, byteLength);

                recievedData.Reset(HandleData(data));
                stream.BeginRead(buffer, 0, bufferSize, RecieveCallBack, null);
            }
            catch (Exception ex)
            {
                Debug.Log($"Error recieving TCP data: {ex}");
            }
        }
    }
    #endregion

    #region UDP Class
    public class UDP
    {
        public IPEndPoint endPoint;
        private int id;
        public UDP(int id)
        {
            this.id = id;
        }

        public void Connect(IPEndPoint endPoint)
        {
            this.endPoint = endPoint;
            ServerSend.udpTest(id);
        }
        public void sendData(Packet packet)
        {
            ServerHost.SendUDPData(endPoint, packet);
        }
        
        public void HandleData(Packet packetData)
        {
            int packetLength = packetData.ReadInt();
            byte[] data = packetData.ReadBytes(packetLength);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(data))
                {
                    int packetID = packet.ReadInt();
                    ServerHost.packetHandlers[packetID](id, packet);
                }
            });
        }
    }
    #endregion
}
