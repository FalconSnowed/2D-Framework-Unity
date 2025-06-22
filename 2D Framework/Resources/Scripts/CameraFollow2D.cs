using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target; // Le joueur local
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothTime = 0.15f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
