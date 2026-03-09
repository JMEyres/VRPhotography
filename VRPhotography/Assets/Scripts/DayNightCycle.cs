using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public GameObject sun;
    public Light sunLight;
    public Gradient sunColorTimeline;
    [SerializeField, Header("Time"), Space]
    private float time, sunTime;
    [SerializeField]
    private float globalTime;
    [SerializeField]
    private float timeScale;
    
    [SerializeField, Header("Day variables")]
    private float sunSizeConvergenceDay = 3.48f;
    [SerializeField]
    private float atmosphereThicknessDay = 2.06f;
    [SerializeField]
    private float exposureDay = 1.0f;
    
    [SerializeField, Header("Night variables")]

    private float sunSizeConvergenceNight = 3.62f;
    [SerializeField]
    private float atmosphereThicknessNight = 0.34f;
    [SerializeField]
    private float exposureNight = 0.5f;

    private bool flip;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        globalTime += Time.deltaTime * timeScale;
        var _sign = flip ? 1 : -1;
        time += (Time.deltaTime * _sign) * timeScale;
        sunTime += Time.deltaTime * timeScale;
        var sunRot = RangeMap(sunTime, 0, 15, -20, 200);
        sun.transform.localEulerAngles = new Vector3 (sunRot,0,0);
        var _lerpTime = time/15;
        
        sunLight.color = sunColorTimeline.Evaluate(_lerpTime);
        UnityEngine.RenderSettings.skybox.SetFloat("_SunSizeConvergence", Mathf.Lerp(sunSizeConvergenceDay, sunSizeConvergenceNight, _lerpTime));
        UnityEngine.RenderSettings.skybox.SetFloat("_AtmosphereThickness", Mathf.Lerp(atmosphereThicknessDay, atmosphereThicknessNight, _lerpTime));
        UnityEngine.RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(exposureDay, exposureNight, _lerpTime));

        if (time >= 15 || time <= 0 && _sign == -1)
        { 
            flip = !flip;
            sunTime = 0;

        }
    }
    float RangeMap(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);
    }
}
