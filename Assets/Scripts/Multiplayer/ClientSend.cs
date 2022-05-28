using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend
{
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        NetworkManager.instance.tcp.sendData(packet);
    }

    private static void SendUDPData(Packet packet)
    {
        packet.WriteLength();
        NetworkManager.instance.udp.sendData(packet);
    }
    #region Packets
    public static void welcomeRecieved(string clientName)
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(NetworkManager.instance.myId);
            _packet.Write(clientName);

            SendTCPData(_packet);
        }
    }
    public static void udpTestRecieved()
    {
        using (Packet _packet = new Packet((int)ClientPackets.udpTestRecieved))
        {
            _packet.Write("Recieved a UDP Packet");

            SendUDPData(_packet);
        }
    }
    public static void damagePlayer(float dmg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.damagePlayer))
        {
            _packet.Write(dmg);
            SendTCPData(_packet);
        }
    }
    public static void ready()
    {
        using (Packet _packet = new Packet((int)ClientPackets.ready))
        {
            SendTCPData(_packet);
        }
    }
    public static void updatePosRot(Vector2 pos, float rot)
    {
        using (Packet _packet = new Packet((int)ClientPackets.updatePosRot))
        {
            _packet.Write(pos);
            _packet.Write(rot);
            SendUDPData(_packet);
        }
    }
    public static void shoot()
    {
        using (Packet _packet = new Packet((int)ClientPackets.shoot))
        {
            SendUDPData(_packet);
        }
    }
    public static void addItem(int IID)
    {
        using (Packet _packet = new Packet((int)ClientPackets.addItem))
        {
            _packet.Write(IID);
            SendTCPData(_packet);
        }
    }
    #endregion
}
