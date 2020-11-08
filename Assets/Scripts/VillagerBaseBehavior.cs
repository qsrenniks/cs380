using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerBaseBehavior : MonoBehaviour
{
    public int[] statArray;
    public int[] statModArray;
    public int primaryRace = -1;
    public int secondaryRace = -1;
    public int generation = 0;
    public GameObject partner = null;
    public bool canMakeChildren = false;
    public enum E_STATS { STRENGTH = 0, DEXTERITY, CONSTITUTION, INTELLIGENCE, WISDOM, CHARISMA, COUNT };
    public enum E_RACE { HUMAN = 0, ELF, DWARF, GNOME, HALFLING, TIEFLING, GOBLIN, COUNT};
    public int[] dateOfBirth; // month, day, year
    public TimeManager.GameTimer reproductionCooldown;

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
        int age = getAge();
        if (age > 18 && reproductionCooldown.isCompleted == true)
            canMakeChildren = true;
        if(age >= 55) // old age check
        {
            float chanceOfDeath = VillageManager.Instance.chanceOfDeathFunction(age);
            if(chanceOfDeath >= Random.Range(0,100)) // villager dies
            {
                VillageManager.Instance.population.Remove(gameObject);
                Destroy(gameObject);
            }
        }
    }
}
