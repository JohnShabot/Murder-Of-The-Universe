using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using System.Net;
using System.Threading;

public class ServerManager : MonoBehaviour
{
    TcpClient M; // The socket that will connect to the main server
    UdpClient sock; // The Client for the actual game
    IPEndPoint ep;

    bool? isServer = null; // the bool to keep if this is a host or client, or not in that phase (when it's null)
    Thread T = null;

    string username = "";
    void Update()
    {
        if(isServer.HasValue)
        {
            if (isServer.Value)
            {
                if(T == null)
                {
                    T = new Thread(new ThreadStart(HandleData));
                    T.Start();
                }
            }
            else if (!isServer.Value)
            {
                byte[] sendData = Encoding.ASCII.GetBytes("JOIN|" + username); // Turns data to bytes
                sock.Send(sendData, sendData.Length, ep);
                string data = Encoding.ASCII.GetString(sock.Receive(ref ep)); // then receive data 
                Debug.Log(data);
                Debug.Log("receive data from " + ep.ToString());
            }
        }

        
    }
    void HandleData()
    {
        string data = Encoding.ASCII.GetString(sock.Receive(ref ep)); // receive data 
        Debug.Log(data);
        Debug.Log("receive data from " + ep.ToString());
        byte[] sendData = Encoding.ASCII.GetBytes("Accepted|" + username); // Turns data to bytes
        sock.Send(sendData, sendData.Length, ep);
        T = null;
    }
    public void Join(string ip)
    {
        ep = new IPEndPoint(IPAddress.Parse(ip), 8888); // endpoint where server is listening
        sock = new UdpClient(ep); // initialize socket
        isServer = false;
        Debug.Log("JOINED");
    }

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
        string ip = Encoding.ASCII.GetString(buffer);
        Debug.Log(ip);
        buffer = new byte[1024];
        stream.Read(buffer, 0, 1024);
        string data = Encoding.ASCII.GetString(buffer);
        Debug.Log(data);
        sendData = Encoding.ASCII.GetBytes("CLOSE| "); // Turns data to bytes
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        M.Close(); // Closes the connection
        data = data.Split('\'')[1];
        Join(data);
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
        sendData = Encoding.ASCII.GetBytes("CLOSE|"); // Turns data to bytes
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        M.Close(); // Closes the connection
        ep = new IPEndPoint(IPAddress.Any, 8888);  // listen on port 8888
        sock = new UdpClient(ep); // initialize socket
        isServer = true;
    }
    public string RefreshList()
    {
        NetworkStream stream = M.GetStream();
        byte[] sendData = Encoding.ASCII.GetBytes("REFRESH|"); // Turns data to bytes
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
        username = name;
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
    public void CloseConnectionMain()
    {
        byte[] sendData = Encoding.ASCII.GetBytes("Close| "); // Turns data to bytes
        NetworkStream stream = M.GetStream();
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        M.Close();
    }
}
