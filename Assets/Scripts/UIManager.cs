using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    private int type;

    public GameObject settingsBar;
    public GameObject WeatherSliders;
    public GameObject TerrianSliders;
    public GameObject VillageSliders;

    // Start is called before the first frame update
    void Start()
    {
        ChangeType(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            settingsBar.SetActive(!settingsBar.activeSelf);
    }

    public void ChangeType(int t)
    {
        type = t;
        if (type == 0) // Weather
        {
            WeatherSliders.SetActive(true);
            TerrianSliders.SetActive(false);
            TerrianSliders.SetActive(false);
        }
        else if (type == 1) // Terrian
        {
            WeatherSliders.SetActive(false);
            TerrianSliders.SetActive(true);
            TerrianSliders.SetActive(false);
        }
        else if (type == 2) // Village
        {
            WeatherSliders.SetActive(false);
            TerrianSliders.SetActive(false);
            TerrianSliders.SetActive(true);
        }
    }
}
