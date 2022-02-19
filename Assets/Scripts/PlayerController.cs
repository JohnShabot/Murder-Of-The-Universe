﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // General Variables
    Camera cam;
    Rigidbody2D body;
    Vector2 position;
    Vector2 mousePos;
    bool isP1;

    // Item List
    LinkedList<Item> itemList;

    //Player Stats
    public float[] PStats = new float[5];

    //Scene Dependant Objects
    public Transform firePoint;
    public GameObject bullet;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        cam = Camera.FindObjectOfType<Camera>();
        itemList = new LinkedList<Item>();
    }

    void Update()
    {
        movement();
        shooting();
    }

    private void FixedUpdate()
    {

        body.MovePosition(body.position + position * PStats[2] * Time.fixedDeltaTime); //moves the player
        Vector2 lookDir = mousePos - body.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        body.rotation = angle;
    }

    void movement()
    {
        position.x = Input.GetAxisRaw("Horizontal"); // gets the input from the WASD keys
        position.y = Input.GetAxisRaw("Vertical");
    }
    void shooting()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject bull = Instantiate(bullet, firePoint.position, firePoint.rotation);
            Rigidbody2D bullbody = bull.GetComponent<Rigidbody2D>();
            bull.GetComponent<Bullet>().SetCameFrom(gameObject);
            bull.GetComponent<Bullet>().setDMG(PStats[1]);
            bullbody.AddForce(firePoint.up * PStats[3], ForceMode2D.Impulse);
        }
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }
    public void damage(float dmg)
    {
        PStats[0] -= dmg;
        Debug.Log("P1 Took Damage: " + dmg);
        if(PStats[0]<= 0)
        {
            Debug.Log("P1 Died");
        }
    }
    void OnTriggerEnter2D(Collider2D c)
    {
        Debug.Log("triggered");
        if (c.gameObject.tag == "Item")
        {
            Item it = c.gameObject.GetComponent<ItemPickup>().GetItem();
            Debug.Log("Gathered New Item");
            itemList.AddLast(it);
            GameObject.Destroy(c.gameObject);
            for (int i = 0; i < 5; i++)
            {
                this.PStats[i] += it.getStatChange()[i];
            }
        }
    }
}
