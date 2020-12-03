using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VillagerBaseBehavior : MonoBehaviour
{
    public int[] statArray;
    public int[] statModArray;
    public E_RACE primaryRace = E_RACE.HUMAN;
    public E_RACE secondaryRace = E_RACE.HUMAN;
    public int generation = 0;
    public GameObject partner = null;
    public bool canMakeChildren = false;
    public enum E_STATS { STRENGTH = 0, DEXTERITY, CONSTITUTION, INTELLIGENCE, WISDOM, CHARISMA, COUNT };
    public enum E_RACE { HUMAN = 0, ELF, DWARF, GNOME, HALFLING, TIEFLING, GOBLIN, COUNT};
    public int[] dateOfBirth; // month, day, year
    public TimeManager.GameTimer reproductionCooldown;
    public TimeManager.GameTimer actionCooldown;
    public TimeManager.GameTimer eatAndDrinkCooldown;
    public int currentDay = 0;
    public bool hasAction = true;
    public int childrenCount = 0;
    public int hungerCount = 0;
    public int thirstCount = 0;


    public (int, int) currentLocation;

    public int getAge()
    {
        int yearsDifference = TimeManager.Instance.currentYear - dateOfBirth[2];
        if (dateOfBirth[0] < TimeManager.Instance.currentMonth)
        {
            return yearsDifference;
        }
        else if (dateOfBirth[0] == TimeManager.Instance.currentMonth)
        {
            if (dateOfBirth[1] <= TimeManager.Instance.currentDay)
                return yearsDifference;
            else
                return yearsDifference - 1;
        }
        else
            return yearsDifference - 1;
    }
    public GameObject MakeChild(int mutationRate, GameObject newVillager)
    {
        VillagerBaseBehavior newVillagerData = newVillager.GetComponent<VillagerBaseBehavior>();
        VillagerBaseBehavior otherVillagerData = partner.GetComponent<VillagerBaseBehavior>();
        for (int j = 0; j < statArray.Length; j++)
        {
            int statDelta = statArray[j] - otherVillagerData.statArray[j];
            int result = Random.Range(1, Mathf.Abs(statDelta) + 1);

            if (statDelta <= 0) // villager has the lower stat
            {
                int newStat = (int)(statArray[j] + (Mathf.Abs(statDelta) * (result / ((float)Mathf.Abs(statDelta) + 1))));
                if (Random.Range(0, 100) <= mutationRate)
                    newStat = Random.Range(1, 20);
                newVillagerData.statArray[j] = newStat;
                newVillagerData.statModArray[j] = GameManager.Instance.calculateModifier(newStat);
            }
            else // other villager has the lower stat
            {
                int newStat = (int)(otherVillagerData.statArray[j] + (Mathf.Abs(statDelta) * (result / ((float)Mathf.Abs(statDelta) + 1))));
                if (Random.Range(0, 100) <= mutationRate)
                    newStat = Random.Range(1, 20);
                newVillagerData.statArray[j] = newStat;
                newVillagerData.statModArray[j] = GameManager.Instance.calculateModifier(newStat);
            }
        }
        newVillagerData.generation = generation + 1;
        newVillagerData.dateOfBirth[0] = TimeManager.Instance.currentMonth;
        newVillagerData.dateOfBirth[1] = TimeManager.Instance.currentDay;
        newVillagerData.dateOfBirth[2] = TimeManager.Instance.currentYear;
        canMakeChildren = false;
        otherVillagerData.canMakeChildren = false;
        childrenCount++;
        otherVillagerData.childrenCount++;
        return newVillager;
    }
   
    void Awake()
    {
        statArray = new int[] { 0, 0, 0, 0, 0, 0 };
        statModArray = new int[] { 0, 0, 0, 0, 0, 0 };
        dateOfBirth = new int[] { 0, 0, 0 };
        reproductionCooldown = new TimeManager.GameTimer(270); // 9 months in days
        reproductionCooldown.isCompleted = true;

        actionCooldown = new TimeManager.GameTimer(3);
        actionCooldown.isCompleted = true;

        eatAndDrinkCooldown = new TimeManager.GameTimer(1);
        eatAndDrinkCooldown.isCompleted = true;
    }

    // Update is called once per frame
    void Update()
    {
        reproductionCooldown.Update();
        actionCooldown.Update();
        eatAndDrinkCooldown.Update();
        if (actionCooldown.isCompleted) // reset action after a week
        {
            hasAction = true;
            actionCooldown.reset();
        }

        int age = getAge();
        if(age >= 18)
        {
            // give job
            if(GetComponents(typeof(Component)).Length <= 2)
            {
                int rollResult = Random.Range(1, 4);
                switch (rollResult)
                {
                    case 1:
                        gameObject.AddComponent(typeof(FoodGathererBehavior));
                        break;
                    case 2:
                        gameObject.AddComponent(typeof(WaterCollectorBehavior));
                        break;
                    case 3:
                        gameObject.AddComponent(typeof(WoodcutterBehavior));
                        break;
                }
            }
            // find partner action
            if (hasAction && partner == null)
            {
                for (int i = 0; i < VillageManager.Instance.population.Count; i++)
                {
                    if (gameObject == VillageManager.Instance.population[i])
                        continue;
                    VillagerBaseBehavior otherVillagerData = VillageManager.Instance.population[i].GetComponent<VillagerBaseBehavior>();
                    if (otherVillagerData.partner == null && GameManager.Instance.DifficultyClassCheck(8, statModArray[(int)E_STATS.CHARISMA]))
                    {
                        partner = otherVillagerData.gameObject;
                        otherVillagerData.partner = VillageManager.Instance.population[i];
                        break;
                    }
                }
                hasAction = false;
            }
            // reproduction action
            if (hasAction && reproductionCooldown.isCompleted == true)
            {
                int randomRoll = Random.Range(0, 101);
                randomRoll -= childrenCount * 20;
                if(randomRoll > 0)
                {
                    GameObject newVillager = Instantiate(VillageManager.Instance.villagerPrefab);
                    MakeChild(5, newVillager);
                    VillageManager.Instance.population.Add(newVillager);
                    reproductionCooldown.reset();
                    canMakeChildren = true;
                    hasAction = false;
                }
            }
        }
        
        if(GetComponent<ExplorationBehavior>() == null && age >= 10) // explore
        {
            gameObject.AddComponent(typeof(ExplorationBehavior));
        }

        if(eatAndDrinkCooldown.isCompleted)
        {
            eatAndDrinkCooldown.reset();
            if (VillageManager.Instance.currentFood <= 0)
            {
                hungerCount++;
                if(hungerCount >= statArray[(int)E_STATS.CONSTITUTION])
                {
                    Debug.Log("Villager Died From Starvation");
                    if (partner)
                    {
                        partner.GetComponent<VillagerBaseBehavior>().partner = null;
                        partner = null;
                    }
                    VillageManager.Instance.population.Remove(gameObject);
                    Destroy(gameObject);
                }
            }
            else
            {
                VillageManager.Instance.currentFood -= 1;
                hungerCount = 0;
            }

            if (VillageManager.Instance.currentWater <= 0)
            {
                thirstCount++;
                if(thirstCount >= statArray[(int)E_STATS.CONSTITUTION] / 3)
                {
                    Debug.Log("Villager Died From Dehydration");
                    if (partner)
                    {
                        partner.GetComponent<VillagerBaseBehavior>().partner = null;
                        partner = null;
                    }
                    VillageManager.Instance.population.Remove(gameObject);
                    Destroy(gameObject);
                }
            }
            else
            {
                VillageManager.Instance.currentWater -= 1;
                thirstCount = 0;
            }
        }

        if (age >= 55) // old age check
        {
            float chanceOfDeath = VillageManager.Instance.chanceOfDeathFunction(age);
            if(chanceOfDeath >= Random.Range(0,101)) // villager dies
            {
                Debug.Log("Villager Died From Old Age");
                if (partner)
                {
                    partner.GetComponent<VillagerBaseBehavior>().partner = null;
                    partner = null;
                }

                VillageManager.Instance.population.Remove(gameObject);
                Destroy(gameObject);
            }
        }
    }
}
