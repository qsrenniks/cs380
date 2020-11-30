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
        if (terrainGenerator.foodLayer.grid)
            return false;
    }

    // Update is called once per frame
    void Update()
    {
        VillagerBaseBehavior baseBehavior = gameObject.GetComponent<VillagerBaseBehavior>();
        var knownFoodSources = VillageManager.Instance.knownFoodSources;
        if(knownFoodSources.Length == 0)
        {
            if(baseBehavior.hasAction)
            {

            }
        }
        else if(baseBehavior.hasAction)
        {

        }
    }
}
