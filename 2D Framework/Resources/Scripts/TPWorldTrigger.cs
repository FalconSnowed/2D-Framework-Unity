using UnityEngine;

public class TPWorldTrigger : MonoBehaviour
{
    public Transform teleportDestination;
    public Transform cameraPointForZone;

    [Header("Audio par zone")]
    public int destinationZoneIndex;
    public AudioClip zone0MusicClip;
    public AudioClip zone1MusicClip;
    public AudioClip zone2MusicClip;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        TeleportPlayer(other.gameObject);
    }

    private void TeleportPlayer(GameObject player)
    {
        player.transform.position = teleportDestination.position;

        if (CameraFixedZoneController.Instance != null)
        {
            CameraFixedZoneController.Instance.MoveCameraTo(cameraPointForZone);
        }

        if (MusicManager.Instance != null)
        {
            AudioClip clipToPlay = null;
            switch (destinationZoneIndex)
            {
                case 0: clipToPlay = zone0MusicClip; break;
                case 1: clipToPlay = zone1MusicClip; break;
                case 2: clipToPlay = zone2MusicClip; break;
            }
            if (clipToPlay != null && MusicManager.Instance.musicSource.clip != clipToPlay)
            {
                MusicManager.Instance.musicSource.clip = clipToPlay;
                MusicManager.Instance.musicSource.Play();
            }
        }
    }
}