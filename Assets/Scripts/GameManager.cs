using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject Player1Prefab;
    public GameObject Player2Prefab;
    public GameObject BulletPrefab;
    public Camera cam;
    Scene s;

    bool isReadyC = false;
    bool isReadyS = false;
    bool gameStarted = false;

    public Dictionary<int, GameObject> Players;

    [SerializeField]
    GameObject[] EnemyPrefabs;
    Dictionary<int, GameObject> CurrentEnemies;
    int nextID = 0;
    public Dictionary<string, GameObject> EnemyTypes { get; private set; }

    [SerializeField]
    public Floor f { get; private set; }
    int currentFloor = 0;
    public string floorName;

    public Sprite elevatorOpen;

    [SerializeField]
    GameObject[] ItemPrefabs;
    public Dictionary<int, GameObject> ItemTypes { get; private set; }
    GameObject CurrentItem;

    public int playerDead{get; private set;}

    private void Start()
    {
      foreach(GameObject e in EnemyPrefabs)
        {
            EnemyTypes[e.name] = e;
        }
        int i = 0;
        foreach (GameObject it in ItemPrefabs)
        {
            ItemTypes[i] = it;

            i++;
        }
        playerDead = -1;
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Debug.Log("Instance Already Exists, Destroying");
            Destroy(this);
        }
        EnemyTypes = new Dictionary<string, GameObject>();
        Players = new Dictionary<int, GameObject>();
        CurrentEnemies = new Dictionary<int, GameObject>();
        ItemTypes = new Dictionary<int, GameObject>();

    }

    void Update()
    {
        if (s.isLoaded)
        {
            if (!gameStarted)
            {
                gameStarted = true;
                SceneManager.SetActiveScene(s);
                GameObject.Find("TF1").GetComponentInChildren<MoveToNewFloor>().Open();

                GameObject P1 = Instantiate(Player1Prefab, new Vector3(5f, -2.55f, 0f), Quaternion.identity);
                Players.Add(0, P1);


                Debug.Log("Instantiated players");
                if (NetworkManager.instance.isServer.Value)
                {
                    Players[0].AddComponent<PlayerController>();
                    Players[0].GetComponent<PlayerController>().Initialize(BulletPrefab);

                    int[] ids = new int[ServerHost.clients.Keys.Count];
                    ServerHost.clients.Keys.CopyTo(ids, 0);
                    foreach (int i in ids)
                    {
                        GameObject P2 = Instantiate(Player2Prefab, new Vector3(7f, -2.55f, 0f), Quaternion.identity);
                        Players.Add(i, P2);
                        Players[i].AddComponent<PlayerServerController>();
                        Players[i].GetComponent<PlayerServerController>().Initialize(BulletPrefab);
                    }

                }
                else if (!NetworkManager.instance.isServer.Value)
                {
                    GameObject P2 = Instantiate(Player2Prefab, new Vector3(7f, -2.55f, 0f), Quaternion.identity);
                    Players.Add(1, P2);
                    Players[1].AddComponent<PlayerServerController>();
                    Players[1].GetComponent<PlayerServerController>().Initialize(BulletPrefab);
                    Players[0].AddComponent<PlayerServerController>();
                    Players[0].GetComponent<PlayerServerController>().Initialize(BulletPrefab);

                    Players[1].AddComponent<PlayerController>();
                    Players[1].GetComponent<PlayerController>().Initialize(BulletPrefab);
                }
            }
            if (NetworkManager.instance.isServer.Value)
            {
                cam.transform.position = new Vector3(Players[0].transform.position.x, Players[NetworkManager.instance.myId].transform.position.y, -1);
                if(currentFloor > 0 && f != null && CurrentEnemies.Count == 0)
                {
                    f.SpawnNextWave();
                }
            }
            else if (!NetworkManager.instance.isServer.Value)
            {
                cam.transform.position = new Vector3(Players[1].transform.position.x, Players[1].transform.position.y, -1);
            }

        }
    }
    public void readyC()
    {
        isReadyC = true;
        UIManager.instance.ReadyRecieved("P2");
        if (isReadyS)
        {
            
            startGame();
        }
    }
    public void readyS()
    {
        isReadyS = true;
        UIManager.instance.ReadyRecieved("P1");
         if (isReadyC)
         {
            startGame();
         }
    }
    private void startGame()
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(cam);
        s = SceneManager.LoadScene("Ground Floor", new LoadSceneParameters(LoadSceneMode.Single));
    }

    public void SpawnEnemy(Vector2 pos, string type)
    {
        Debug.Log("1");
        if (floorName == SceneManager.GetActiveScene().name)
        {
            Debug.Log("2 " + type);

            CurrentEnemies[nextID] = Instantiate(EnemyTypes[type], pos, Quaternion.identity);
            if (!NetworkManager.instance.isServer.Value)
            {
                Debug.Log("3");
                CurrentEnemies[nextID].AddComponent<EnemyServerController>();
                CurrentEnemies[nextID].GetComponent<EnemyServerController>().setID(nextID);
                Debug.Log("Spawned Enemy of Type: " + type + " with ID: " + nextID);
            }
            else
            {
                CurrentEnemies[nextID].AddComponent<EnemyController>();
                CurrentEnemies[nextID].GetComponent<EnemyController>().setID(nextID);

            }
            nextID++;
            Debug.Log("Spawned Enemy: " + (nextID-1));
        }
        else
        {
            StartCoroutine(SpawnEnemyNextFrame(pos, type));
        }
    }
    IEnumerator SpawnEnemyNextFrame(Vector2 pos, string type)
    {
        yield return 0;
        SpawnEnemy(pos, type);
    }

    public void SpawnItem(int type)
    {
        CurrentItem = Instantiate(ItemTypes[type], new Vector2(1.5f, 0), Quaternion.identity);
        Debug.Log("Item Spawned");
    }
    public int getItemID(Item it)
    {
        foreach (KeyValuePair<int, GameObject> i in ItemTypes)
        {
            if (i.Value.GetComponent<ItemPickup>().GetItem().itemName == it.itemName) return i.Key;
        }
        return -1;
    }

    public void DestroyEnemy(int id)
    {
        CurrentEnemies.Remove(id);
    }
    public void DestroyItem(GameObject it)
    {
        Debug.Log(CurrentItem.name);
        Debug.Log(CurrentItem.GetComponent<ItemPickup>().GetItem().itemName);
        if(CurrentItem.GetComponent<ItemPickup>().GetItem().itemName == it.GetComponent<ItemPickup>().GetItem().itemName)
        {
            Debug.Log("Item Destroyed");
            Destroy(CurrentItem);
        }
        CurrentItem = null;
    }
    public Dictionary<int,GameObject> getCurrentEnemies()
    {
        return CurrentEnemies;
    }

    public void SendToNewFloor(string floorTo)
    {
        currentFloor++;
        if(currentFloor == 1)
        {
            DontDestroyOnLoad(Players[0]);
            foreach(int PID in Players.Keys)
            {
                DontDestroyOnLoad(Players[PID]);

            }
        }
        s = SceneManager.LoadScene(floorTo, new LoadSceneParameters(LoadSceneMode.Single));
        if (NetworkManager.instance.isServer.Value)
        {
            StartCoroutine(spawnWaveOnNextFrame());
        }
        floorName = floorTo;
        Debug.Log("Moved To new Floor - " + currentFloor);
    }
    IEnumerator spawnWaveOnNextFrame()
    {
        yield return 0;
        if (currentFloor == 1) f = new Floor(currentFloor, "SlimeBoss");
        if (currentFloor == 2) f = new Floor(currentFloor, "Han-Tyumi");
        f.SpawnNextWave();
    }

    public void SpawnBoss(string boss, Vector2 spawnPos)
    {
        Debug.Log(nextID);
        CurrentEnemies.Add(nextID, Instantiate(EnemyTypes[boss], spawnPos, Quaternion.identity));
        if(boss == "SlimeBoss")
        {
            CurrentEnemies[nextID].AddComponent<SlimeBossController>();
            CurrentEnemies[nextID].GetComponent<SlimeBossController>().setID(nextID);
            CurrentEnemies[nextID].tag = "SlimeBoss";
        }
        if (boss == "Han-Tyumi")
        {
            CurrentEnemies[nextID].AddComponent<HanTyumiController>();
            CurrentEnemies[nextID].GetComponent<HanTyumiController>().setID(nextID);
            CurrentEnemies[nextID].tag = "HanTyumi";
        }
        nextID++;
    }

    public void PlayerKilled(int id)
    {
        if(playerDead == -1)
        {
            playerDead = id;
        }
        else if(playerDead != id)
        {
            //Send lose
        }
    }
    public void RevivePlayer()
    {
        try
        {
            Players[playerDead].GetComponent<PlayerServerController>().RevivePlayer();
        }
        catch
        {
            Players[playerDead].GetComponent<PlayerController>().RevivePlayer();
        }
    }
    public void BossKilled(string bossName)
    {
        GameObject.Find("Elevator TF" + (currentFloor + 1)).GetComponentInChildren<MoveToNewFloor>().Open();
        GameObject.Find("Elevator TF" + (currentFloor + 1)).GetComponent<SpriteRenderer>().sprite = elevatorOpen;
        if (bossName == "Han-Tyumi")
        {
            Win();
        }
    }
    public void Win()
    {
        int[] ids = new int[Players.Keys.Count];
        Players.Keys.CopyTo(ids, 0);
        Debug.Log("You Win");
        if(NetworkManager.instance.isServer.Value) NetworkManager.instance.Win(ids);
        foreach(int id in Players.Keys)
        {
            Destroy(Players[id]);
            Players = new Dictionary<int, GameObject>();
        }
        SceneManager.LoadScene("Title Screen", new LoadSceneParameters(LoadSceneMode.Single));
        UIManager.instance.ChangeScreen("Room Selector");
    }
    public void Lose()
    {
        Debug.Log("You Lose");
        foreach (int id in Players.Keys)
        {
            Destroy(Players[id]);
        }
        SceneManager.LoadScene("Title Screen", new LoadSceneParameters(LoadSceneMode.Single));
    }
}
