using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerServerController : MonoBehaviour
{
    Rigidbody2D body;
    Vector2 position;

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
        itemList = new LinkedList<Item>();
        foreach (Transform t in this.GetComponentsInChildren<Transform>())
        {
            if (t != this.transform)
            {
                firePoint = t;
            }
        }
    }

    public void UpdatePosRot(Vector2 position, float rotation)
    {
        body.position = position;
        body.rotation = rotation;
    }

    public void AddItem(GameObject item)
    {
        if (!itemList.Contains(item.GetComponent<ItemPickup>().item))
        {
            GameManager.instance.DestroyItem(item);
            itemList.AddLast(item.GetComponent<ItemPickup>().item);
            for (int i = 0; i < 5; i++)
            {
                this.PStats[i] += item.GetComponent<ItemPickup>().item.getStatChange()[i];
            }
        }
    }
    public void RemoveItem(Item item)
    {
        if (itemList.Contains(item))
        {
            foreach(Item it in itemList)
            {
                if (it.Equals(item))
                {
                    itemList.Remove(it);
                    for (int i = 0; i < 5; i++)
                    {
                        this.PStats[i] -= item.getStatChange()[i];
                    }
                }
            }
        }
    }
    public void Shoot()
    {
        GameObject bull = Instantiate(bullet, firePoint.position, firePoint.rotation);
        Rigidbody2D bullbody = bull.GetComponent<Rigidbody2D>();
        bull.GetComponent<Bullet>().SetCameFrom(gameObject);
        bull.GetComponent<Bullet>().setDMG(PStats[1]);
        bullbody.AddForce(firePoint.up * PStats[3], ForceMode2D.Impulse);
    }

    public void damage(float dmg)
    {
        PStats[0] -= dmg;
        Debug.Log("P1 Took Damage: " + dmg);
        if (PStats[0] <= 0)
        {
            Debug.Log("P1 Died");
            Destroy(this.gameObject);
        }
    }
}
