using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public Canvas[] screens;
    public GameObject LabelTemplate;
    public GameObject RoomsSection;

    List<GameObject> roomLabels = new List<GameObject>();

    private void Start()
    {
        foreach (Canvas s in screens)
        {
            if (s.name == "Title Screen") s.enabled = true;
            else s.enabled = false;
        }
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
        InputField[] texts = createRoom.GetComponentsInChildren<UnityEngine.UI.InputField>();
        name = texts[0];
        pass = texts[1];

        gameObject.GetComponent<ServerManager>().Host(name.text, pass.text);
        ChangeScreen("Room");
    }
    public void RefreshRooms()
    {
        int i = 0;
        while (roomLabels.ToArray().GetLength() > 0)
        {
            Destroy(roomLabels.ToArray()[i]);
            i++;
        }
        string roomsStr = gameObject.GetComponent<ServerManager>().RefreshList();
        string[] roomsLst = roomsStr.Split('\n');
        roomLabels = new List<GameObject>();
        int topY = 800;
        foreach (string room in roomsLst)
        {
            if(room != "")
            {
                Debug.Log(room);
                GameObject RoomLabel = Instantiate(LabelTemplate, new Vector3(385, topY, 0), new Quaternion(0,0,0,1), RoomsSection.transform);
                RoomLabel.GetComponentInChildren<Text>().text = room;
                topY -= 70;
                roomLabels.Add(RoomLabel);
            }
        }
    }
}
