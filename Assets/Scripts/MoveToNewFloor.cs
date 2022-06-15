using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToNewFloor : MonoBehaviour
{
    [SerializeField]
    public string floorTo;
    [SerializeField]
    int range;

    bool isOpen;

    public void Open()
    {
        isOpen = true;
    }
    void Update()
    {
        float minD = Vector2.Distance(transform.position, GameManager.instance.Players[0].transform.position);
        int id = 0;
        foreach (int i in GameManager.instance.Players.Keys)
        {
            float d = Vector2.Distance(transform.position, GameManager.instance.Players[i].transform.position);
            if (d < minD)
            {
                minD = d;
                id = i;
            }
        }
        if(minD < range && isOpen)
        {
            GameManager.instance.SendToNewFloor(floorTo);
        }
    }
    public void revivePlayer(int PID)
    {

    }

}
