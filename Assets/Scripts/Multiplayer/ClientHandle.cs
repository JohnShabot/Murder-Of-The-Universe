using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void welcome(Packet packet)
    {
        string hostName = packet.ReadString();
        int id = packet.ReadInt();

        Debug.Log($"The host's name is {hostName}");
        NetworkManager.instance.myId = id;
        UIManager.instance.ChangeP1Name(hostName);
        ClientSend.welcomeRecieved(NetworkManager.instance.username);

        NetworkManager.instance.udp.Connect(((IPEndPoint)NetworkManager.instance.tcp.socket.Client.LocalEndPoint).Port);
    }
    public static void udpTest(Packet packet)
    {
        string msg = packet.ReadString();
        Debug.Log(msg);
        ClientSend.udpTestRecieved();
    }
    public static void ready(Packet packet)
    {
        GameManager.instance.readyS();
        Debug.Log("Host Is Ready");
    }
    public static void updatePosRot(Packet packet)
    {
        GameManager.instance.Players[0].GetComponent<PlayerServerController>().UpdatePosRot(packet.ReadVector2(), packet.ReadFloat());
    }
    public static void shoot(Packet packet)
    {
        GameManager.instance.Players[0].GetComponent<PlayerServerController>().Shoot();
    }
    public static void spawnEnemy(Packet packet)
    {

    }
    public static void updateEnemyPos(Packet packet)
    {

    }
}
