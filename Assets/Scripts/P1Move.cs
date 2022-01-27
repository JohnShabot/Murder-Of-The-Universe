using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1Move : MonoBehaviour
{
    Rigidbody2D body;
    Transform pos;

    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;
    int dirState; // 0 = up, 2 = down, 1 = right, 3 = left
    int prevDirState;

    public float runSpeed = 20.0f;
    public float bulletSpeed = 50.0f;

    public GameObject bullet;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        pos = GetComponent<Transform>();
        prevDirState = 0;
    }

    void Update()
    {
        movement();
    }

    private void FixedUpdate()
    {

        if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }
        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed); //moves the player
        shoot();

    }
    void movement()
    {
        horizontal = Input.GetAxisRaw("Horizontal"); // gets the input from the WASD keys
        vertical = Input.GetAxisRaw("Vertical");
        if (vertical > 0)// Check player direction state
        {
            dirState = 0;
        }
        else if (vertical < 0)
        {
            dirState = 2;
        }
        else if (horizontal > 0)
        {
            dirState = 1;
        }
        else if (horizontal < 0)
        {
            dirState = 3;
        }
        for (int i = 0; i < Mathf.Abs(prevDirState - dirState); i++) // rotate the player's image to the correct dir state
        {
            if (prevDirState - dirState > 0)
            {
                pos.Rotate(new Vector3(0, 0, 90));
            }
            else
            {
                pos.Rotate(new Vector3(0, 0, -90));
            }
        }
        prevDirState = dirState;
    }

    void shoot()
    {
        if(Input.GetKeyDown("space"))
        {

            Vector3 bulletDir = Input.mousePosition;
            Instantiate(bullet, new Vector3(pos.position.x + horizontal * 0.5f, pos.position.y *+ vertical * 0.5f, pos.position.z), Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(bulletDir.x * bulletSpeed, bulletDir.y * bulletSpeed);
        }
    }
}
