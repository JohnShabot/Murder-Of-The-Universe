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
        foreach(int k in ServerHost.clients.Keys)
        {
            if (ServerHost.clients[k] != null) SendUDPData(k, packet);
        }
    }
    private static void SendUDPDataToAll(int exceptClient, Packet packet)
    {
        foreach (int k in ServerHost.clients.Keys)
        {
            if (ServerHost.clients[k] != null)
            {
                if (k != exceptClient) SendUDPData(k, packet);
            }
        }
    }
    private static void SendTCPData(int toClient, Packet packet)
    {
        packet.WriteLength();
        ServerHost.clients[toClient].tcp.sendData(packet);
    }
    private static void SendTCPDataToAll(Packet packet)
    {
        foreach (int k in ServerHost.clients.Keys)
        {
            if (ServerHost.clients[k] != null)  SendTCPData(k, packet);
        }
    }
    private static void SendTCPDataToAll(int exceptClient, Packet packet)
    {
        foreach (int k in ServerHost.clients.Keys)
        {
            if (ServerHost.clients[k] != null)
            {
                if (k != exceptClient) SendTCPData(k, packet);
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
    public static void spawnEnemy(Vector2 pos, string EnemyType)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            _packet.Write(pos);
            _packet.Write(EnemyType);
            SendTCPDataToAll(_packet);
        }
    }
    public static void spawnitem(int ItemType)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnItem))
        {
            _packet.Write(ItemType);
            SendTCPDataToAll(_packet);
        }
    }
    public static void damageEnemy(int id, float dmg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.damageEnemy))
        {
            _packet.Write(id);
            _packet.Write(dmg);
            SendTCPDataToAll(_packet);
        }
    }
    public static void damagePlayer(int id, float dmg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.damagePlayer))
        {
            _packet.Write(id);
            _packet.Write(dmg);
            SendTCPDataToAll(id, _packet);
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
    public static void updateEnemyPos(int id, Vector2 pos, float rot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.updateEnemyPos))
        {
            _packet.Write(id);
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
    public static void addItem(int PID, int IID)
    {
        using (Packet _packet = new Packet((int)ServerPackets.addItem))
        {
            _packet.Write(PID);
            _packet.Write(IID);
            SendTCPDataToAll(PID, _packet);
        }
    }
    public static void bossKilled(string bossKilled)
    {
        using (Packet _packet = new Packet((int)ServerPackets.bossKilled))
        {
            _packet.Write(bossKilled);
            SendTCPDataToAll(_packet);
        }
    }
    public static void RevivePlayer()
    {
        using (Packet _packet = new Packet((int)ServerPackets.revivePlayer))
        {
            SendTCPDataToAll(_packet);
        }
    }
    public static void Win()
    {
        using (Packet _packet = new Packet((int)ServerPackets.win))
        {
            SendTCPDataToAll(_packet);
        }
    }
    public static void Lose()
    {
        using (Packet _packet = new Packet((int)ServerPackets.lose))
        {
            SendTCPDataToAll(_packet);
        }
    }
    #endregion
}
