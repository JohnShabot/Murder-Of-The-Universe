using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class ServerSend
{
    #region Sends
    private static void SendUDPData(int toClient, Packet packet)
    {
        packet.WriteLength();
        ServerHost.clients[toClient].udp.sendData(packet);
    }
    private static void SendUDPDataToAll(Packet packet)
    {
        for (int i = 1; i <= ServerHost.maxPlayers; i++)
        {
            SendUDPData(i, packet);
        }
    }
    private static void SendTCPData(int toClient, Packet packet)
    {
        packet.WriteLength();
        ServerHost.clients[toClient].tcp.sendData(packet);
    }
    private static void SendTCPDataToAll(Packet packet)
    {
        for (int i = 1; i <= ServerHost.maxPlayers; i++)
        {
            SendTCPData(i, packet);
        }
    }
    private static void SendTCPDataToAll(int exceptClient, Packet packet)
    {
        for (int i = 1; i <= ServerHost.maxPlayers; i++)
        {
            if (ServerHost.clients[i] != null)
            {
                if (i != exceptClient) SendTCPData(i, packet);
            }
        }
    } 
    #endregion

    #region Packet Creation
    public static void welcome(int toClient, string hostName)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(hostName);
            _packet.Write(toClient);

            SendTCPData(toClient, _packet);
        }
    }

    public static void ready()
    {
        using (Packet _packet = new Packet((int)ServerPackets.ready))
        {
            SendTCPDataToAll(_packet);
        }
    }

    public static void spawnEnemy(Vector2 pos, int EnemyType)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(pos);
            _packet.Write(EnemyType);
            SendTCPDataToAll(_packet);
        }
    }

    public static void udpTest(int toClient)
    {
        using (Packet _packet = new Packet((int)ServerPackets.udpTest))
        {
            _packet.Write("THIS IS A UDP TEST!!! -server");
            SendUDPData(toClient, _packet);
        }
    }

    public static void updatePosRot(Vector2 pos, float rot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.updatePosRot))
        {
            _packet.Write(pos);
            _packet.Write(rot);
            SendUDPDataToAll(_packet);
        }
    }
    public static void updateEnemyPos(Vector2 pos, float rot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.updateEnemyPos))
        {
            _packet.Write(pos);
            _packet.Write(rot);
            SendUDPDataToAll(_packet);
        }
    }
    public static void shoot()
    {
        using (Packet _packet = new Packet((int)ServerPackets.shoot))
        {
            SendUDPDataToAll(_packet);
        }
    }
    #endregion
}
