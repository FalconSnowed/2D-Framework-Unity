using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    private bool dialogueActive = false;

    public GameObject dialoguePanel;
    public Text dialogueText; // ou TMP_Text si tu utilises TextMeshPro
    private string[] lines;
    private int currentLine;

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string[] dialogueLines)
    {
        if (dialogueActive) return; // ✅ empêche redéclenchement si déjà actif

        lines = dialogueLines;
        currentLine = 0;
        dialoguePanel.SetActive(true);
        dialogueActive = true;
        ShowNextLine();
    }


    void Update()
    {
        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            ShowNextLine();
        }
    }

    void ShowNextLine()
    {
        if (currentLine < lines.Length)
        {
            dialogueText.text = lines[currentLine];
            currentLine++;
        }
        else
        {
            EndDialogue();
        }
    }
    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueActive = false; // ✅ permet de redéclencher plus tard
    }

}
