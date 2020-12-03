using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ExplorationBehavior : MonoBehaviour
{
    private void exploreOnce(int x, int y)
    {
        VillageManager.Instance.exploredTiles.Add((x, y));
        TerrainGenerator terrainGenerator = VillageManager.Instance.terrainManager.GetComponent<TerrainGenerator>();

        // Base cases
        if (x < 0 || x >= terrainGenerator.mapSize || y < 0 || y >= terrainGenerator.mapSize)
            return;

        if (terrainGenerator.foodLayer.getTileIntensity(x, y) > 0.0f)
            VillageManager.Instance.knownFoodSources.Add((x, y));
        if (terrainGenerator.foliageLayer.getTileIntensity(x, y) > 0.0f)
            VillageManager.Instance.knownForrests.Add((x, y));
        if (terrainGenerator.IsWater(x, y))
            VillageManager.Instance.knownWaterSources.Add((x, y));


        if (!VillageManager.Instance.exploredTiles.Contains((x + 1, y)))
            VillageManager.Instance.exporationQueue.Enqueue((x + 1, y));

        if (!VillageManager.Instance.exploredTiles.Contains((x - 1, y)))
            VillageManager.Instance.exporationQueue.Enqueue((x - 1, y));

        if (!VillageManager.Instance.exploredTiles.Contains((x, y + 1)))
            VillageManager.Instance.exporationQueue.Enqueue((x, y + 1));

        if (!VillageManager.Instance.exploredTiles.Contains((x, y - 1)))
            VillageManager.Instance.exporationQueue.Enqueue((x, y - 1));
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        VillagerBaseBehavior baseBehavior = gameObject.GetComponent<VillagerBaseBehavior>();
        if (baseBehavior.hasAction)
        {
            if(VillageManager.Instance.exporationQueue.Count > 0)
            {
                var top = VillageManager.Instance.exporationQueue.First();
                VillageManager.Instance.exporationQueue.Dequeue();
                exploreOnce(top.Item1, top.Item2);
            }
        }
    }
}
