using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light sun;

    [Header("Cycle Settings")]
    public float dayDuration = 120f; // Full day in seconds
    public Gradient lightColor;      // Color over time
    public AnimationCurve lightIntensity; // Intensity over time

    private float timeOfDay; // 0 → 1

    void Update()
    {
        timeOfDay += Time.deltaTime / dayDuration;
        timeOfDay %= 1f;

        // Rotate sun
        sun.transform.rotation = Quaternion.Euler((timeOfDay * 360f) - 90f, 170f, 0);

        // Adjust light intensity
        sun.intensity = lightIntensity.Evaluate(timeOfDay);

        // Adjust light color
        sun.color = lightColor.Evaluate(timeOfDay);
    }
}