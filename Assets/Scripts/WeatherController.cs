using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{

    [Range(0, 1f)]
    public float WCmasterIntensity = 1f;
    [Range(0, 1f)]
    public float WCrainIntensity = 1f;
    [Range(0, 1f)]
    public float WCsnowIntensity = 1f;
    [Range(0, 1f)]
    public float WCwindIntensity = 1f;
    [Range(0, 1f)]
    public float WCfogIntensity = 1f;


    public float[,] WeatherMatrix = new float [3, 3]; // 0 = rain, 1 = snow, 2 = clear
    public float[,] RainMatrix = new float[4, 4]; // 0 = just rain, 1 = rain + Wind, 2 = rain + fog, 3 = rain + wind + fog
    public float[,] SnowMatrix = new float[4, 4]; // 0 = just snow, 1 = snow + Wind, 2 = snow + fog, 3 = snow + wind + fog
    public float[,] ClearMatrix = new float[4, 4]; //0 = clear, 1 = wind, 2 = fog, 3 = wind + fog

    public enum weather {rain, snow, clear};

    public weather currentWeather = weather.rain; // 0 = rain, 1 = snow, 2 = clear

    public GameObject rainObject;
    public GameObject snowObject;

    RainController RC;
    SnowController SC;

    public bool autoUpdate = false;

    void Start()
    {
        //Init Matrices 

        RC = rainObject.GetComponent<RainController>();
        SC = snowObject.GetComponent<SnowController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(autoUpdate)
        {
            RC.OnMasterChanged(WCmasterIntensity);
            SC.OnMasterChanged(WCmasterIntensity);
            RC.OnRainChanged(WCrainIntensity);
            SC.OnSnowChanged(WCsnowIntensity);
            RC.OnWindChanged(WCwindIntensity);
            RC.OnFogChanged(WCfogIntensity);
        }


    }

    static void readWeatherMat(ref float[,] wMat) //determines what matrix to call
    {

    }

    static void readRainMatrix(ref float[,] rMat) //sets rain
    {

    }

    static void readSnowMatrix(ref float[,] sMat) //sets snow
    {

    }

    static void readClearMatrix(ref float[,] cMat) //sets clear
    {

    }

}
