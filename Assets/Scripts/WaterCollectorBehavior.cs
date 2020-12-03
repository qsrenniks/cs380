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
        var knownWoodSources = VillageManager.Instance.knownForrests;
        if (knownWoodSources.Count != 0)
        {
            if (baseBehavior.hasAction)
            {
                bool collectedWater = false;
                foreach ((int, int) woodSource in knownWoodSources)
                {
                    if (checkForWater(woodSource.Item1, woodSource.Item2))
                    {
                        VillageManager.Instance.currentWater += 60;
                        VillageManager.Instance.currentWater = Mathf.Min(VillageManager.Instance.currentWater, VillageManager.Instance.waterCapacity);
                        collectedWater = true;
                        break;
                    }
                }
                if(!collectedWater)
                {
                    if(GameManager.Instance.DifficultyClassCheck(10, baseBehavior.statModArray[(int)VillagerBaseBehavior.E_STATS.INTELLIGENCE]))
                    {
                        VillageManager.Instance.currentWater += (40 + baseBehavior.statArray[(int)VillagerBaseBehavior.E_STATS.INTELLIGENCE]);
                        VillageManager.Instance.currentWater = Mathf.Min(VillageManager.Instance.currentWater, VillageManager.Instance.waterCapacity);
                    }
                    else
                    {
                        VillageManager.Instance.currentWater += 10;
                        VillageManager.Instance.currentWater = Mathf.Min(VillageManager.Instance.currentWater, VillageManager.Instance.waterCapacity);
                    }
                }
                baseBehavior.hasAction = false;
            }
        }
    }
}
