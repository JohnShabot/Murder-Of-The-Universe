using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
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
                try
                {
                    col.gameObject.GetComponent<PlayerController>().damage(dmg);
                }
                catch
                {

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
