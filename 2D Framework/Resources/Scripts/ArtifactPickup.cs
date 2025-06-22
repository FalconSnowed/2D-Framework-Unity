using UnityEngine;

public class ArtifactPickup : MonoBehaviour
{
    public string artifactId = "Artifact_01"; // Unique par objet
    public QuestData linkedQuest; // Optionnel : associer à une quête

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Empêche la collecte si la quête n’est pas active
        if (linkedQuest != null && (!QuestManager.Instance.activeQuests.Contains(linkedQuest) || linkedQuest.isCompleted))
        {
            Debug.Log("⛔ Quête non active ou déjà complétée, impossible de récupérer cet artéfact.");
            return;
        }

        Debug.Log("🧿 Artéfact récupéré : " + artifactId);

        QuestManager.Instance.CollectArtifact(artifactId);

        if (linkedQuest != null)
        {
            QuestManager.Instance.RegisterArtifactCollection(linkedQuest, artifactId);
        }

        Destroy(gameObject); // Supprime l'objet de la scène
    }

}
