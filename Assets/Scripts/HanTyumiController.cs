using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HanTyumiController : MonoBehaviour
{
    int myID;

    attackType at;

    float[] EStats; // HP, ATK, SPD, RNG
    private float wait;
    float startWait = 5;
    int InvFrame = 0;
    bool firsthit = false;

    private int randomSpots;

    void Start()
    {
        wait = startWait;
        EStats = gameObject.GetComponent<EnemyStats>().stats;
    }
    private void FixedUpdate()
    {
        InvFrame++;
    }
    void Update()
    {
        try
        {
            Rigidbody2D body = this.GetComponent<Rigidbody2D>();
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

            if (minD < EStats[3])
            {
                Chase(id);
            }

            else
            {
                SpawnCyborgs();
            }
            ServerSend.updateEnemyPos(myID, body.position, body.rotation);
        }
        catch
        {
            SpawnCyborgs();
        }
    }

    void SpawnCyborgs()
    {

        if (wait <= 0 && GameManager.instance.getCurrentEnemies().Count < 10)
        {
            GameManager.instance.SpawnEnemy(gameObject.transform.position, "Cyborg 1");
            GameManager.instance.SpawnEnemy(gameObject.transform.position, "Cyborg 2");
            GameManager.instance.SpawnEnemy(gameObject.transform.position, "Cyborg 3");
            ServerSend.spawnEnemy(gameObject.transform.position, "Cyborg 1");
            ServerSend.spawnEnemy(gameObject.transform.position, "Cyborg 2");
            ServerSend.spawnEnemy(gameObject.transform.position, "Cyborg 3");

            wait = startWait;
        }
        else wait -= Time.deltaTime;
    }


    void Chase(int id)
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(GameManager.instance.Players[id].transform.position.x, transform.position.y), EStats[2] * Time.deltaTime);
        Vector2 lookDir = GameManager.instance.Players[id].transform.position - transform.position; // calculates the vector from the enemy to the player
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg + 90f; // calculates the angle from the player to the mouse
        GetComponent<Rigidbody2D>().rotation = angle;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!firsthit)
        {
            firsthit = true;
            InvFrame = 100;
        }

        if (collision.collider.tag == "Player" && InvFrame >= 100)
        {
            if (collision.gameObject.Equals(GameManager.instance.Players[NetworkManager.instance.myId]))
                collision.gameObject.GetComponent<PlayerController>().damage(EStats[1]);
            InvFrame = 0;
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {

        if (!firsthit)
        {
            firsthit = true;
            InvFrame = 100;
        }
        if (collision.collider.tag == "Player" && InvFrame >= 100)
        {
            if (collision.gameObject.Equals(GameManager.instance.Players[NetworkManager.instance.myId]))
                collision.gameObject.GetComponent<PlayerController>().damage(EStats[1]);
            InvFrame = 0;
        }
    }
    public void damage(float dmg)
    {
        Debug.Log("Boss Took Damage: " + dmg);
        EStats[0] -= dmg;
        if (EStats[0] <= 0)
        {
            GameManager.instance.f.BossKilled();
            GameManager.instance.DestroyEnemy(myID);
            Destroy(gameObject);
            Debug.Log("Boss Died");
        }
        ServerSend.damageEnemy(myID, dmg);
    }

    public void setID(int id)
    {
        this.myID = id;
    }
}
