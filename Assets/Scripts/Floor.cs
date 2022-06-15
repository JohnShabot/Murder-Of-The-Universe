using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor
{
    [SerializeField]
    int floorNum;
    int[] difficulties = { 1, 2, 2, 3, 4 };

    [SerializeField]
    Wave[] waves;
    int isItem;
    string boss;

    bool itemTaken = true;
    bool itemCreated = false;
    bool bossSpawned = false;
    public bool FloorFinished { get; private set; }

    int currentWave = 0;
    public Floor(int floorNum, string boss)
    {
        FloorFinished = false;
        this.floorNum = floorNum;
        waves = new Wave[5];
        for(int i = 0; i< 5; i++)
        {
            waves[i] = new Wave(difficulties[i],floorNum);
        }
        isItem = Random.Range(1,5);
        this.boss = boss;
    }

    public void SpawnNextWave()
    {
        if (NetworkManager.instance.isServer.Value)
        {
            if (isItem == currentWave && !itemCreated)
            {
                itemTaken = false;
                int itID = Random.Range(0, GameManager.instance.ItemTypes.Count);
                GameManager.instance.SpawnItem(itID);
                ServerSend.spawnitem(itID);
                itemCreated = true;
            }
            else if (currentWave < 5 && itemTaken)
            {
                for (int i = 0; i < waves[currentWave].GetEnemies().Length; i++)
                {
                    ServerSend.spawnEnemy(waves[currentWave].GetEnemies()[i].Value, waves[currentWave].GetEnemies()[i].Key);
                    GameManager.instance.SpawnEnemy(waves[currentWave].GetEnemies()[i].Value, waves[currentWave].GetEnemies()[i].Key);
                }
                currentWave++;
            }
            else if (currentWave >= 5 && !bossSpawned)
            {
                SpawnBoss();
                bossSpawned = true;
            }
        }
    }

    public void setItemTaken(bool isTaken)
    {
        itemTaken = isTaken;
    }
    public void SpawnBoss()
    {
        Vector2 spawnPos = new Vector2(0, 0);
        switch (boss)
        {
            case "SlimeBoss":
                spawnPos = new Vector2(0, 0);
                break;
            case "Han-Tyumi":
                spawnPos = new Vector2(8, 5);
                break;
        }

        ServerSend.spawnEnemy(spawnPos, boss);
        GameManager.instance.SpawnBoss(boss, spawnPos);
    }
    public void BossKilled()
    {
        int itID = Random.Range(0, GameManager.instance.ItemTypes.Count);
        if(GameManager.instance.playerDead != -1)
        {
            GameManager.instance.RevivePlayer();
            ServerSend.RevivePlayer();
        }
        if (boss == "Han-Tyumi")
        {
            GameManager.instance.Win();
            ServerSend.Win();
        }
        else
        {
            GameManager.instance.SpawnItem(itID);
            ServerSend.spawnitem(itID);
            GameObject.Find("Elevator TF" + (floorNum + 1)).GetComponentInChildren<MoveToNewFloor>().Open();
            GameObject.Find("Elevator TF" + (floorNum + 1)).GetComponent<SpriteRenderer>().sprite = GameManager.instance.elevatorOpen;
            ServerSend.bossKilled(boss);
        }


    }
}
