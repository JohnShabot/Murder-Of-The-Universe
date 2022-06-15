using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ClientHandle
{
    public static void welcome(Packet packet)
    {
        string hostName = packet.ReadString();
        int id = packet.ReadInt();
        Debug.Log($"The host's name is {hostName}");
        UIManager.instance.ChangeP1Name(hostName);
        ClientSend.welcomeRecieved(NetworkManager.instance.myId, NetworkManager.instance.username);

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
        if(GameManager.instance.Players.ContainsKey(0))
            GameManager.instance.Players[0].GetComponent<PlayerServerController>().UpdatePosRot(packet.ReadVector2(), packet.ReadFloat());
    }
    public static void shoot(Packet packet)
    {
        GameManager.instance.Players[0].GetComponent<PlayerServerController>().Shoot();
    }
    public static void spawnEnemy(Packet packet)
    {
        Debug.Log("Spawned Enemy");
        GameManager.instance.SpawnEnemy(packet.ReadVector2(), packet.ReadString());
    }
    public static void spawnItem(Packet packet)
    {
        GameManager.instance.SpawnItem(packet.ReadInt());
    }
    public static void updateEnemyPos(Packet packet)
    {
        int enemyID = packet.ReadInt();
        Vector2 pos = packet.ReadVector2();
        float rot = packet.ReadFloat();
        GameObject e = GameManager.instance.getCurrentEnemies()[enemyID];
        e.GetComponent<EnemyServerController>().UpdatePosRot(pos, rot);
    }
    public static void damageEnemy(Packet packet)
    {
        GameObject e = GameManager.instance.getCurrentEnemies()[packet.ReadInt()];
        e.GetComponent<EnemyServerController>().damage(packet.ReadFloat());
    }
    public static void damagePlayer(Packet packet)
    {
        GameObject p = GameManager.instance.Players[packet.ReadInt()];
        p.GetComponent<PlayerServerController>().damage(packet.ReadFloat());
    }
    public static void addItem(Packet packet)
    {
        GameObject p = GameManager.instance.Players[packet.ReadInt()];
        p.GetComponent<PlayerServerController>().AddItem(GameManager.instance.ItemTypes[packet.ReadInt()]);
    }
    public static void bossKilled(Packet packet)
    {
        GameManager.instance.BossKilled(packet.ReadString());
    }
    public static void RevivePlayer(Packet packet)
    {
        GameManager.instance.RevivePlayer();
    }
    public static void Win(Packet packet)
    {

    }
    public static void Lose(Packet packet)
    {

    }
}
