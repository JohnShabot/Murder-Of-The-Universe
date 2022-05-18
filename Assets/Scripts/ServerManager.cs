using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;
using System.Threading;

class State
{
    public byte[] buffer = new byte[1024];
}

public class ServerManager : MonoBehaviour
{
    Thread T;

    TcpClient M; // The socket that will connect to the main server
    Socket sock; // The Client for the actual game
    EndPoint ep;
    private AsyncCallback recv = null;
    State state = new State();

    bool? isServer = null; // the bool to keep if this is a host or client, or not in that phase (when it's null)

    string username = "";
    void Update()
    {
        if(isServer.HasValue)
        {
            if (isServer.Value)
            {
                string rcv = "";
                rcv = Receive();
                string[] data = rcv.Split('|');
                if(rcv != null)
                {
                    Debug.Log(data[0]);
                    if (data[0] == "JOIN")
                    {
                        ep = new IPEndPoint(IPAddress.Parse(data[1].Split("#")[1]), 11111);
                        Send("ACCEPT|" + username);
                        Debug.Log("Sent Message");
                    }
                }
            }
            else if (!isServer.Value)
            {
                string rcv = "";
                rcv = Receive();
                string[] data = rcv.Split('|');
                if (rcv != "")
                {
                    Debug.Log(data[0]);
                    if (data[0] == "ACCEPT")
                    {
                        Send("ACCEPT|" + username);
                        Debug.Log("Sent Message");
                    }
                }
            }
        }

        
    }
    public void Send(string text)
    {
        byte[] data = Encoding.ASCII.GetBytes(text);
        sock.BeginSendTo(data, 0, data.Length, SocketFlags.None, ep, (ar) =>
        {
            State so = (State)ar.AsyncState;
            int bytes = sock.EndSendTo(ar);
            //Debug.Log(string.Format("SEND: {0}, {1}", bytes, text));
        }, state);
    }
    string Receive()
    {
        sock.BeginReceiveFrom(state.buffer, 0, 1024, SocketFlags.None, ref ep, recv = (ar) =>
        {
            State so = (State)ar.AsyncState;
            int bytes = sock.EndReceiveFrom(ar, ref ep);
            sock.BeginReceiveFrom(so.buffer, 0, 1024, SocketFlags.None, ref ep, recv, so);
            //Debug.Log(string.Format("RECV: {0}: {1}, {2}", ep.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes)));
            state.buffer = so.buffer;
        }, state);
        string s = Encoding.ASCII.GetString(state.buffer);
        return s;
    }
    public void StartClient(string selfIP)
    {
        Debug.Log(ep);
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        Debug.Log("Connecting...");
        string data = "";
        Send("JOIN|" + username + "#" + selfIP);
        data = Receive();
        Debug.Log(data);
        isServer = false;
        Debug.Log("JOINED");
    }
    public void StartServ()
    {
        ep = new IPEndPoint(IPAddress.Any, 11000);
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        isServer = true;
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
        StartServ();
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
        string[] data = Encoding.ASCII.GetString(buffer).Split("#");
        Debug.Log(data[0]);
        Debug.Log(data[1]);

        sendData = Encoding.ASCII.GetBytes("CLOSE| "); // Turns data to bytes
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        M.Close(); // Closes the connection
        data[0] = data[0].Split('\'')[1];
        Debug.Log(data[0].ToString());
        data[1] = data[1].Split('\'')[1];
        Debug.Log(data[1]);
        ep = new IPEndPoint(IPAddress.Parse(data[0]), 11000);
        StartClient(data[1]);
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
