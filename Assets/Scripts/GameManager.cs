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
    GameObject[] ItemPrefabs;
    public Dictionary<int, GameObject> ItemTypes { get; private set; }
    GameObject CurrentItem;

    private void Start()
    {
      foreach(GameObject e in EnemyPrefabs)
        {
            EnemyTypes[e.name] = e;
            Debug.Log(e.name);
        }
        int i = 0;
        foreach (GameObject it in ItemPrefabs)
        {
            Debug.Log(it.name);
            ItemTypes[i] = it;

            i++;
        }
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
                GameObject P1 = Instantiate(Player1Prefab, new Vector3(5f, -2.55f, 0f), Quaternion.identity);

                Players.Add(0, P1);
                GameObject P2 = Instantiate(Player2Prefab, new Vector3(7f, -2.55f, 0f), Quaternion.identity);
                Players.Add(1, P2);
                Debug.Log("Instantiated players");
                if (NetworkManager.instance.isServer.Value)
                {
                    Players[0].AddComponent<PlayerController>();
                    Players[0].GetComponent<PlayerController>().Initialize(BulletPrefab);

                    Players[0].GetComponent<PlayerController>().PStats[0] = 100;
                    Players[0].GetComponent<PlayerController>().PStats[1] = 10;
                    Players[0].GetComponent<PlayerController>().PStats[2] = 5;
                    Players[0].GetComponent<PlayerController>().PStats[3] = 20;
                    Players[0].GetComponent<PlayerController>().PStats[4] = 10;

                    Players[1].AddComponent<PlayerServerController>();
                    Players[1].GetComponent<PlayerServerController>().Initialize(BulletPrefab);

                    Players[1].GetComponent<PlayerServerController>().PStats[0] = 100;
                    Players[1].GetComponent<PlayerServerController>().PStats[1] = 10;
                    Players[1].GetComponent<PlayerServerController>().PStats[2] = 5;
                    Players[1].GetComponent<PlayerServerController>().PStats[3] = 20;
                    Players[1].GetComponent<PlayerServerController>().PStats[4] = 10;

                    ServerSend.spawnEnemy(new Vector2(10, 1.5f), "FastZombie");
                    CurrentEnemies.Add(nextID, Instantiate(EnemyTypes["FastZombie"], new Vector2(10, 1.5f), Quaternion.identity));
                    CurrentEnemies[nextID].AddComponent<EnemyController>();
                    CurrentEnemies[nextID].GetComponent<EnemyController>().setID(nextID);
                    nextID++;
                    SpawnItem(0);
                    ServerSend.spawnitem(0);
                }
                else if (!NetworkManager.instance.isServer.Value)
                {
                    Players[0].AddComponent<PlayerServerController>();
                    Players[0].GetComponent<PlayerServerController>().Initialize(BulletPrefab);

                    Players[0].GetComponent<PlayerServerController>().PStats[0] = 100;
                    Players[0].GetComponent<PlayerServerController>().PStats[1] = 10;
                    Players[0].GetComponent<PlayerServerController>().PStats[2] = 5;
                    Players[0].GetComponent<PlayerServerController>().PStats[3] = 20;
                    Players[0].GetComponent<PlayerServerController>().PStats[4] = 10;

                    Players[1].AddComponent<PlayerController>();
                    Players[1].GetComponent<PlayerController>().Initialize(BulletPrefab);

                    Players[1].GetComponent<PlayerController>().PStats[0] = 100;
                    Players[1].GetComponent<PlayerController>().PStats[1] = 10;
                    Players[1].GetComponent<PlayerController>().PStats[2] = 5;
                    Players[1].GetComponent<PlayerController>().PStats[3] = 20;
                    Players[1].GetComponent<PlayerController>().PStats[4] = 10;
                }
            }
            if (NetworkManager.instance.isServer.Value)
            {
                cam.transform.position = new Vector3(Players[0].transform.position.x, Players[NetworkManager.instance.myId].transform.position.y, -1);

            }
            else if (!NetworkManager.instance.isServer.Value)
            {
                cam.transform.position = new Vector3(Players[NetworkManager.instance.myId].transform.position.x, Players[NetworkManager.instance.myId].transform.position.y, -1);

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
        Scene thisScene = SceneManager.GetActiveScene();
        s = SceneManager.LoadScene("SampleScene", new LoadSceneParameters(LoadSceneMode.Single));
    }

    public void SpawnEnemy(Vector2 pos, string type)
    {
        CurrentEnemies[nextID] = Instantiate(EnemyTypes[type], pos, Quaternion.identity);
        CurrentEnemies[nextID].AddComponent<EnemyServerController>();
        nextID++;
    }
    public void SpawnItem(int type)
    {
        CurrentItem = Instantiate(ItemTypes[type], new Vector2(1.5f, 0), Quaternion.identity);
    }
    public void DestroyEnemy(int id)
    {
        CurrentEnemies.Remove(id);
    }
    public void DestroyItem(GameObject it)
    {
        if(CurrentItem.GetComponent<ItemPickup>().GetItem().itemName == it.GetComponent<ItemPickup>().GetItem().itemName) Destroy(CurrentItem);
        CurrentItem = null;
    }
    public Dictionary<int,GameObject> getCurrentEnemies()
    {
        return CurrentEnemies;
    }
    public int getItemID(Item it)
    {
        foreach(KeyValuePair<int,GameObject> i in ItemTypes)
        {
            if (i.Value.GetComponent<ItemPickup>().GetItem().itemName == it.itemName) return i.Key;
        }
        return -1;
    }
}
