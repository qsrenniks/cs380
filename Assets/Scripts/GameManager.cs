using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    public GameObject villageManagerPrefab;
    private VillageManager villageManager;
    private TimeManager.Timer repopulateTimer;

    public static GameManager Instance
    {
        get
        {
            instance = FindObjectOfType<GameManager>();
            if (instance == null)
            {
                GameObject container = new GameObject("GameManager");
                instance = container.AddComponent<GameManager>();
            }
            return instance;
        }
    }

    public int calculateModifier(int stat)
    {
        switch (stat)
        {
            case 1:
                return -5;
            case 2:
                return -4;
            case 3:
                return -4;
            case 4:
                return -3;
            case 5:
                return -3;
            case 6:
                return -2;
            case 7:
                return -2;
            case 8:
                return -1;
            case 9:
                return -1;
            case 10:
                return 0;
            case 11:
                return 0;
            case 12:
                return 1;
            case 13:
                return 1;
            case 14:
                return 2;
            case 15:
                return 2;
            case 16:
                return 3;
            case 17:
                return 3;
            case 18:
                return 4;
            case 19:
                return 4;
            case 20:
                return 5;
        }
        return -1;
    }

    public bool DifficultyClassCheck(int DC, int MOD)
    {
        Debug.Log("Difficulty Check");
        return DC <= Random.Range(1, 20) + MOD;
    }

    // Start is called before the first frame update
    void Start()
    {
        villageManager = Instantiate(villageManagerPrefab).GetComponent<VillageManager>();
        villageManager.CreateInitialPopulation();
        repopulateTimer = new TimeManager.Timer(10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        repopulateTimer.Update();
        if (repopulateTimer.isDone())
        {
            villageManager.Repopulate();
            villageManager.CullTheOldestGeneration();
            repopulateTimer.reset();
        }
    }
}
