using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textWeather : MonoBehaviour
{
    // Start is called before the first frame update

    public WeatherController WC;
    public UnityEngine.UI.Text text;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (WC.getCurrentWeather() == WC.weatherArray[0])
        {
            text.text = "Clear";
        }
        else if (WC.getCurrentWeather() == WC.weatherArray[1])
        {
            text.text = "Rain";
        }
        else if (WC.getCurrentWeather() == WC.weatherArray[2])
        {
            text.text = "Snow";
        }
    }
}
