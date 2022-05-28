using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyServerController : MonoBehaviour
{
    attackType at;

    float HP = 50f;


    public void UpdatePosRot(Vector2 position, float rotation)
    {
        Rigidbody2D body = this.GetComponent<Rigidbody2D>();
        body.position = position;
        body.rotation = rotation;
    }

    public void damage(float dmg)
    {
        Debug.Log("Enemy Took Damage: " + dmg);
        HP -= dmg;
        if (HP <= 0)
        {
            Destroy(gameObject);
            Debug.Log("Enemy Died");
        }
    }
}
