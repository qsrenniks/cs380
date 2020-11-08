using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private static TimeManager instance = null;

    public int currentYear = 100;
    public int currentMonth = 1;
    public int currentDay = 1;
    public int rawDay = 0;
    public Timer dayTimer;
    public float realSecondsToGameDay = 5.0f;
    public static TimeManager Instance
    {
        get
        {
            instance = FindObjectOfType<TimeManager>();
            if(instance == null)
            {
                GameObject container = new GameObject("TimeManager");
                instance = container.AddComponent<TimeManager>();
            }
            return instance;
        }
    }

    public class GameTimer // timer for in-game raw days
    {
        public int dayStarted;
        public int daysToGo;

        public bool isCompleted = false;
        public GameTimer(int daysTilActivation)
        {
            dayStarted = Instance.rawDay;
            daysToGo = daysTilActivation;
        }
        public void Update()
        {
            if (!isCompleted && ((dayStarted + daysToGo) <= Instance.rawDay))
                isCompleted = true;
        }
        public void reset()
        {
            dayStarted = Instance.rawDay;
            isCompleted = false;
        }
    }

    public class Timer // timer for realworld time
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
            if (timeRemaining < 0.0f)
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
        dayTimer = new Timer(realSecondsToGameDay);
    }

    // Update is called once per frame
    void Update()
    {
        dayTimer.Update();
        if(dayTimer.isDone())
        {
            currentDay++;
            rawDay++;
            if (currentDay > 30)
            {
                currentDay = 1;
                currentMonth++;
                if(currentMonth > 12)
                {
                    currentMonth = 1;
                    currentYear++;
                }
            }
            dayTimer.reset();
        }
    }
}
