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

    Scene s;

    bool isReadyC = false;
    bool isReadyS = false;
    bool gameStarted = false;

    public Dictionary<int, GameObject> Players;
    public Dictionary<int, GameObject> EnemyTypes;

    List<GameObject> CurrentEnemies;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Debug.Log("Instance Already Exists, Destroying");
            Destroy(this);
        }
        Players = new Dictionary<int, GameObject>();
    }

    void Update()
    {
        if (s.isLoaded && gameStarted == false)
        {
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
            gameStarted = true;
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
        Scene thisScene = SceneManager.GetActiveScene();
        s = SceneManager.LoadScene("SampleScene", new LoadSceneParameters(LoadSceneMode.Single));
    }
    public void SpawnEnemy(Vector2 pos, int type)
    {

    }
}
