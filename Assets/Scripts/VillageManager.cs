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
    public HashSet<(int, int)> bulidingTiles;

    public int knownForrestCount = 0;
    public int knownFoodSourceCount = 0;
    public int knownWaterSourceCount = 0;
    public int buildingCount = 1;



    public int currentFood = 0;
    public int foodCapacity = 0;

    public int currentWood = 0;
    public int woodCapacity = 0;

    public int currentWater = 0;
    public int waterCapacity = 0;

    public Queue<(int, int)> exporationQueue;
    public HashSet<(int, int)> exploredTiles;

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
        return Random.Range(1, 7) + Random.Range(1, 7) + Random.Range(1, 7);
    }

    private int distance((int, int) x, (int,int) y)
    {
        return Mathf.Max(Mathf.Abs(x.Item1 - y.Item1), Mathf.Abs(x.Item1 - y.Item2));
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
                villagerData.dateOfBirth[0] = Random.Range(1, 13);
                villagerData.dateOfBirth[1] = Random.Range(1, 31);
                villagerData.dateOfBirth[2] = TimeManager.Instance.currentYear - Random.Range(18, 31);
            }
            population.Add(newVillager);
        }

        // find a starting spot for population
        TerrainGenerator terrainGenerator = terrainManager.GetComponent(typeof(TerrainGenerator)) as TerrainGenerator;
        int randomX = Random.Range(0, terrainGenerator.mapSize);
        int randomY = Random.Range(0, terrainGenerator.mapSize);
        while (!terrainGenerator.IsLand(randomX, randomY))
        {
            randomX = Random.Range(0, terrainGenerator.mapSize);
            randomY = Random.Range(0, terrainGenerator.mapSize);
        }
        terrainGenerator.populaceLayer.setTileIntensity(randomX, randomY, 1.0f);
        homeTile = (randomX, randomY);
        for (int i = 0; i < initialPopulaton; i++)
        {
            population[i].GetComponent<VillagerBaseBehavior>().currentLocation = homeTile;
        }
        terrainGenerator.buildingsLayer.setTileIntensity(homeTile.Item1, homeTile.Item2, 0.1f);
        foodCapacity += 200;
        woodCapacity += 300;
        waterCapacity += 200;

        currentFood = 200;
        currentWood = 0;
        currentWater = 200;
        bulidingTiles.Add((homeTile.Item1, homeTile.Item2));
        buildingCount++;

        exporationQueue.Enqueue(homeTile);
    }

    void createNewBuilding()
    {
        TerrainGenerator terrainGenerator = terrainManager.GetComponent(typeof(TerrainGenerator)) as TerrainGenerator;
        bool allTilesFull = true;
        bool buildSuccess = false;
        foreach ((int, int) tile in bulidingTiles)
        {
            float currentTileIntensity = terrainGenerator.buildingsLayer.getTileIntensity(tile.Item1, tile.Item2);
            if (currentTileIntensity < 1.0f)
            {
                terrainGenerator.buildingsLayer.setTileIntensity(tile.Item1, tile.Item2, currentTileIntensity + .2f); // add another building
                allTilesFull = false;
                buildSuccess = true;
                break;
            }
        }
        if(allTilesFull)
        {
            HashSet<(int, int)> tempNewBuildings = new HashSet<(int, int)>();
            foreach ((int, int) tile in bulidingTiles)
            {
                int randomDirection = Random.Range(1, 5);
                switch (randomDirection)
                {
                    case 1:
                        var up = (tile.Item1, tile.Item2 - 1);
                        if (buildSuccess == false && terrainGenerator.IsInBounds(up.Item1, up.Item2) && !bulidingTiles.Contains(up) && terrainGenerator.IsLand(up.Item1, up.Item2))
                        {
                            terrainGenerator.buildingsLayer.setTileIntensity(up.Item1, up.Item2, .2f);
                            tempNewBuildings.Add(up);
                            buildSuccess = true;
                        }
                        break;
                    case 2:
                        var right = (tile.Item1 + 1, tile.Item2);
                        if (buildSuccess == false && terrainGenerator.IsInBounds(right.Item1, right.Item2) && !bulidingTiles.Contains(right) && terrainGenerator.IsLand(right.Item1, right.Item2))
                        {
                            terrainGenerator.buildingsLayer.setTileIntensity(right.Item1, right.Item2, .2f);
                            tempNewBuildings.Add(right);
                            buildSuccess = true;
                        }
                        break;
                    case 3:
                        var down = (tile.Item1, tile.Item2 + 1);
                        if (buildSuccess == false && terrainGenerator.IsInBounds(down.Item1, down.Item2) && !bulidingTiles.Contains(down) && terrainGenerator.IsLand(down.Item1, down.Item2))
                        {
                            terrainGenerator.buildingsLayer.setTileIntensity(down.Item1, down.Item2, .2f);
                            tempNewBuildings.Add(down);
                            buildSuccess = true;
                        }
                        break;
                    case 4:
                        var left = (tile.Item1 - 1, tile.Item2);
                        if (buildSuccess == false && terrainGenerator.IsInBounds(left.Item1, left.Item2) && !bulidingTiles.Contains(left) && terrainGenerator.IsLand(left.Item1, left.Item2))
                        {
                            terrainGenerator.buildingsLayer.setTileIntensity(left.Item1, left.Item2, .2f);
                            tempNewBuildings.Add(left);
                            buildSuccess = true;
                        }
                        break;
                }
            }
            bulidingTiles.UnionWith(tempNewBuildings);
        }
        if(buildSuccess == true)
        {
            foodCapacity += 100;
            woodCapacity += 150;
            waterCapacity += 100;
            currentWood -= 100;
        }
    }

    void Awake()
    {
        jobCounter = new int[] { 0, 0, 0};
        knownForrests = new HashSet<(int, int)>();
        knownFoodSources = new HashSet<(int, int)>();
        knownWaterSources = new HashSet<(int, int)>();
        bulidingTiles = new HashSet<(int, int)>();

        exploredTiles = new HashSet<(int, int)>();
        exporationQueue = new Queue<(int, int)>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        knownForrestCount = knownForrests.Count;
        knownFoodSourceCount = knownFoodSources.Count;
        knownWaterSourceCount = knownWaterSources.Count;
        if (currentWood >= 100)
        {
            createNewBuilding();
        }
    }
}
