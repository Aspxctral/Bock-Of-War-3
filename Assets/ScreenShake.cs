using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance;

    [Header("Default Shake Settings")]
    [Range(0.05f, 0.5f)] public float intensity = 0.15f;
    [Range(0.05f, 0.5f)] public float duration = 0.18f;

    private Vector3 originalPosition;
    private Coroutine shakeRoutine;

    void Awake()
    {
        Instance = this;
    }

    public void Shake(float intensityOverride = -1f, float durationOverride = -1f)
    {
        float useInt = intensityOverride > 0 ? intensityOverride : intensity;
        float useDur = durationOverride > 0 ? durationOverride : duration;

        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(DoShake(useInt, useDur));
    }

    IEnumerator DoShake(float intensity, float duration)
    {
        originalPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            transform.localPosition = originalPosition + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}