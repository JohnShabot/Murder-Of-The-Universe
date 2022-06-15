using System.Collections;
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
    public float[] PStats = { 100, 10, 5, 20, 10 };

    //Scene Dependant Objects
    Transform firePoint;
    GameObject bullet;

    bool alive = true;
    Sprite SAlive;
    Sprite SDead;


    public void Initialize(GameObject bullPrefab)
    {
        bullet = bullPrefab;
    }
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        cam = GameManager.instance.cam;
        itemList = new LinkedList<Item>();
        foreach (Transform t in this.GetComponentsInChildren<Transform>())
        {
            if (t != this.transform)
            {
                firePoint = t;
            }
        }
        SAlive = gameObject.GetComponent<SpriteRenderer>().sprite;
        SDead = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 0, 0), new Vector2(0, 0));
    }

    void Update()
    {    
        movement();
        shooting();
        if (!gameObject.GetComponent<SpriteRenderer>().color.Equals(new Color(255, 255, 255)))
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(255, gameObject.GetComponent<SpriteRenderer>().color.g +1, gameObject.GetComponent<SpriteRenderer>().color.b+1);
        }
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
        if (Input.GetButtonDown("Fire1") && alive)
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
        if (alive)
        {
            PStats[0] -= dmg;
            Debug.Log("P1 Took Damage: " + dmg);
            if (PStats[0] <= 0)
            {
                Debug.Log("P1 Died");
                alive = false;
                gameObject.GetComponent<SpriteRenderer>().sprite = SDead;
                GameManager.instance.PlayerKilled(NetworkManager.instance.myId);
            }
            if (NetworkManager.instance.isServer.Value)
            {
                ServerSend.damagePlayer(0, dmg);
            }
            else if (!NetworkManager.instance.isServer.Value)
            {
                ClientSend.damagePlayer(dmg);
            }
            gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
        }

    }
    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.tag == "Item")
        { 
            if (alive)
            {
                Item it = c.gameObject.GetComponent<ItemPickup>().GetItem();
                if (NetworkManager.instance.isServer.Value)
                {
                    ServerSend.addItem(NetworkManager.instance.myId, GameManager.instance.getItemID(it));
                    GameManager.instance.f.setItemTaken(true);
                }
                else if (!NetworkManager.instance.isServer.Value)
                {
                    ClientSend.addItem(GameManager.instance.getItemID(it));
                }
                GameManager.instance.DestroyItem(c.gameObject);
                if (NetworkManager.instance.isServer.Value)
                {
                    GameManager.instance.f.setItemTaken(true);
                }
                itemList.AddLast(c.gameObject.GetComponent<ItemPickup>().item);
                for (int i = 0; i < 5; i++)
                {
                    PStats[i] += c.gameObject.GetComponent<ItemPickup>().item.getStatChange()[i];
                }
            }
        }
    }

    public void RevivePlayer()
    {
        alive = true;
        gameObject.GetComponent<SpriteRenderer>().sprite = SAlive;
        PStats[0] = 50;

    }
}
