using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    public int initialPopulaton = 10;
    public List<GameObject> population;
    public GameObject villagerPrefab;
    public int mutationRate;
    public bool DifficultyClassCheck(int DC, int MOD)
    {
        Debug.Log("Difficulty Check");
        return DC <= Random.Range(1, 20) + MOD;
    }

    public int calculateModifier(int stat)
    {
        switch(stat)
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

    private int statDistrobutionFunction()
    {
        int positiveOrNegative = Random.Range(0, 1);
        float randomRoll = Random.Range(10, 100) / 100.0f;
        int newStat = 0;
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
        for (int i = 0; i < population.Count; i++)
        {
            VillagerBaseBehavior villagerData = population[i].GetComponent<VillagerBaseBehavior>();
            if (villagerData.generation == oldestGeneration)
            {
                Destroy(population[i], .5f);
                population.Remove(population[i]);
            }
        }
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
                villagerData.statModArray[j] = calculateModifier(newStat);
            }
            population.Add(newVillager);
        }
    }

    public void Repopulate()
    {
        Debug.Log("Repopulation");
        for (int i = 0; i < population.Count; i++)
        {
            VillagerBaseBehavior villagerData = population[i].GetComponent<VillagerBaseBehavior>();
            if (villagerData.partner == null)
            {
                for (int j = 1; j < population.Count; j++)
                {
                    VillagerBaseBehavior otherVillagerData = population[j].GetComponent<VillagerBaseBehavior>();
                    if (villagerData.partner == null && DifficultyClassCheck(8, villagerData.statModArray[(int)VillagerBaseBehavior.E_STATS.CHARISMA]))
                    {
                        villagerData.partner = population[j];
                        otherVillagerData.partner = population[i];
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < population.Count; i++)
        {
            VillagerBaseBehavior villagerData = population[i].GetComponent<VillagerBaseBehavior>();
            if (villagerData.partner != null && villagerData.hasMadeChildren == false)
            {
                GameObject newVillager = Instantiate(villagerPrefab);
                VillagerBaseBehavior newVillagerData = newVillager.GetComponent<VillagerBaseBehavior>();
                VillagerBaseBehavior otherVillagerData = villagerData.partner.GetComponent<VillagerBaseBehavior>();
                for (int j = 0; j < villagerData.statArray.Length; j++)
                {
                    int statDelta = villagerData.statArray[j] - otherVillagerData.statArray[j];
                    int result = Random.Range(1, Mathf.Abs(statDelta) + 1);

                    if(statDelta <= 0) // villager has the lower stat
                    {
                        int newStat = (int)(villagerData.statArray[j] + (Mathf.Abs(statDelta) * (result / ((float)Mathf.Abs(statDelta) + 1))));
                        if (Random.Range(0, 100) <= mutationRate)
                            newStat = Random.Range(1, 20);
                        newVillagerData.statArray[j] = newStat;
                        newVillagerData.statModArray[j] = calculateModifier(newStat);
                    }
                    else // other villager has the lower stat
                    {
                        int newStat = (int)(otherVillagerData.statArray[j] + (Mathf.Abs(statDelta) * (result / ((float)Mathf.Abs(statDelta) + 1))));
                        if (Random.Range(0, 100) <= mutationRate)
                            newStat = Random.Range(1, 20);
                        newVillagerData.statArray[j] = newStat;
                        newVillagerData.statModArray[j] = calculateModifier(newStat);
                    }
                }
                newVillagerData.generation = villagerData.generation + 1;
                villagerData.hasMadeChildren = true;
                otherVillagerData.hasMadeChildren = true;
                population.Add(newVillager);
            }
        }
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
