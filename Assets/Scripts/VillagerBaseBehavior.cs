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
    public bool hasMadeChildren = false;
    public enum E_STATS { STRENGTH = 0, DEXTERITY, CONSTITUTION, INTELLIGENCE, WISDOM, CHARISMA, COUNT };
    public enum E_RACE { HUMAN = 0, ELF, DWARF, GNOME, HALFLING, TIEFLING, GOBLIN, COUNT};


    // Start is called before the first frame update
    void Awake()
    {
        statArray = new int[] { 0, 0, 0, 0, 0, 0 };
        statModArray = new int[] { 0, 0, 0, 0, 0, 0 };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
