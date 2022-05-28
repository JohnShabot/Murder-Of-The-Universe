using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum attackType
{
    melee,
    ranged_single,
    ranged_multiple,
    beam
}
public class EnemyController : MonoBehaviour
{
    int myID;

    attackType at;

    float HP = 50f;
    float attack = 10f;
    float speed = 3f;
    float Range = 5f;

    private float wait;
    float startWait = 3;
    int InvFrame = 0;
    bool firsthit = false;
    List<Transform> MoveSpots;

    private int randomSpots;

    void Start()
    {
        MoveSpots = new List<Transform>();
        foreach (Transform t in GameObject.Find("Enemy Patrol Points").GetComponentsInChildren<Transform>())
        {
            MoveSpots.Add(t);
        }
        wait = startWait;
        randomSpots = Random.Range(0, MoveSpots.ToArray().Length);
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

            if (minD < Range)
            {
                Chase(id);
            }

            else
            {
                PatrolNext();
            }
            ServerSend.updateEnemyPos(myID ,body.position, body.rotation);
        }
        catch
        {
            PatrolNext();
        }
    }

    void PatrolNext()
    {
        Debug.Log(wait);
        transform.position = Vector2.MoveTowards(transform.position, MoveSpots[randomSpots].position, speed * Time.deltaTime); // moves the enemy the a desired pos //
        Vector2 lookDir = MoveSpots[randomSpots].position - transform.position; // calculates the vector from the player to the mouse
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f; // calculates the angle from the player to the mouse
        this.GetComponent<Rigidbody2D>().rotation = angle;
        if (Vector2.Distance(transform.position, MoveSpots[randomSpots].position) < 0.2f)
        {
            if (wait <= 0)
            {
                randomSpots = Random.Range(0, MoveSpots.ToArray().Length);
                wait = startWait;
            }
            else wait -= Time.deltaTime;
        }
    }


    void Chase(int id)
    {
        transform.position = Vector2.MoveTowards(transform.position, GameManager.instance.Players[id].transform.position, speed * Time.deltaTime);
        Vector2 lookDir = GameManager.instance.Players[id].transform.position - transform.position; // calculates the vector from the player to the mouse
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f; // calculates the angle from the player to the mouse
        this.GetComponent<Rigidbody2D>().rotation = angle;
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
                collision.gameObject.GetComponent<PlayerController>().damage(attack);
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
            if(collision.gameObject.Equals(GameManager.instance.Players[NetworkManager.instance.myId]))
                collision.gameObject.GetComponent<PlayerController>().damage(attack);
            InvFrame = 0;
        }
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
        ServerSend.damageEnemy(myID,dmg);
    }

    public void setID(int id)
    {
        this.myID = id;
    }
}
