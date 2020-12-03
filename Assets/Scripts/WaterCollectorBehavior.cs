using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCollectorBehavior : MonoBehaviour
{
    bool checkForWater(int x, int y)
    {
        TerrainGenerator terrainGenerator = VillageManager.Instance.terrainManager.GetComponent<TerrainGenerator>();
        if (!terrainGenerator.IsInBounds(x, y))
            return false;
        return terrainGenerator.IsWater(x, y); ;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        VillagerBaseBehavior baseBehavior = gameObject.GetComponent<VillagerBaseBehavior>();
        var knownWaterSources = VillageManager.Instance.knownWaterSources;
        if (knownWaterSources.Count != 0)
        {
            if (baseBehavior.hasAction)
            {
                bool collectedWater = false;
                foreach ((int, int) waterSource in knownWaterSources)
                {
                    if (checkForWater(waterSource.Item1, waterSource.Item2))
                    {
                        VillageManager.Instance.currentWater += (80 + baseBehavior.statArray[(int)VillagerBaseBehavior.E_STATS.STRENGTH]); ;
                        VillageManager.Instance.currentWater = Mathf.Min(VillageManager.Instance.currentWater, VillageManager.Instance.waterCapacity);
                        collectedWater = true;
                        break;
                    }
                }
                if(!collectedWater)
                {
                    if(GameManager.Instance.DifficultyClassCheck(10, baseBehavior.statModArray[(int)VillagerBaseBehavior.E_STATS.INTELLIGENCE]))
                    {
                        VillageManager.Instance.currentWater += (60 + baseBehavior.statArray[(int)VillagerBaseBehavior.E_STATS.INTELLIGENCE]);
                        VillageManager.Instance.currentWater = Mathf.Min(VillageManager.Instance.currentWater, VillageManager.Instance.waterCapacity);
                    }
                    else
                    {
                        VillageManager.Instance.currentWater += 40;
                        VillageManager.Instance.currentWater = Mathf.Min(VillageManager.Instance.currentWater, VillageManager.Instance.waterCapacity);
                    }
                }
                baseBehavior.hasAction = false;
            }
        }
    }
}
