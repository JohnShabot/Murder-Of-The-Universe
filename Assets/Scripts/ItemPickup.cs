﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    //Item Properties
    public RegularItem item = new RegularItem();
    
    public RegularItem GetItem()
    {
        return item;
    }
}
