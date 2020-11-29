using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainController : MonoBehaviour
{
    [Range(0, 1f)]
     float masterIntensity = 0f;
    [Range(0, 1f)]
     float rainIntensity = 0f;
    [Range(0, 1f)]
     float windIntensity = 0f;
    [Range(0, 1f)]
     float fogIntensity = 0f;
    [Range(0, 1f)]
    float lightningIntensity = 0f;
    public bool autoUpdate = true;

    public Camera MainCamera;

    public ParticleSystem rainPart;
    public ParticleSystem windPart;
    public ParticleSystem lightningPart;
    public ParticleSystem fogPart;

    private ParticleSystem.EmissionModule rainEmission;
    private ParticleSystem.ForceOverLifetimeModule rainForce;
    private ParticleSystem.EmissionModule windEmission;
    private ParticleSystem.MainModule windMain;
    private ParticleSystem.EmissionModule lightningEmission;
    private ParticleSystem.MainModule lightningMain;
    private ParticleSystem.EmissionModule fogEmission;

    void Awake()
    {
        autoUpdate = true;
        rainEmission = rainPart.emission;
        rainForce = rainPart.forceOverLifetime;
        windEmission = windPart.emission;
        windMain = windPart.main;
        lightningEmission = lightningPart.emission;
        lightningMain = lightningPart.main;
        fogEmission = fogPart.emission;

        Vector3 v = MainCamera.transform.position;
        rainPart.transform.position = new Vector3(v.x, v.y + 65.0f, 0);
        windPart.transform.position = new Vector3(v.x + 150.0f, v.y-2.3f, 0);
        lightningPart.transform.position = new Vector3(v.x - 0.5f, v.y+3.35f, 4.46f);
        fogPart.transform.position = new Vector3(v.x +150.8f, v.y+1.2f, 4.46f);

        UpdateAll();
    }

    void Update()
    {
        if (autoUpdate)
            UpdateAll();
    }

    void UpdateAll(){
        rainEmission.rateOverTime = 200f * masterIntensity * rainIntensity;
        rainForce.x = new ParticleSystem.MinMaxCurve(-25f * windIntensity * masterIntensity, (-3-30f * windIntensity) * masterIntensity);
        windEmission.rateOverTime = 5f * masterIntensity * (windIntensity + fogIntensity);
        windMain.startLifetime = 5f + 5f * (1f - windIntensity);
        windMain.startSpeed = new ParticleSystem.MinMaxCurve(15f * windIntensity, 25 * windIntensity);
        fogEmission.rateOverTime = (1f + (rainIntensity + windIntensity)*0.5f) * fogIntensity * masterIntensity;
        if (rainIntensity * masterIntensity < 0.7f)
            lightningEmission.rateOverTime = 0;
        else
            lightningEmission.rateOverTime = lightningIntensity * masterIntensity * 0.4f;
    }

    public void OnMasterChanged(float value)
    {
        masterIntensity = value;
        //UpdateAll();
    }
    public void OnRainChanged(float value)
    {
        rainIntensity = value;
        //UpdateAll();
    }
    public void OnWindChanged(float value)
    {
        windIntensity = value;
        //UpdateAll();
    }
    public void OnLightningChanged(float value)
    {
        lightningIntensity = value;
        //UpdateAll();
    }
    public void OnFogChanged(float value)
    {
        fogIntensity = value;
        //UpdateAll();
    }
}
