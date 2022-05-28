using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void welcomeReceived(int fromClient, Packet packet)
    {
        int idFrom = packet.ReadInt();
        string name = packet.ReadString();

        UIManager.instance.ChangeP2Name(name);
        Debug.Log($"Client {idFrom}'s name is {name}!");
        if(fromClient != idFrom)
        {
            Debug.Log($"Player {name} (ID: {fromClient}) has assumed the wrong client ID ({idFrom})!!!");
        }
    }
    public static void udpTestReceived(int fromClient, Packet packet)
    {
        string msg = packet.ReadString();
        Debug.Log($"message from client recieved via UDP: {msg}");
    }
    public static void ready(int fromClient, Packet packet)
    {
        GameManager.instance.readyC();
        Debug.Log($"Client {fromClient} Is Ready");
    }
    public static void updatePosRot(int fromClient, Packet packet)
    {
        GameManager.instance.Players[fromClient].GetComponent<PlayerServerController>().UpdatePosRot(packet.ReadVector2(), packet.ReadFloat());
    }
    public static void shoot(int fromClient, Packet packet)
    {
        GameManager.instance.Players[fromClient].GetComponent<PlayerServerController>().Shoot();
    }
    public static void damagePlayer(int fromClient, Packet packet)
    {
        GameManager.instance.Players[fromClient].GetComponent<PlayerServerController>().damage(packet.ReadFloat());
    }
    public static void addItem(int fromClient, Packet packet)
    {
        GameObject p = GameManager.instance.Players[fromClient];
        p.GetComponent<PlayerServerController>().AddItem(GameManager.instance.ItemTypes[packet.ReadInt()]);
    }
    public static void removeItem(int fromClient, Packet packet)
    {
        GameObject p = GameManager.instance.Players[fromClient];
        p.GetComponent<PlayerServerController>().RemoveItem(GameManager.instance.ItemTypes[packet.ReadInt()].GetComponent<ItemPickup>().item);
    }
}
