using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject villageManagerPrefab;
    private VillageManager villageManager;
    private Timer repopulateTimer;

    public class Timer
    {
        public float timeRemaining = 0.0f;
        public float initialTime = 0.0f;
        public bool isRunning = true;

        public Timer(float timeMax)
        {
            timeRemaining = initialTime = timeMax;
        }

        public void Update()
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            if(timeRemaining < 0.0f)
            {
                timeRemaining = 0.0f;
            }
        }

        public bool isDone()
        {
            if (timeRemaining == 0.0f)
                return true;
            else
                return false;
        }

        public void reset()
        {
            timeRemaining = initialTime;
        }

        public void pause()
        {
            isRunning = !isRunning;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        villageManager = Instantiate(villageManagerPrefab).GetComponent<VillageManager>();
        villageManager.CreateInitialPopulation();
        repopulateTimer = new Timer(10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        repopulateTimer.Update();
        if (repopulateTimer.isDone())
        {
            villageManager.Repopulate();
            villageManager.CullTheOldestGeneration();
            repopulateTimer.reset();
        }
    }
}
