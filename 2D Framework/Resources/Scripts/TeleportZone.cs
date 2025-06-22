using UnityEngine;
using Fusion;

[RequireComponent(typeof(Collider2D))]
public class TeleportZone : NetworkBehaviour
{
    [Header("Teleport Settings")]
    public Transform destinationPoint;
    public bool instantCameraSnap = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!Runner || !Runner.IsRunning || destinationPoint == null)
            return;

        if (!other.TryGetComponent(out NetworkObject netObj))
            return;

        if (!netObj.HasInputAuthority)
            return;

        // Téléportation du joueur local
        netObj.transform.position = destinationPoint.position;

        // Repositionne la caméra locale (si unique et fixe)
        if (instantCameraSnap && Camera.main != null)
        {
            Camera.main.transform.position = destinationPoint.position + new Vector3(0, 0, -10);
        }

        Debug.Log($"[TeleportZone] Local player teleported to {destinationPoint.position}");
    }
}
