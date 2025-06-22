using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class CameraTraveling : MonoBehaviour
{
    [Header("Points de parcours")]
    public List<Transform> waypoints;

    [Header("Options")]
    public float travelSpeed = 2f;
    public float waitTimeBetweenPoints = 1f;
    public AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool loop = false;

    private Camera cam;
    private int currentIndex = 0;
    private bool isTraveling = false;

    private void Start()
    {
        cam = GetComponent<Camera>();

        if (waypoints.Count >= 2)
            StartCoroutine(TravelRoutine());
        else
            Debug.LogWarning("Définis au moins 2 points dans la liste des waypoints !");
    }

    private IEnumerator TravelRoutine()
    {
        isTraveling = true;

        while (true)
        {
            Transform startPoint = waypoints[currentIndex];
            Transform endPoint = waypoints[(currentIndex + 1) % waypoints.Count];

            float elapsed = 0f;
            float distance = Vector3.Distance(startPoint.position, endPoint.position);
            float duration = distance / travelSpeed;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float easedT = easingCurve.Evaluate(t);
                transform.position = Vector3.Lerp(startPoint.position, endPoint.position, easedT);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = endPoint.position;
            yield return new WaitForSeconds(waitTimeBetweenPoints);

            currentIndex++;
            if (currentIndex >= waypoints.Count - 1)
            {
                if (loop)
                    currentIndex = 0;
                else
                    break;
            }
        }

        isTraveling = false;
    }
}
