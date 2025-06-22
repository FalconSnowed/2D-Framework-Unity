using UnityEngine;
using UnityEngine.UI;

public class DialTrigger : MonoBehaviour
{
    [TextArea(2, 5)]
    public string[] dialogueLines;

    public GameObject dialoguePanel;
    public Text dialogueText;

    private int currentLineIndex = 0;
    private bool isPlayerInRange = false;
    private bool dialogueActive = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!dialogueActive)
            {
                dialoguePanel.SetActive(true);
                dialogueText.text = dialogueLines[currentLineIndex];
                dialogueActive = true;
            }
            else
            {
                currentLineIndex++;

                if (currentLineIndex < dialogueLines.Length)
                {
                    dialogueText.text = dialogueLines[currentLineIndex];
                }
                else
                {
                    dialoguePanel.SetActive(false);
                    dialogueActive = false;
                    currentLineIndex = 0;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            dialoguePanel.SetActive(false);
            dialogueActive = false;
            currentLineIndex = 0;
        }
    }
}
