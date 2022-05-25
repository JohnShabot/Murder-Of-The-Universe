using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class ServerHost
{
    public static int maxPlayers { get; private set; }

    public static int port { get; private set; }
    public static Dictionary<int, ServerClient> clients = new Dictionary<int, ServerClient>();

    private static TcpListener tcpListener;
    private static UdpClient UdpListener;

    public delegate void PacketHandler(int fromClient, Packet packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    public static void Start(int _max, int _port)
    {
        port = _port;
        maxPlayers = _max;

        Debug.Log("Starting Server");
        InitializeServerData();

        tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        UdpListener = new UdpClient(port);
        UdpListener.BeginReceive(UDPRecieveCallback, null);

        Debug.Log($"Server started on port {port}");
    }

    private static void TCPConnectCallback(IAsyncResult _result)
    {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        Debug.Log($"Incoming Connection from {_client.Client.RemoteEndPoint}");
        //tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
        for (int i = 1; i <= maxPlayers; i++)
        {
            if(clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(_client);
                return;
            }
        }
        Debug.Log($"{_client.Client.RemoteEndPoint} failed to Connect, Server is Full");
    }

    private static void UDPRecieveCallback(IAsyncResult result)
    {
        try
        {
            IPEndPoint ClientEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = UdpListener.EndReceive(result, ref ClientEP);
            UdpListener.BeginReceive(UDPRecieveCallback, null);

            if(data.Length < 4)
            {
                return;
            }
            
            using(Packet packet = new Packet(data))
            {
                int ClientID = packet.ReadInt();

                if(ClientID == 0)
                {
                    return;
                }

                if(clients[ClientID].udp.endPoint == null)
                {
                    clients[ClientID].udp.Connect(ClientEP);
                    return;
                }

                if(clients[ClientID].udp.endPoint.ToString() == ClientEP.ToString())
                {
                    clients[ClientID].udp.HandleData(packet);
                }
            }
        }
        catch(Exception ex)
        {
            Debug.Log($"Error recieving message from client via UDP: {ex}");
        }
    }

    public static void SendUDPData(IPEndPoint clientEP, Packet packet)
    {
        try
        {
            Debug.Log($"Sending Data To {clientEP}");
            if(clientEP != null)
            {
                UdpListener.BeginSend(packet.ToArray(), packet.Length(), clientEP ,null, null);
                Debug.Log("Sent!");
            }
        }
        catch(Exception ex)
        {
            Debug.Log($"Error sending message to {clientEP} via UDP: {ex}");
        }
    }

    public static void InitializeServerData()
    {
        for (int i = 1; i <= maxPlayers; i++)
        {
            clients[i] = new ServerClient(i);
        }
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ClientPackets.welcomeReceived, ServerHandle.welcomeReceived },
            { (int)ClientPackets.udpTestRecieved, ServerHandle.udpTestReceived },
            { (int)ClientPackets.ready, ServerHandle.ready },
            { (int)ClientPackets.updatePosRot, ServerHandle.updatePosRot },
            { (int)ClientPackets.shoot, ServerHandle.shoot }
            //{ (int)ClientPackets.addItem, ServerHandle.addItem },
            //{ (int)ClientPackets.removeItem, ServerHandle.removeItem }
        };
        Debug.Log("initialized Packets");
    }
}
