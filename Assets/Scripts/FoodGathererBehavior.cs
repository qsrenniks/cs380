using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodGathererBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    bool checkForFood(int x, int y)
    {
        TerrainGenerator terrainGenerator = VillageManager.Instance.terrainManager.GetComponent<TerrainGenerator>();
        if (!terrainGenerator.IsInBounds(x, y))
            return false;
        float foodAmount = terrainGenerator.foodLayer.getTileIntensity(x, y);
        if (foodAmount <= 0.0f)
            return false;
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        VillagerBaseBehavior baseBehavior = gameObject.GetComponent<VillagerBaseBehavior>();
        var knownFoodSources = VillageManager.Instance.knownFoodSources;
        if(knownFoodSources.Count != 0)
        {
            if(baseBehavior.hasAction)
            {
                foreach ((int, int) foodSource in knownFoodSources)
                {
                    if(checkForFood(foodSource.Item1,foodSource.Item2))
                    {
                        if (GameManager.Instance.DifficultyClassCheck(13, baseBehavior.statModArray[(int)VillagerBaseBehavior.E_STATS.WISDOM]))
                        {
                            TerrainGenerator terrainGenerator = VillageManager.Instance.terrainManager.GetComponent<TerrainGenerator>();
                            VillageManager.Instance.currentFood += (30 + baseBehavior.statArray[(int)VillagerBaseBehavior.E_STATS.WISDOM]); ;
                            VillageManager.Instance.currentFood = Mathf.Min(VillageManager.Instance.currentFood, VillageManager.Instance.foodCapacity);
                            break;
                        }
                    }
                }
                baseBehavior.hasAction = false;
            }
        }
    }
}