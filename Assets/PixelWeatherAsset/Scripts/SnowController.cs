using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowController : MonoBehaviour
{
    [Range(0, 1f)]
    float masterIntensity = 0f;
    [Range(0, 1f)]
    float snowIntensity = 0f;
    [Range(0, 1f)]
    float windIntensity = 0f;
    [Range(0, 1f)]
    float fogIntensity = 0f;
    [Range(0, 7f)]
    float snowLevel;
    public bool autoUpdate = true;

    public Camera MainCamera;

    public ParticleSystem snowPart;
    public ParticleSystem windPart;
    public ParticleSystem fogPart;

    private ParticleSystem.EmissionModule snowEmission;
    private ParticleSystem.ForceOverLifetimeModule snowForce;
    private ParticleSystem.ShapeModule snowShape;
    private ParticleSystem.EmissionModule windEmission;
    private ParticleSystem.MainModule windMain;
    private ParticleSystem.EmissionModule lightningEmission;
    private ParticleSystem.MainModule lightningMain;
    private ParticleSystem.EmissionModule fogEmission;
    private Transform snowTransform;

    public Material snowMat;

    void Awake()
    {
        autoUpdate = true;
        snowTransform = snowPart.transform;
        snowEmission = snowPart.emission;
        snowShape = snowPart.shape;
        snowForce = snowPart.forceOverLifetime;
        windEmission = windPart.emission;
        windMain = windPart.main;
        fogEmission = fogPart.emission;

        Vector3 v = MainCamera.transform.position;
        snowPart.transform.position = new Vector3(v.x, v.y + 65.0f, 0);

        UpdateAll();
    }

    void Update(){
        if (autoUpdate)
            UpdateAll();
    }

    void UpdateAll(){
        snowEmission.rateOverTime = 110f * masterIntensity * snowIntensity;
        snowShape.radius = 30f * Mathf.Clamp(windIntensity, 0.4f, 1f) * masterIntensity;
        snowForce.x = new ParticleSystem.MinMaxCurve(-9f * windIntensity, -3-14f * windIntensity);
        windEmission.rateOverTime = 14f * masterIntensity * (windIntensity + fogIntensity);
        windMain.startLifetime = 2f + 6f * (1f - windIntensity);
        windMain.startSpeed = new ParticleSystem.MinMaxCurve(15f * windIntensity, 20 * windIntensity);
        fogEmission.rateOverTime = (fogIntensity + (snowIntensity + windIntensity)*0.5f) * masterIntensity;
        snowMat.SetFloat("_SnowLevel", snowLevel);
    }

    public void OnMasterChanged(float value)
    {
        masterIntensity = value;
        UpdateAll();
    }
    public void OnSnowChanged(float value)
    {
        snowIntensity = value;
        UpdateAll();
    }
    public void OnWindChanged(float value)
    {
        windIntensity = value;
        UpdateAll();
    }
    public void OnFogChanged(float value)
    {
        fogIntensity = value;
        UpdateAll();
    }
    public void OnSnowLevelChanged(float value)
    {
        snowLevel = value;
        UpdateAll();
    }
}
