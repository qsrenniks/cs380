﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderText : MonoBehaviour
{
    public UnityEngine.UI.Slider slider;
    public UnityEngine.UI.Text text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "" + slider.value;
    }
}
