using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int currentDay = 0;
    public bool hasAction = true;

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
        return newVillager;
    }
    void Awake()
    {
        statArray = new int[] { 0, 0, 0, 0, 0, 0 };
        statModArray = new int[] { 0, 0, 0, 0, 0, 0 };
        dateOfBirth = new int[] { 0, 0, 0 };
        reproductionCooldown = new TimeManager.GameTimer(270); // 9 months in days
        reproductionCooldown.isCompleted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentDay + 7 <= TimeManager.Instance.rawDay) // reset action after a week
            hasAction = true;

        int age = getAge();

        // find partner action
        if(hasAction && partner == null && age >= 18)
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
        if (age >= 18 && reproductionCooldown.isCompleted == true && hasAction)
        {
            GameObject newVillager = Instantiate(VillageManager.Instance.villagerPrefab);
            MakeChild(5, newVillager);
            reproductionCooldown.reset();
            hasAction = false;
        }

        if (age >= 55) // old age check
        {
            float chanceOfDeath = VillageManager.Instance.chanceOfDeathFunction(age);
            if(chanceOfDeath >= Random.Range(0,100)) // villager dies
            {
                partner.GetComponent<VillagerBaseBehavior>().partner = null;
                VillageManager.Instance.population.Remove(gameObject);
                Destroy(gameObject);
            }
        }
    }
}
