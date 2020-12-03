using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodcutterBehavior : MonoBehaviour
{
    float checkForWood(int x, int y)
    {
        TerrainGenerator terrainGenerator = VillageManager.Instance.terrainManager.GetComponent<TerrainGenerator>();
        if (!terrainGenerator.IsInBounds(x, y))
            return -1.0f;
        float woodAmount = terrainGenerator.foliageLayer.getTileIntensity(x, y);
        return woodAmount;
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
                foreach ((int, int) woodSource in knownWoodSources)
                {
                    float currentWoodAmount = checkForWood(woodSource.Item1, woodSource.Item2);
                    if (currentWoodAmount > 0.0f)
                    {
                        if (GameManager.Instance.DifficultyClassCheck(13, baseBehavior.statModArray[(int)VillagerBaseBehavior.E_STATS.STRENGTH]))
                        {
                            VillageManager.Instance.currentWood += (40 + baseBehavior.statArray[(int)VillagerBaseBehavior.E_STATS.STRENGTH]);
                            VillageManager.Instance.currentWood = Mathf.Min(VillageManager.Instance.currentWood, VillageManager.Instance.woodCapacity);
                            break;
                        }
                    }
                }
                baseBehavior.hasAction = false;
            }
        }
    }
}
