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
    bool isP1;

    // Item List
    LinkedList<Item> itemList;

    //Player Stats
    float runSpeed = 2.5f;
    float bulletForce = 10f;
    float HP = 100f;

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

        body.MovePosition(body.position + position * runSpeed * Time.fixedDeltaTime); //moves the player
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
            bullbody.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
        }
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }
    public void damage(float dmg)
    {
        HP -= dmg;
        Debug.Log("Took Damage: " + dmg);
        if(HP<= 0)
        {
            Debug.Log("P1 Died");
        }
    }
    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.tag == "Item")
        {
            itemList.AddLast(c.gameObject.GetComponent<ItemPickup>().item);
            GameObject.Destroy(c.gameObject);
        }
    }
}
