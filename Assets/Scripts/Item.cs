using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Item
{
    public string itemName;
    [TextArea]
    public string description;
    public float[] statChange = new float[5]; //array to signify which stats to change [hp,dmg,spd,fire-rate,size]

    public abstract void SpecialItem(GameObject player);
    public abstract void ActiveAbility(GameObject player);
    public abstract float[] getStatChange();
}
