using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float dmg=10f;
    GameObject cameFrom;

    public void setDMG(float dmg)
    {
        this.dmg = dmg;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.Equals(cameFrom))
        {
            return;
        }
        switch(col.tag)
        {
            case "Player":
                Debug.Log("Don't Hit ME");
                break;
            case "Enemy":
                if (NetworkManager.instance.isServer.Value)
                {
                    col.gameObject.GetComponent<EnemyController>().damage(dmg);
                }
                break;
        }
        Destroy(gameObject);
    }
    public void SetCameFrom(GameObject cameFrom)
    {
        this.cameFrom = cameFrom;
    }
}
