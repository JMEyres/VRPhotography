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
    public float dayLength = 15f;
    
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
        // 1. Constant progression (no more flipping!)
        globalTime += Time.deltaTime * timeScale;
        time += Time.deltaTime * timeScale;

        // 2. The "Reset": When we hit the end of the day, start back at 0
        if (time >= dayLength) 
        {
            time = 0;
        }

        // 3. Normalized 0 to 1 value
        float solarPercent = time / dayLength;

        // 4. Smooth 360 rotation
        // 0 = Sunrise, 90 = Midday, 180 = Sunset, 270 = Midnight
        float sunRot = solarPercent * 360f;
        sun.transform.localEulerAngles = new Vector3(sunRot, 0, 0);

        // 5. Apply the Gradient
        sunLight.color = sunColorTimeline.Evaluate(solarPercent);

        // 6. Intensity Logic: Turn off the sun when it's under the floor (180 to 360 degrees)
        if (sunRot > 180f) {
            sunLight.intensity = 0;
        } else {
            sunLight.intensity = exposureDay;
        }

            // 7. Update Skybox variables based on whether it is Day or Night
        // This makes the transition happen ONLY when the sun is above/below the horizon
        float skyboxLerp = (solarPercent > 0.5f) ? 1.0f : solarPercent * 2.0f; 

        RenderSettings.skybox.SetFloat("_SunSizeConvergence", Mathf.Lerp(sunSizeConvergenceDay, sunSizeConvergenceNight, skyboxLerp));
        RenderSettings.skybox.SetFloat("_AtmosphereThickness", Mathf.Lerp(atmosphereThicknessDay, atmosphereThicknessNight, skyboxLerp));
        RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(exposureDay, exposureNight, skyboxLerp));
    }
    float RangeMap(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);
    }
}
