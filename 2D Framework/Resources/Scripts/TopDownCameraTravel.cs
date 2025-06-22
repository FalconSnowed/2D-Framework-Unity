using UnityEngine;

public class TopDownCameraTravel : MonoBehaviour
{
    [Header("Camera Settings")]
    public float travelSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10f);

    [Header("Target")]
    public Transform target; // Peut être défini dynamiquement

    private bool isTraveling = false;

    void Update()
    {
        if (target != null && isTraveling)
        {
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, travelSpeed * Time.deltaTime);

            // Stop if near enough
            if (Vector3.Distance(transform.position, desiredPosition) < 0.05f)
            {
                transform.position = desiredPosition;
                isTraveling = false;
            }
        }
    }

    public void MoveToTarget(Transform newTarget)
    {
        target = newTarget;
        isTraveling = true;
    }

    public void MoveToPosition(Vector3 worldPosition)
    {
        target = null;
        StartCoroutine(TravelTo(worldPosition + offset));
    }

    private System.Collections.IEnumerator TravelTo(Vector3 destination)
    {
        isTraveling = true;
        while (Vector3.Distance(transform.position, destination) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, destination, travelSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = destination;
        isTraveling = false;
    }
}
