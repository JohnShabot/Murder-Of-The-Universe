using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPress : MonoBehaviour
{
    string rname;
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
        Managers.GetComponent<UIManager>().GetPassword(rname);
    }
    public void setName(string name)
    {
        this.rname = name;
    }
}
