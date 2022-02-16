using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    public string itemName;
    public string description;
    public int[] statChange = new int[5]; //array to signify which stats to change [hp,dmg,spd,fire-rate,size]

    public abstract void SpecialItem(GameObject player);
    public abstract void ActiveAbility(GameObject player);
}
