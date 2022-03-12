using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;

public class ServerManager : MonoBehaviour
{
    TcpClient M; // The socket that will connect to the main server
    Socket P2P; // The socket that will host / connect to a host in the Peer To Peer connection

    public void ConnectToMain()
    {
        M = new TcpClient("127.0.0.1", 8888);  // Connects to server 
        byte[] sendData = Encoding.ASCII.GetBytes("LOGIN"); // Turns data to bytes
        NetworkStream stream = M.GetStream();
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        byte[] buffer= new byte[1024];
        stream.Read(buffer,0,1024);
        string data = Encoding.ASCII.GetString(buffer);
        Debug.Log(data);
    }
    public void ConnectToHost(string hostName, string pass)
    {
        M.Close(); // Closes the connection
    }
    public void Host(string roomName, string pass)
    {
        byte[] sendData = Encoding.ASCII.GetBytes("HOST|" + roomName + "#" + Hash128.Compute(pass).ToString()); // Turns data to bytes
        NetworkStream stream = M.GetStream();
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        byte[] buffer = new byte[1024]; // The variable that will store the recieved data
        stream.Read(buffer, 0, 1024); // Read the data
        string data = Encoding.ASCII.GetString(buffer); // Turn the data into a string
        Debug.Log(data);
    }
    public string RefreshList()
    {
        NetworkStream stream = M.GetStream();
        byte[] sendData = Encoding.ASCII.GetBytes("REFRESH"); // Turns data to bytes
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        byte[] buffer = new byte[1024]; // The variable that will store the recieved data
        stream.Read(buffer, 0, 1024); // Read the data
        string data = Encoding.ASCII.GetString(buffer); // Turn the data into a string
        Debug.Log(data);
        stream.Read(buffer, 0, 1024); // Read the data
        data = Encoding.ASCII.GetString(buffer); // Turn the data into a string
        Debug.Log(data);
        return data;
    }
}
