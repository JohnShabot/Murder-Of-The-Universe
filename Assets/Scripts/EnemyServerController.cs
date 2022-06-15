using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyServerController : MonoBehaviour
{
    attackType at;

    float[] EStats; // HP, ATK, SPD, RNG

    int myID;
    void Start()
    {
       EStats = gameObject.GetComponent<EnemyStats>().stats;
    }
    public void UpdatePosRot(Vector2 position, float rotation)
    {
        Rigidbody2D body = this.GetComponent<Rigidbody2D>();
        body.position = position;
        body.rotation = rotation;
    }

    public void damage(float dmg)
    {
        Debug.Log("Enemy Took Damage: " + dmg);
        EStats[0] -= dmg;
        if (EStats[0] <= 0)
        {
            GameManager.instance.DestroyEnemy(myID);
            Destroy(gameObject);
            Debug.Log("Enemy Died");
        }
    }
    public void setID(int id)
    {
        myID = id;
    }
    public void OnDestroy()
    {
        Debug.Log("Enemy Destroyed");
    }
}
