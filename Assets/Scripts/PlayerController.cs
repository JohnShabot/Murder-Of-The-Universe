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

    // Item List
    LinkedList<Item> itemList;

    //Player Stats
    public float[] PStats = new float[5];

    //Scene Dependant Objects
    Transform firePoint;
    GameObject bullet;

    public void Initialize(GameObject bullPrefab)
    {
        bullet = bullPrefab;
    }
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        cam = Camera.FindObjectOfType<Camera>();
        itemList = new LinkedList<Item>();
        foreach (Transform t in this.GetComponentsInChildren<Transform>())
        {
            if (t != this.transform)
            {
                firePoint = t;
            }
        }
    }

    void Update()
    {    
        movement();
        shooting();
    }

    private void FixedUpdate()
    {
        body.MovePosition(body.position + position * PStats[2] * Time.fixedDeltaTime); //moves the player 
        if (NetworkManager.instance.isServer.Value)
        {
            ServerSend.updatePosRot(body.position, body.rotation);
        }
        else if (!NetworkManager.instance.isServer.Value)
        {
            ClientSend.updatePosRot(body.position, body.rotation);
        }
    }

    void movement()
    {
        Vector2 newPos = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); // gets the movement of the player from WASD
        Vector2 newMousePos = cam.ScreenToWorldPoint(Input.mousePosition); // gets the mouse position


        // let server know about position and rotation client changes
        if (newPos != position || mousePos != newMousePos) 
        {
            mousePos = newMousePos;
            Vector2 lookDir = mousePos - body.position; // calculates the vector from the player to the mouse
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f; // calculates the angle from the player to the mouse
            body.rotation = angle;
            position = newPos;
        }
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
            if (NetworkManager.instance.isServer.Value)
            {
                ServerSend.shoot();
            }
            else if (!NetworkManager.instance.isServer.Value)
            {
                ClientSend.shoot();
            }
        }
    }


    public void damage(float dmg)
    {
        PStats[0] -= dmg;
        Debug.Log("P1 Took Damage: " + dmg);
        if(PStats[0]<= 0)
        {
            Debug.Log("P1 Died");
            GameManager.instance.Players.Remove(NetworkManager.instance.myId);
            Destroy(this.gameObject);
        }
        if (NetworkManager.instance.isServer.Value)
        {
            ServerSend.damagePlayer(0, dmg);
        }
        else if (!NetworkManager.instance.isServer.Value)
        {
            ClientSend.damagePlayer(dmg);
        }
    }
    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.tag == "Item")
        {
            Item it = c.gameObject.GetComponent<ItemPickup>().GetItem();
            if (NetworkManager.instance.isServer.Value) ServerSend.addItem(NetworkManager.instance.myId, GameManager.instance.getItemID(it));
            if (!NetworkManager.instance.isServer.Value) ClientSend.addItem(GameManager.instance.getItemID(it));

            Debug.Log("Gathered New Item");
            itemList.AddLast(it);
            GameManager.instance.DestroyItem(c.gameObject);
            for (int i = 0; i < 5; i++)
            {
                PStats[i] += it.getStatChange()[i];
            }
        }
    }
    void OnDestroy()
    {
        Debug.Log("Destroying Player");
    }
}
