using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    public QuestData questToTrigger;
    private bool hasBeenTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasBeenTriggered && other.CompareTag("Player") && questToTrigger != null)
        {
            QuestManager.Instance.AddQuest(questToTrigger);
            hasBeenTriggered = true;

            // Optionnel : désactiver le trigger après déclenchement
            gameObject.SetActive(false);
        }
    }
}
