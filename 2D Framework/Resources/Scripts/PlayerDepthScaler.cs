using UnityEngine;

public class PlayerDepthScaler : MonoBehaviour
{
    [Header("Paramètres de profondeur")]
    public float minY = -5f;              // Y minimal de la map
    public float maxY = 5f;               // Y maximal de la map
    public float minScale = 0.6f;         // Échelle minimale (en bas)
    public float maxScale = 1.2f;         // Échelle maximale (en haut)

    private Transform playerTransform;

    void Start()
    {
        playerTransform = transform;
    }

    void Update()
    {
        float y = playerTransform.position.y;
        float t = Mathf.InverseLerp(minY, maxY, y);
        float scale = Mathf.Lerp(minScale, maxScale, t);
        playerTransform.localScale = new Vector3(scale, scale, 1f);
    }
}
