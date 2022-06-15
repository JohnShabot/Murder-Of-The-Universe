using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Canvas[] screens;
    public GameObject LabelTemplate;
    public GameObject RoomsSection;
    public GameObject passwordPanel;

    string currRoom;
    private string UserName;
    List<GameObject> roomLabels;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Debug.Log("Instance Already Exists, Destroying");
            Destroy(this);
        }
    }
    private void Start()
    {
        passwordPanel.SetActive(false);
        foreach (Canvas s in screens)
        {
            if (s.name == "Title Screen") s.enabled = true;
            else s.enabled = false;
        }
        roomLabels = new List<GameObject>();
    }

    public void ReadyRecieved(string P)
    {
        GameObject P2 = GameObject.Find($"{P} Name");
        P2.GetComponent<Text>().text += "- READY!";
    }
    public void ReadyClick()
    {
        GameObject readyButton = GameObject.Find("Ready Button");
        readyButton.GetComponent<Button>().enabled = false;
        if (!NetworkManager.instance.isServer.Value)
        {
            ClientSend.ready();
            GameManager.instance.readyC();

        }
        else if (NetworkManager.instance.isServer.Value)
        {
            ServerSend.ready();
            GameManager.instance.readyS();
        }

    }
    public void ChangeScreen(string changeTo)
    {
        foreach(Canvas s in screens)
        {
            if (s.name == changeTo) s.enabled = true;
            else s.enabled = false;
        }
        if (changeTo == "Room Selector") RefreshRooms();
    }
    public void ExitGame()
    {
        NetworkManager.instance.CloseConnectionMain();
        Application.Quit();
    }

    public void HostActivation()
    {
        Canvas createRoom = screens[6];
        InputField name;
        InputField pass;

        foreach (Canvas s in screens)
        {
            if (s.name == "Room Create Screen") createRoom = s;
        }
        InputField[] texts = createRoom.GetComponentsInChildren<InputField>();
        name = texts[0];
        pass = texts[1];
        if(name.text != "")
        {
            if(pass.text != "") NetworkManager.instance.Host(name.text, pass.text, UserName);
            else NetworkManager.instance.Host(name.text, "", UserName);
            texts[0].text = "";
            texts[1].text = "";
            ChangeP1Name(UserName);
        }
        
    }
    public void ChangeP2Name(string PName)
    {
        GameObject P2 = GameObject.Find("P2 Name");
        P2.GetComponent<Text>().text = "P2: " + PName;
    }
    public void ChangeP1Name(string PName)
    {
        GameObject P1 = GameObject.Find("P1 Name");
        P1.GetComponent<Text>().text = "P1: " + PName;
    }
    public void RefreshRooms()
    {
        int i = 0;
        if(roomLabels.ToArray().Length > 0)
        {
            while (i < roomLabels.ToArray().Length)
            {
                Destroy(roomLabels.ToArray()[i]);
                i++;
            }
        }
        string roomsStr = NetworkManager.instance.RefreshList();
        string[] roomsLst = roomsStr.Split('#');
        roomLabels = new List<GameObject>();
        int topY = 800;
        foreach (string room in roomsLst)
        {
            if(room !="")
            {
                Debug.Log(room);
                GameObject RoomLabel = Instantiate(LabelTemplate, new Vector3(385, topY, 0), new Quaternion(0,0,0,1), RoomsSection.transform);
                RoomLabel.GetComponentInChildren<Text>().text = room;
                topY -= 70;
                RoomLabel.GetComponent<RoomPress>().setName(room);
                roomLabels.Add(RoomLabel);
            }
            else
            {
                break;
            }
        }
    }
    public void GetPassword(string name)
    {
        currRoom = name;
        passwordPanel.SetActive(true);
    }
    public void ConnectToRoom()
    {
        NetworkManager.instance.ConnectToHost(currRoom, passwordPanel.GetComponentInChildren<InputField>().text);
        ChangeScreen("Room");
        passwordPanel.GetComponentInChildren<InputField>().text = "";
        passwordPanel.SetActive(false);
        ChangeP2Name(NetworkManager.instance.username);
        
    }

    public void Register()
    {
        Canvas RegisterScreen = screens[3];

        InputField[] texts = RegisterScreen.GetComponentsInChildren<InputField>();
        GameObject lbl = GameObject.Find("Invalid Details SU");
        if (texts[2].text == texts[3].text)
        {
            NetworkManager.instance.Register(texts[0].text, texts[1].text, texts[2].text);
            ChangeScreen("Two Factor Auth");    
        }
        else
        {
            lbl.GetComponent<Text>().text = "Invalid Details";
        }
        texts[0].text = ""; //Revert The Input Fields
        texts[1].text = "";
        texts[2].text = "";
        texts[3].text = "";

    }
    public void Login()
    {
        Canvas LoginScreen = screens[2];
        InputField[] texts = LoginScreen.GetComponentsInChildren<InputField>();
        GameObject lbl = GameObject.Find("Invalid Details LI");
        bool s = NetworkManager.instance.Login(texts[0].text, texts[1].text);
        if (s)
        {
            UserName = texts[0].text;
            ChangeScreen("Two Factor Auth");
            texts[0].text = "";
            texts[1].text = "";
            lbl.GetComponent<Text>().text = "";
        }
        else
        {
            lbl.GetComponent<Text>().text = "Invalid Details";
        }

    }
    public void ConnectToMain()
    {
        Canvas LoginScreen = screens[0];
        InputField IPField = LoginScreen.GetComponentInChildren<InputField>();
        GameObject lbl = GameObject.Find("Invalid Details IP");
        bool s = NetworkManager.instance.ConnectToMain(IPField.text);
        if (s)
        {
            UserName = IPField.text;
            ChangeScreen("Login Screen");
            IPField.text = "";
            lbl.GetComponent<Text>().text = "";
        }
        else
        {
            lbl.GetComponent<Text>().text = "Invalid Server IP";
        }

    }
    public void verify()
    {
        Canvas verifyRoom = screens[4];
        InputField text = verifyRoom.GetComponentInChildren<InputField>();
        GameObject lbl = GameObject.Find("Invalid Details 2FA");
        int s = NetworkManager.instance.Verify(text.text);
        if (s == 1)
        {
            ChangeScreen("Room Selector");
            text.text = "";
            lbl.GetComponent<Text>().text = "";
        }
        else if (s < 0)
        {
            lbl.GetComponent<Text>().text = "Invalid Code, Try Again, Tries Left: " + s;
        }
        else if (s == 0)
        {
            GameObject Btn = GameObject.Find("Verify Button");
            Btn.SetActive(false);
        }
    }
}