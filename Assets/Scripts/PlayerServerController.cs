using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerServerController : MonoBehaviour
{
    Rigidbody2D body;
    Vector2 position;

    // Item List
    LinkedList<string> itemList;

    //Player Stats
    float[] PStats = { 100, 10, 5, 20, 10 };

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
        itemList = new LinkedList<string>();
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

    public void UpdatePosRot(Vector2 position, float rotation)
    {
        body.position = position;
        body.rotation = rotation;
    }

    public void AddItem(GameObject item)
    {
        if (alive)
        {
            GameManager.instance.DestroyItem(item);
            if (NetworkManager.instance.isServer.Value)
            {
                GameManager.instance.f.setItemTaken(true);
            }
            itemList.AddLast(item.GetComponent<ItemPickup>().item.itemName);
            for (int i = 0; i < 5; i++)
            {
                PStats[i] += item.GetComponent<ItemPickup>().item.getStatChange()[i];
            }
        }
    }
    public void Shoot()
    {
        if (alive)
        {
            GameObject bull = Instantiate(bullet, firePoint.position, firePoint.rotation);
            Rigidbody2D bullbody = bull.GetComponent<Rigidbody2D>();
            bull.GetComponent<Bullet>().SetCameFrom(gameObject);
            bull.GetComponent<Bullet>().setDMG(PStats[1]);
            bullbody.AddForce(firePoint.up * PStats[3], ForceMode2D.Impulse);
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
                if (!itemList.Contains("Totem"))
                {
                    Debug.Log("P1 Died");
                    alive = false;
                    gameObject.GetComponent<SpriteRenderer>().sprite = SDead;
                    foreach (int id in GameManager.instance.Players.Keys)
                    {
                        if (gameObject.Equals(GameManager.instance.Players[id]))
                        {
                            GameManager.instance.PlayerKilled(id);
                        }
                    }
                }
                else
                {
                    PStats[0] += 50;
                    PStats[1] += 20;
                    PStats[2] += 2;
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
    public void OnDestroy()
    {
        Debug.Log("Destroyed Player");
    }
}
