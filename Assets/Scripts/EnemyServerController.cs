using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyServerController : MonoBehaviour
{
    attackType at;

    float[] EStats; // HP, ATK, SPD, RNG

    int myID;


    GameObject bullet;
    Transform firePoint;
    void Start()
    {
        foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
        {
            if (t != transform)
            {
                firePoint = t;
            }
        }
        bullet = Resources.Load("Prefabs/Players/laserbulletEnemy.prefab") as GameObject;
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
    public void Shoot()
    {
        GameObject bull = Instantiate(bullet, firePoint.position, firePoint.rotation);
        Rigidbody2D bullbody = bull.GetComponent<Rigidbody2D>();
        bull.GetComponent<Bullet>().SetCameFrom(gameObject);
        bull.GetComponent<Bullet>().setDMG(EStats[1]);
        bullbody.AddForce(firePoint.up * 10, ForceMode2D.Impulse);
    }

    public void OnDestroy()
    {
        Debug.Log("Enemy Destroyed");
    }
}
