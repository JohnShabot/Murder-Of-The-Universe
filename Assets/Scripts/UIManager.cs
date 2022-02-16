using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Canvas[] screens;

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
        UnityEngine.UI.InputField name;
        UnityEngine.UI.InputField pass;

        foreach (Canvas s in screens)
        {
            if (s.name == "Room Create Screen") createRoom = s;
        }
        UnityEngine.UI.InputField[] texts = createRoom.GetComponentsInChildren<UnityEngine.UI.InputField>();
        name = texts[0];
        pass = texts[1];

        gameObject.GetComponent<ServerManager>().Host(name.text, pass.text);
        ChangeScreen("Room");
    }
}
