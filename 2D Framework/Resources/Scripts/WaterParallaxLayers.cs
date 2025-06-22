using UnityEngine;

public class WaterParallaxLayers : MonoBehaviour
{
    public Renderer layer1;
    public Renderer layer2;

    public Vector2 speedLayer1 = new Vector2(0.01f, 0.005f);
    public Vector2 speedLayer2 = new Vector2(-0.008f, 0.003f);

    private Vector2 offset1;
    private Vector2 offset2;

    void Update()
    {
        if (layer1 && layer2)
        {
            offset1 += speedLayer1 * Time.deltaTime;
            offset2 += speedLayer2 * Time.deltaTime;

            layer1.material.mainTextureOffset = offset1;
            layer2.material.mainTextureOffset = offset2;
        }
    }
}
