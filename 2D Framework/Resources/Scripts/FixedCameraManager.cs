using UnityEngine;

public class CameraFixedZoneController : MonoBehaviour
{
    public static CameraFixedZoneController Instance;
    public Transform mainCamera;
    public Transform currentCamPoint;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Appelé pour déplacer la caméra à un point fixe.
    /// </summary>
    public void MoveCameraTo(Transform targetTransform)
    {
        if (targetTransform == null) return;

        Vector3 newPos = new Vector3(
            targetTransform.position.x,
            targetTransform.position.y,
            transform.position.z
        );

        transform.position = newPos;
    }
    void Start()
    {
        if (mainCamera != null)
            mainCamera.gameObject.tag = "MainCamera";

        if (currentCamPoint != null)
            MoveCameraTo(currentCamPoint);
    }

    private void OnEnable()
    {
        string camPointName = PlayerPrefs.GetString("SavedCamPoint", "");
        if (!string.IsNullOrEmpty(camPointName))
        {
            Transform savedPoint = GameObject.Find(camPointName)?.transform;
            if (savedPoint != null)
                MoveCameraTo(savedPoint);
        }
        if (mainCamera != null && !mainCamera.gameObject.activeSelf)
            mainCamera.gameObject.SetActive(true);
    }
}