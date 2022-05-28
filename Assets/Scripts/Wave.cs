using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Wave
{
    KeyValuePair<string,Vector2>[] Enemies;
    public Dictionary<int, GameObject> CurrentEnemies { get; private set; }
    int difficulty;
    public Wave()
    {
        CurrentEnemies = new Dictionary<int, GameObject>();
    }
}
