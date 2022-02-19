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
    attackType at;

    float HP = 50f;
    float attack = 10f;
    public float speed = 3f;
    public float Range = 5f;

    float Distance;
    private float Wait;
    public float StartWait = 500f;
    int InvFrame = 0;
    bool firsthit = false;
    public Transform[] MoveSpots;

    private int randomSpots;

    void Start()
    {

        Wait = StartWait;
        randomSpots = Random.Range(0, MoveSpots.Length);
    }
    private void FixedUpdate()
    {
        InvFrame++;
    }
    void Update()
    {
        
        Distance = Vector2.Distance(transform.position, GameObject.FindWithTag("Player").transform.position);

        if (Distance < Range)
        {
            Chase();
        }

        else
        {
            PatrolNext();
        }
    }

    void PatrolNext()
    {
        transform.position = Vector2.MoveTowards(transform.position, MoveSpots[randomSpots].position, speed * Time.deltaTime); // moves the enemy the a desired pos //

        if (Vector2.Distance(transform.position, MoveSpots[randomSpots].position) < 0.2f)
        {
            if (Wait <= 0)
            {
                randomSpots = Random.Range(0, MoveSpots.Length);
                Wait = StartWait;
            }
            else Wait -= Time.deltaTime;
        }
    }


    void Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position, speed * Time.deltaTime);

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
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().damage(attack);
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
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().damage(attack);
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
    }
}
