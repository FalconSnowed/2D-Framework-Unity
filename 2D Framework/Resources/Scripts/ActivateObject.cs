using UnityEngine;

public class ActivateObject : MonoBehaviour
{
    [Header("Target Object to Activate")]
    public GameObject target;

    [Tooltip("Activer automatiquement à Start()")]
    public bool activateOnStart = false;

    public void Activate()
    {
        if (target != null)
            target.SetActive(true);
    }

    private void Start()
    {
        if (activateOnStart)
            Activate();
    }
}
