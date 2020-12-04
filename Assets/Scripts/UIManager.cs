using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene("NewWorld");
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void ChangeType(int t)
    {
        type = t;
        if (type == 0) // terrain
        {
            //WeatherSliders.SetActive(true);
            TerrianSliders.SetActive(true);
            VillageSliders.SetActive(false);
        }
        else if (type == 1) // Terrian
        {
            //WeatherSliders.SetActive(false);
            TerrianSliders.SetActive(false);
            VillageSliders.SetActive(true);
        }
       // else if (type == 2) // Village
       // {
       //    // WeatherSliders.SetActive(false);
       //     TerrianSliders.SetActive(false);
       //     TerrianSliders.SetActive(true);
        //}
    }
}
