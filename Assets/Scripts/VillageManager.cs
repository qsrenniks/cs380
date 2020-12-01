using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class VillageManager : MonoBehaviour
{
    private static VillageManager instance = null;
    public int initialPopulaton = 10;
    public Tile purple;

    public List<GameObject> population;
    public GameObject villagerPrefab;
    public int mutationRate;
    public enum E_JOB {WOODCUTTER = 0, FOODGATHER, WATERCOLLECT, COUNT}
    public int[] jobCounter;
    public GameObject terrainManager;

    public (int, int) homeTile;

    public HashSet<(int, int)> knownForrests;
    public HashSet<(int, int)> knownFoodSources;
    public HashSet<(int, int)> knownWaterSources;

    public static VillageManager Instance
    {
        get
        {
            instance = FindObjectOfType<VillageManager>();
            if (instance == null)
            {
                GameObject container = new GameObject("VillageManager");
                instance = container.AddComponent<VillageManager>();
            }
            return instance;
        }
    }

    public float chanceOfDeathFunction(int age)
    {
        return (float)((0.5f * age) * (0.5f * age));
    }

    private int statDistrobutionFunction()
    {
        int positiveOrNegative = Random.Range(0, 1);
        float randomRoll = Random.Range(10, 100) / 100.0f;
        int newStat;
        if (positiveOrNegative == 1)
        {
            newStat = (int)(6.59023520066 * Mathf.Sqrt(-Mathf.Log(randomRoll)) + 10);
        }
        else
        {
            newStat = (int)(-6.59023520066 * Mathf.Sqrt(-Mathf.Log(randomRoll)) + 10);
        }
        return newStat;
    }

    private int randomStat()
    {
        return Random.Range(1, 6) + Random.Range(1, 6) + Random.Range(1, 6);
    }
    public void CullTheOldestGeneration()
    {
        Debug.Log("Culling");
        int oldestGeneration = 2147483647; // max int
        for (int i = 0; i < population.Count; i++)
        {
            VillagerBaseBehavior villagerData = population[i].GetComponent<VillagerBaseBehavior>();
            if(villagerData.generation < oldestGeneration)
            {
                oldestGeneration = villagerData.generation;
            }
        }
        List<GameObject> newList = new List<GameObject>();
        for (int i = 0; i < population.Count; i++)
        {
            VillagerBaseBehavior villagerData = population[i].GetComponent<VillagerBaseBehavior>();
            if (villagerData.generation == oldestGeneration)
            {
                if(villagerData.partner != null)
                {
                    villagerData.partner.GetComponent<VillagerBaseBehavior>().partner = null;
                }
                Destroy(population[i]);
            }
            else
            {
                newList.Add(population[i]);
            }
        }
        population = newList;
    }

    public void CreateInitialPopulation()
    {
        Debug.Log("Initial Creation");
        for (int i = 0; i < initialPopulaton; i++)
        {
            GameObject newVillager = Instantiate(villagerPrefab);
            VillagerBaseBehavior villagerData = newVillager.GetComponent<VillagerBaseBehavior>();
            for (int j = 0; j < villagerData.statArray.Length; j++)
            {
                int newStat = randomStat();
                villagerData.statArray[j] = newStat;
                villagerData.statModArray[j] = GameManager.Instance.calculateModifier(newStat);
                villagerData.dateOfBirth[0] = Random.Range(1, 12);
                villagerData.dateOfBirth[1] = Random.Range(1, 30);
                villagerData.dateOfBirth[2] = TimeManager.Instance.currentYear - Random.Range(18, 30);
            }
            population.Add(newVillager);
        }

        // find a starting spot for population
        TerrainGenerator terrainGenerator = terrainManager.GetComponent(typeof(TerrainGenerator)) as TerrainGenerator;
        int randomX = Random.Range(0, terrainGenerator.mapSize - 1);
        int randomY = Random.Range(0, terrainGenerator.mapSize - 1);
        while (!terrainGenerator.IsLand(randomX, randomY))
        {
            randomX = Random.Range(0, terrainGenerator.mapSize - 1);
            randomY = Random.Range(0, terrainGenerator.mapSize - 1);
        }
        terrainGenerator.populaceLayer.setTileIntensity(randomX, randomY, 1.0f);
        homeTile = (randomX, randomY);
        for (int i = 0; i < initialPopulaton; i++)
        {
            population[i].GetComponent<VillagerBaseBehavior>().currentLocation = homeTile;
        }
    }

    void Awake()
    {
        jobCounter = new int[] { 0, 0, 0};
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
