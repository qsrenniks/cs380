using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading;

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

    public Vector3 clearTo = new Vector3( 0.3f, 0.4f, 0.3f );
    public Vector3 rainTo = new Vector3(0.4f, 0.4f, 0.2f);
    public Vector3 snowTo = new Vector3(0.2f, 0.5f, 0.3f);


    public double[][] WeatherMatrix = new double[3][]; // 0 = clear, 1 = rain, 2 = snow
    public double[] WeatherType = new double[4]; // 0 = just weather, 1 = weather + Wind, 2 = weather + fog, 3 = weather + wind + fog
    public enum weather {clear = 0 , rain = 1, snow = 2};
    public enum secondWeather { none, wind, fog, windFog};

    weather[] weatherArray = { weather.clear, weather.rain, weather.snow };

    public weather currentWeather = weather.rain; // 0 = rain, 1 = snow, 2 = clear
    public secondWeather currentSecondW = secondWeather.none;

    public GameObject rainObject;
    public GameObject snowObject;

    RainController RC;
    SnowController SC;

    public bool autoUpdate = false;

    bool changeWeather = false;

    void Start()
    {
        //Init Matrices 
        //Weather
        //[["0.6","0.2","0.2"],
        //["0.4","0.4","0.2"],
        //["0.2","0.5","0.3"]]

        WeatherMatrix[0] = new double[] { clearTo.x, clearTo.y, clearTo.z };
        WeatherMatrix[1] = new double[] { rainTo.x, rainTo.y, rainTo.z };
        WeatherMatrix[2] = new double[] { snowTo.x, snowTo.y, snowTo.z };

        //Type 
        // .5, .2, .2, .1
        WeatherType[0] = .5; //none
        WeatherType[1] = .2; //wind
        WeatherType[2] = .2; //fog
        WeatherType[3] = .1; //wind + fog

        RC = rainObject.GetComponent<RainController>();
        SC = snowObject.GetComponent<SnowController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(changeWeather || autoUpdate)
        {
            updateMatrix();
            if(changeWeather)
            {
                if(currentWeather == weather.clear)
                {
                    currentWeather = Choice(weatherArray, WeatherMatrix[0]);
                    //change values
                    updateIntensity(currentWeather);
                }
                else if (currentWeather == weather.rain)
                {
                    currentWeather = Choice(weatherArray, WeatherMatrix[1]);
                    //change values
                    updateIntensity(currentWeather);
                }
                else if (currentWeather == weather.snow)
                {
                    currentWeather = Choice(weatherArray, WeatherMatrix[2]);
                    //change values
                    updateIntensity(currentWeather);
                }
                changeWeather = false;
            }

            //set values
            RC.OnMasterChanged(WCmasterIntensity);
            SC.OnMasterChanged(WCmasterIntensity);
            RC.OnRainChanged(WCrainIntensity);
            SC.OnSnowChanged(WCsnowIntensity);
            RC.OnWindChanged(WCwindIntensity);
            RC.OnFogChanged(WCfogIntensity);
            
        }
    }

    private void updateMatrix()
    {
        WeatherMatrix[0][0] = clearTo.x;
        WeatherMatrix[0][1] = clearTo.y;
        WeatherMatrix[0][2] = clearTo.z;
        WeatherMatrix[1][0] = rainTo.x;
        WeatherMatrix[1][1] = rainTo.y;
        WeatherMatrix[1][2] = rainTo.z;
        WeatherMatrix[2][0] = snowTo.x;
        WeatherMatrix[2][1] = snowTo.y;
        WeatherMatrix[2][2] = snowTo.z;
    }

    private void updateIntensity(weather newWeather)
    {
        if(newWeather == weather.clear)
        {
            WCmasterIntensity = 0.0f;
            WCrainIntensity = 0.0f;
            WCsnowIntensity = 0.0f;
        }
        else if(newWeather == weather.rain)
        {
            WCmasterIntensity = 1.0f;
            WCrainIntensity = 1.0f;
            WCsnowIntensity = 0.0f;
        }
        else if(newWeather == weather.snow)
        {
            WCmasterIntensity = 1.0f;
            WCrainIntensity = 0.0f;
            WCsnowIntensity = 1.0f;
        }
    }

    public void ChangeWeather()
    {
        changeWeather = true;
    }

    public weather getCurrentWeather()
    {
        return currentWeather;
    }

    public secondWeather getSecondWeather()
    {
        return currentSecondW;
    }

    static readonly ThreadLocal<System.Random> _random = new ThreadLocal<System.Random>(() => new System.Random());
    static weather Choice(weather[] sequence, double[] distribution)
    {
        double sum = 0;
        // first change shape of your distribution probablity array
        // we need it to be cumulative, that is:
        // if you have [0.1, 0.2, 0.3, 0.4] 
        // we need     [0.1, 0.3, 0.6, 1  ] instead
        var cumulative = distribution.Select(c => {
            var result = c + sum;
            sum += c;
            return result;
        }).ToList();
        // now generate random double. It will always be in range from 0 to 1
        var r = _random.Value.NextDouble();
        // now find first index in our cumulative array that is greater or equal generated random value
        var idx = cumulative.BinarySearch(r);
        // if exact match is not found, List.BinarySearch will return index of the first items greater than passed value, but in specific form (negative)
        // we need to apply ~ to this negative value to get real index
        if (idx < 0)
            idx = ~idx;
        if (idx > cumulative.Count - 1)
             idx = cumulative.Count - 1; 

        return sequence[idx];
        
    }
}
