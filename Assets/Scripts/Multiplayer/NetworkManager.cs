using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    TcpClient M; // The socket that will connect to the main server
    static UdpClient P2P; //The UDP socket for the game
    static IPEndPoint ep;

    public bool? isServer = null; // the bool to keep if this is a host or client, or not in that phase (when it's null)
    static bool messageReceived;
    public string username = "";

    public static NetworkManager instance;

    public static int bufferSize = 1024;
    public string remoteIP;
    public int port = 11111;
    public int myId = 0;
    public TCP tcp;
    public UDP udp;

    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    // This function is called before the very first frame is called
    private void Awake()
    {
        if (instance == null) instance = this;
        else if(instance != this)
        {
            Debug.Log("Instance Already Exists, Destroying");
            Destroy(this);
        }
    }

    public void StartClient(string remoteIP, string selfIP)
    {
        this.remoteIP = remoteIP;
        tcp = new TCP();
        udp = new UDP();
        InitializeClientData();
        tcp.Connect();
        isServer = false;
    }
    public void StartServer()
    {
        isServer = true;
        myId = 0;
        ServerHost.Start(1, 11111);
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.welcome },
            { (int)ServerPackets.udpTest, ClientHandle.udpTest },
            { (int)ServerPackets.ready, ClientHandle.ready },
            { (int)ServerPackets.updatePosRot, ClientHandle.updatePosRot },
            { (int)ServerPackets.shoot, ClientHandle.shoot },
            { (int)ServerPackets.spawnEnemy, ClientHandle.spawnEnemy },
            { (int)ServerPackets.updateEnemyPos, ClientHandle.updateEnemyPos },
            { (int)ServerPackets.damageEnemy, ClientHandle.damageEnemy },
            { (int)ServerPackets.damagePlayer, ClientHandle.damagePlayer },
            { (int)ServerPackets.spawnItem, ClientHandle.spawnItem },
            { (int)ServerPackets.addItem, ClientHandle.addItem },
            { (int)ServerPackets.bossKilled, ClientHandle.bossKilled },
            { (int)ServerPackets.revivePlayer, ClientHandle.RevivePlayer },
            { (int)ServerPackets.lose, ClientHandle.Lose },
            { (int)ServerPackets.win, ClientHandle.Win },


        };
        Debug.Log("initialized Packets");
    }

    #region MainServerConnection
    public void Win(int[] ids)
    {
        string s = "";
        foreach(int i in ids)
        {
            s += i + "#";
        }
        byte[] sendData = Encoding.ASCII.GetBytes("WIN|" + s); // Turns data to bytes
        NetworkStream stream = M.GetStream();
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        byte[] buffer = new byte[1024]; // The variable that will store the recieved data
        stream.Read(buffer, 0, 1024); // Read the data
        string data = Encoding.ASCII.GetString(buffer); // Turn the data into a string
        Debug.Log(data);
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
        StartServer();
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
        string[] ips = ip.Split("#");
        Debug.Log(ips[0]);
        Debug.Log(ips[1]);

        sendData = Encoding.ASCII.GetBytes("CLOSE| "); // Turns data to bytes
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        M.Close(); // Closes the connection
        ips[0] = ips[0].Split('\'')[1];
        ips[1] = ips[1].Split('\'')[1];
        StartClient(ips[0], ips[1]);
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
    public bool Login(string name, string pass)
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
        if(int.Parse(data) == 0)
        {
            return false;
        }
        else
        {
            myId = int.Parse(data);
            return true;
        }
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


    public bool ConnectToMain(string ip)
    {
        try
        {
            //if(ip == "")
            //{
            //    return false;
            //}
            M = new TcpClient(ip, 8888);  // Connects to server
        }
        catch
        {
            return false;
        }
        byte[] sendData = Encoding.ASCII.GetBytes("START| "); // Turns data to bytes
        NetworkStream stream = M.GetStream();
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        byte[] buffer = new byte[1024];
        stream.Read(buffer, 0, 1024);
        string data = Encoding.ASCII.GetString(buffer);
        Debug.Log(data);
        return true;
    }
    public void CloseConnectionMain()
    {
        byte[] sendData = Encoding.ASCII.GetBytes("Close| "); // Turns data to bytes
        NetworkStream stream = M.GetStream();
        stream.Write(sendData, 0, sendData.Length); // Sends the data
        M.Close();
    }
    #endregion

    #region TCP Class
    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet recievedData;
        private byte[] buffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = bufferSize,
                SendBufferSize = bufferSize
            };

            buffer = new byte[bufferSize];
            socket.BeginConnect(instance.remoteIP, instance.port, ConnectCallback, socket);
        }
        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);
            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();
            recievedData = new Packet();
            stream.BeginRead(buffer, 0, bufferSize, RecieveCallBack, null);
        }

        public void sendData(Packet packet)
        {
            try
            {
                if(socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch(Exception ex)
            {
                Debug.Log($"Error sending data to the server via TCP: {ex}");
            }
        }

        private void RecieveCallBack(IAsyncResult _result)
        {
            try
            {
                int byteLength = stream.EndRead(_result);
                if (byteLength <= 0)
                {
                    return;
                }
                byte[] data = new byte[byteLength];
                Array.Copy(buffer, data, byteLength);

                recievedData.Reset(HandleData(data));
                stream.BeginRead(buffer, 0, bufferSize, RecieveCallBack, null);
            }
            catch (Exception ex)
            {
                Debug.Log($"Error recieving TCP data: {ex}");
            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLength = 0;

            recievedData.SetBytes(data);

            if (recievedData.UnreadLength() >= 4)
            {
                packetLength = recievedData.ReadInt();
                if(packetLength <= 0)
                {
                    return true;
                }
            }

            while(packetLength > 0 && packetLength <= recievedData.UnreadLength())
            {
                byte[] packetbytes = recievedData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetbytes))
                    {
                        int _packetId = packet.ReadInt();
                        packetHandlers[_packetId](packet);
                    }
                });

                packetLength = 0;
                if (recievedData.UnreadLength() >= 4)
                {
                    packetLength = recievedData.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }
            }
            if(packetLength <= 1)
            {
                return true;
            }
            return false;
        }
    }
    #endregion

    #region UDP Class
    public class UDP
    {
        public UdpClient socket;
        IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.remoteIP), instance.port);
        }

        public void Connect(int localPort)
        {
            socket = new UdpClient(localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(RecieveCallback, null);
            using(Packet packet = new Packet())
            {
                sendData(packet);
            }
        }
        public void sendData(Packet packet)
        {
            try
            {
                packet.InsertInt(instance.myId);
                if(socket != null)
                {
                    socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
                }
            }
            catch(Exception ex)
            {
                Debug.Log($"Error sending message to server via UDP: {ex}");
            }
        }
        private void RecieveCallback(IAsyncResult result)
        {
            try
            {
                byte[] data = socket.EndReceive(result, ref endPoint);
                socket.BeginReceive(RecieveCallback, null);

                if(data.Length < 4)
                {
                    return;
                }

                HandleData(data);
            }
            catch(Exception ex)
            {
                Debug.Log($"Error recieving message from server via UDP: {ex}");
            }
        }
        private void HandleData(byte[] data)
        {
            using(Packet packet = new Packet(data))
            {
                int packetLength = packet.ReadInt();
                data = packet.ReadBytes(packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(data))
                {
                    int packetID = packet.ReadInt();
                    packetHandlers[packetID](packet);
                }
            });
        }
    }
        #endregion
}
