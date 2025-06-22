using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class DialogueBubbleChat : MonoBehaviour
{
    public UnityEngine.UI.Text dialogueText;
    public GameObject dialogueBox;
    public string[] dialogueLines;
    private int currentLine = 0;

    void Start()
    {
        dialogueBox.SetActive(false);
    }

    public void StartDialogue()
    {
        dialogueBox.SetActive(true);
        currentLine = 0;
        dialogueText.text = dialogueLines[currentLine];
    }

    public void DisplayNextLine()
    {
        if (currentLine < dialogueLines.Length - 1)
        {
            currentLine++;
            dialogueText.text = dialogueLines[currentLine];
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        dialogueBox.SetActive(false);
    }
}
