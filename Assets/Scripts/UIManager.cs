using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public Canvas[] screens;
    public GameObject LabelTemplate;
    public GameObject RoomsSection;
    public GameObject passwordPanel;

    string currRoom;
    private string UserName;
    List<GameObject> roomLabels;

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
    public void ChangeScreen(string changeTo)
    {
        foreach(Canvas s in screens)
        {
            if (s.name == changeTo) s.enabled = true;
            else s.enabled = false;
        }
    }
    public void ExitGame()
    {
        gameObject.GetComponent<ServerManager>().CloseConnection();
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
            if(pass.text != "") gameObject.GetComponent<ServerManager>().Host(name.text, pass.text, UserName);
            else gameObject.GetComponent<ServerManager>().Host(name.text, "", UserName);
            texts[0].text = "";
            texts[1].text = "";
            GameObject P1 = GameObject.Find("P1");
            P1.GetComponentInChildren<Text>().text = "P1: " + UserName;
            ChangeScreen("Room");
        }
        
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
        string roomsStr = gameObject.GetComponent<ServerManager>().RefreshList();
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
        gameObject.GetComponent<ServerManager>().ConnectToHost(currRoom, passwordPanel.GetComponentInChildren<InputField>().text);
        passwordPanel.GetComponentInChildren<InputField>().text = "";
        passwordPanel.SetActive(false);
    }

    public void Register()
    {
        Canvas RegisterScreen = screens[3];

        InputField[] texts = RegisterScreen.GetComponentsInChildren<InputField>();
        GameObject lbl = GameObject.Find("Invalid Details SU");
        if (texts[2].text == texts[3].text)
        {
            gameObject.GetComponent<ServerManager>().Register(texts[0].text, texts[1].text, texts[2].text);
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
        int s = gameObject.GetComponent<ServerManager>().Login(texts[0].text, texts[1].text);
        Debug.Log("func result: " + s);
        if (s == 1)
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
    public void verify()
    {
        Canvas verifyRoom = screens[4];
        InputField text = verifyRoom.GetComponentInChildren<InputField>();
        GameObject lbl = GameObject.Find("Invalid Details 2FA");
        int s = gameObject.GetComponent<ServerManager>().Verify(text.text);
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