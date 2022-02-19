using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RegularItem : Item
{
    public override void ActiveAbility(GameObject player)
    {

    }
    public override void SpecialItem(GameObject player)
    {
        
    }
    public override float[] getStatChange()
    {
        return this.statChange;
    }
}
