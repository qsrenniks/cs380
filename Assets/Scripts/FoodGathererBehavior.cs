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
        if(knownFoodSources.Count == 0)
        {
            if(baseBehavior.hasAction)
            {
                int direction = Random.Range(1, 4);
                switch (direction)
                {
                    case 1:
                        if (checkForFood(baseBehavior.currentLocation.Item1, baseBehavior.currentLocation.Item2 - 1))
                        {
                            knownFoodSources.Add((baseBehavior.currentLocation.Item1, baseBehavior.currentLocation.Item2 - 1));
                            baseBehavior.currentLocation = (baseBehavior.currentLocation.Item1, baseBehavior.currentLocation.Item2 - 1);
                        }
                        else if (checkForFood(baseBehavior.currentLocation.Item1, baseBehavior.currentLocation.Item2 - 2))
                        {
                            knownFoodSources.Add((baseBehavior.currentLocation.Item1, baseBehavior.currentLocation.Item2 - 2));
                            baseBehavior.currentLocation = (baseBehavior.currentLocation.Item1, baseBehavior.currentLocation.Item2 - 2);
                        }
                        break;
                    case 2:
                        if (checkForFood(baseBehavior.currentLocation.Item1 + 1, baseBehavior.currentLocation.Item2))
                        {
                            knownFoodSources.Add((baseBehavior.currentLocation.Item1 + 1, baseBehavior.currentLocation.Item2));
                            baseBehavior.currentLocation = (baseBehavior.currentLocation.Item1 + 1, baseBehavior.currentLocation.Item2);
                        }
                        else if (checkForFood(baseBehavior.currentLocation.Item1 + 2, baseBehavior.currentLocation.Item2))
                        {
                            knownFoodSources.Add((baseBehavior.currentLocation.Item1 + 2, baseBehavior.currentLocation.Item2));
                            baseBehavior.currentLocation = (baseBehavior.currentLocation.Item1 + 2, baseBehavior.currentLocation.Item2);
                        }
                        break;
                    case 3:
                        if (checkForFood(baseBehavior.currentLocation.Item1, baseBehavior.currentLocation.Item2 + 1))
                        {
                            knownFoodSources.Add((baseBehavior.currentLocation.Item1, baseBehavior.currentLocation.Item2 + 1));
                            baseBehavior.currentLocation = (baseBehavior.currentLocation.Item1, baseBehavior.currentLocation.Item2 + 1);
                        }
                        else if (checkForFood(baseBehavior.currentLocation.Item1, baseBehavior.currentLocation.Item2 + 2))
                        {
                            knownFoodSources.Add((baseBehavior.currentLocation.Item1, baseBehavior.currentLocation.Item2 + 2));
                            baseBehavior.currentLocation = (baseBehavior.currentLocation.Item1, baseBehavior.currentLocation.Item2 + 2);
                        }
                        break;
                    case 4:
                        if (checkForFood(baseBehavior.currentLocation.Item1 + 1, baseBehavior.currentLocation.Item2))
                        {
                            knownFoodSources.Add((baseBehavior.currentLocation.Item1 + 1, baseBehavior.currentLocation.Item2));
                            baseBehavior.currentLocation = (baseBehavior.currentLocation.Item1 + 1, baseBehavior.currentLocation.Item2);
                        }
                        else if (checkForFood(baseBehavior.currentLocation.Item1 + 2, baseBehavior.currentLocation.Item2))
                        {
                            knownFoodSources.Add((baseBehavior.currentLocation.Item1 + 2, baseBehavior.currentLocation.Item2));
                            baseBehavior.currentLocation = (baseBehavior.currentLocation.Item1 + 2, baseBehavior.currentLocation.Item2);
                        }
                        break;
                }
                baseBehavior.hasAction = false;
            }
        }
        else if(baseBehavior.hasAction)
        {
            if (checkForFood(baseBehavior.currentLocation.Item1, baseBehavior.currentLocation.Item2))
            {
                return;
            }
            else
            {
                int location = Random.Range(0, knownFoodSources.Count - 1);

            }

        }
    }
}