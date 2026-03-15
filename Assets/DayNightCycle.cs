using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    public float dayDuration = 120f; // Full day length in seconds

    [Header("Skyboxes")]
    public Material daySkybox;
    public Material sunsetSkybox;
    public Material nightSkybox;

    [Header("Fog Settings")]
    public float dayFogDensity = 0.02f;
    public float nightFogDensity = 0.005f;

    [Header("Sun Settings")]
    public float sunYRotation = 170f; // Keeps your sun angled nicely

    private float timeOfDay; // 0 → 1

    void Start()
    {
        RenderSettings.fog = true;
    }

    void Update()
    {
        // Advance time
        timeOfDay += Time.deltaTime / dayDuration;
        timeOfDay %= 1f;

        RotateSun();
        UpdateSkybox();
        UpdateFog();
    }

    void RotateSun()
    {
        transform.rotation = Quaternion.Euler(
            (timeOfDay * 360f) - 90f,
            sunYRotation,
            0f
        );
    }

    void UpdateSkybox()
    {
        // Sunrise & Sunset (same skybox)
        if ((timeOfDay >= 0.20f && timeOfDay < 0.30f) ||
            (timeOfDay >= 0.70f && timeOfDay < 0.80f))
        {
            RenderSettings.skybox = sunsetSkybox;
        }
        // Day
        else if (timeOfDay >= 0.30f && timeOfDay < 0.70f)
        {
            RenderSettings.skybox = daySkybox;
        }
        // Night
        else
        {
            RenderSettings.skybox = nightSkybox;
        }

        DynamicGI.UpdateEnvironment();
    }

    void UpdateFog()
    {
        bool isNight = timeOfDay < 0.20f || timeOfDay >= 0.80f;

        float targetFog = isNight ? nightFogDensity : dayFogDensity;

        // Smooth blend
        RenderSettings.fogDensity = Mathf.Lerp(
            RenderSettings.fogDensity,
            targetFog,
            Time.deltaTime * 2f
        );

        // Optional subtle color shift
        RenderSettings.fogColor = isNight
            ? new Color(0.1f, 0.15f, 0.25f)
            : new Color(0.7f, 0.7f, 0.7f);
    }
}