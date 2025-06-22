using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxEffectWithLight : MonoBehaviour
{
    [Header("Parallax Settings")]
    public Transform cameraTransform;
    [Range(0f, 1f)] public float parallaxFactor = 0.5f;

    [Header("Light FX")]
    public bool enableLightPulse = true;
    public float pulseSpeed = 1f;
    public float pulseIntensity = 0.1f;

    [Header("Cloud Drift")]
    public bool enableCloudDrift = true;
    public Vector2 driftDirection = new Vector2(0.1f, 0f);
    public float driftSpeed = 0.2f;

    private Vector3 previousCamPos;
    private SpriteRenderer sr;
    private float initialAlpha;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        sr = GetComponent<SpriteRenderer>();
        previousCamPos = cameraTransform.position;

        initialAlpha = sr.color.a;
    }

    void LateUpdate()
    {
        Vector3 delta = cameraTransform.position - previousCamPos;
        transform.position += new Vector3(delta.x * parallaxFactor, delta.y * parallaxFactor, 0);
        previousCamPos = cameraTransform.position;

        if (enableCloudDrift)
            transform.position += (Vector3)(driftDirection.normalized * driftSpeed * Time.deltaTime);

        if (enableLightPulse)
            PulseAlpha();
    }

    void PulseAlpha()
    {
        float alpha = initialAlpha + Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
        Color c = sr.color;
        sr.color = new Color(c.r, c.g, c.b, Mathf.Clamp01(alpha));
    }
}
