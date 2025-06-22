using UnityEngine;


public class DayNightCycle : MonoBehaviour
{
    [Header("Durée d’un cycle (en secondes)")]
    public float fullDayDuration = 120f;
    [Range(0f, 1f)] public float currentTime = 0f; // 0 = minuit, 0.5 = midi
    public Gradient lightColorGradient;
    public AnimationCurve lightIntensityCurve;

    [Header("Références URP")]
    public UnityEngine.Rendering.Universal.Light2D globalLight; // Référence à la lumière 2D globale

    void Update()
    {
        currentTime += Time.deltaTime / fullDayDuration;
        if (currentTime > 1f)
            currentTime -= 1f;

        UpdateLighting(currentTime);
    }

    void UpdateLighting(float t)
    {
        if (globalLight != null)
        {
            globalLight.color = lightColorGradient.Evaluate(t);
            globalLight.intensity = lightIntensityCurve.Evaluate(t);
        }
    }
}
