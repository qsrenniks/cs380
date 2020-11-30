using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSliders : MonoBehaviour
{

    public WeatherController WC;
    public UnityEngine.UI.Slider clear;
    public UnityEngine.UI.Slider rain;
    public UnityEngine.UI.Slider snow;

    // Start is called before the first frame update
    void Start()
    {
        clear.value = WC.clearTo.x;
        rain.value = WC.clearTo.y;
        snow.value = WC.clearTo.z;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
