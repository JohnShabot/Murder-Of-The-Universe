using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Wave
{
    [SerializeField]
    KeyValuePair<string,Vector2>[] Enemies;

    [SerializeField]
    int difficulty;
    public Wave(int difficulty, int floorNum)
    {
        this.difficulty = difficulty;
        Enemies = new KeyValuePair<string, Vector2>[(int)(difficulty * 1.5f + 1)];
        for(int i = 0; i<Enemies.Length; i++)
        {
            Vector2 spawnPos = new Vector2(Random.Range(-2.5f, 19.6f), Random.Range(4, -5.5f));
            int r = floorNum*10 + Random.Range(1, 3);
            switch (r)
            {
                case 11:
                    Enemies[i] = new KeyValuePair<string, Vector2>("Slime Enemy 1", spawnPos);
                    break;
                case 12:
                    Enemies[i] = new KeyValuePair<string, Vector2>("Slime Enemy 1", spawnPos);
                    break;
                case 13:
                    Enemies[i] = new KeyValuePair<string, Vector2>("Slime Enemy 1", spawnPos);
                    break;
                case 21:
                    Enemies[i] = new KeyValuePair<string, Vector2>("NormalZombie", spawnPos);
                    break;
                case 22:
                    Enemies[i] = new KeyValuePair<string, Vector2>("FastZombie", spawnPos);
                    break;
                case 23:
                    Enemies[i] = new KeyValuePair<string, Vector2>("BuffedZombie", spawnPos);
                    break;

            }

        }
    }
    public KeyValuePair<string, Vector2>[] GetEnemies()
    {
        return this.Enemies;
    }
}
