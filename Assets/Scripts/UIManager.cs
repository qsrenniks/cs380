using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    private int type;

    public GameObject settingsBar;
    public GameObject slider1;
    public GameObject slider2;

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
        if (type == 0)
        {
            slider1.SetActive(true);
        }
        else if (type == 1)
        {
            slider2.SetActive(false);
        }
    }
}
