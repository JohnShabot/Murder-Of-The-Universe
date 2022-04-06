using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPress : MonoBehaviour
{
    string name;
    // Start is called before the first frame update
    void Start()
    {
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnClick()
    {
        GameObject Managers = GameObject.FindGameObjectWithTag("Manager");
        Managers.GetComponent<UIManager>().GetPassword(name);
    }
    public void setName(string name)
    {
        this.name = name;
    }
}
