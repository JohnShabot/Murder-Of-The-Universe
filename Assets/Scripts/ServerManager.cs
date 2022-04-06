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
        byte[] sendData = Encoding.ASCII.GetBytes("START| "); // Turns data to bytes
        NetworkStream stream = M.GetStream();
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        byte[] buffer= new byte[1024];
        stream.Read(buffer,0,1024);
        string data = Encoding.ASCII.GetString(buffer);
        Debug.Log(data);
    }
    public void ConnectToHost(string hostName, string pass)
    {
        byte[] sendData = Encoding.ASCII.GetBytes("JOIN|" + hostName + "#" + Hash128.Compute(pass).ToString()); // Turns data to bytes
        NetworkStream stream = M.GetStream();
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        byte[] buffer = new byte[1024];
        stream.Read(buffer, 0, 1024);
        string data = Encoding.ASCII.GetString(buffer);
        Debug.Log(data);
        buffer = new byte[1024];
        stream.Read(buffer, 0, 1024);
        data = Encoding.ASCII.GetString(buffer);
        Debug.Log(data);
        sendData = Encoding.ASCII.GetBytes("CLOSE| "); // Turns data to bytes
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        M.Close(); // Closes the connection
    }
    public void Host(string roomName, string pass, string username)
    {
        byte[] sendData = Encoding.ASCII.GetBytes("HOST|" + roomName + "#" + Hash128.Compute(pass).ToString() + "#" + username); // Turns data to bytes
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
    public void Register(string roomName, string email, string pass)
    {
        byte[] sendData = Encoding.ASCII.GetBytes("REGISTER|" + roomName + "#" + Hash128.Compute(pass).ToString() + "#" + email); // Turns data to bytes
        NetworkStream stream = M.GetStream();
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        byte[] buffer = new byte[1024]; // The variable that will store the recieved data
        stream.Read(buffer, 0, 1024); // Read the data
        string data = Encoding.ASCII.GetString(buffer); // Turn the data into a string
        Debug.Log(data);
    }
    public int Login(string name, string pass)
    {
        byte[] sendData = Encoding.ASCII.GetBytes("LOGIN|" + name + "#" + Hash128.Compute(pass).ToString()); // Turns data to bytes
        NetworkStream stream = M.GetStream();
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        byte[] buffer = new byte[1024]; // The variable that will store the recieved data
        stream.Read(buffer, 0, 1024); // Read the data
        string data = Encoding.ASCII.GetString(buffer); // Turn the data into an int
        Debug.Log(data);
        stream.Read(buffer, 0, 1024);
        Debug.Log(Encoding.ASCII.GetString(buffer));
        return int.Parse(data);
    }
    public int Verify(string code)
    {
        byte[] sendData = Encoding.ASCII.GetBytes("VERIFY|" + code); // Turns data to bytes
        NetworkStream stream = M.GetStream();
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        byte[] buffer = new byte[1024]; // The variable that will store the recieved data
        stream.Read(buffer, 0, 1024); // Read the data
        string data = Encoding.ASCII.GetString(buffer); // Turn the data into an int
        Debug.Log(data);
        stream.Read(buffer, 0, 1024);
        Debug.Log(Encoding.ASCII.GetString(buffer));
        return int.Parse(data);
    }
    public void CloseConnection()
    {
        byte[] sendData = Encoding.ASCII.GetBytes("Close| "); // Turns data to bytes
        NetworkStream stream = M.GetStream();
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        M.Close();
    }
}
