using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherPosition : MonoBehaviour
{

    public new Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v = camera.transform.position;
        this.transform.position = new Vector3(v.x, v.y, 0);
        
    }
}
