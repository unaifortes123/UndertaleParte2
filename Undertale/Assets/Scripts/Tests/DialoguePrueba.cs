using System.Collections;
using TMPro;
using UnityEngine;

public class DialoguePrueba : MonoBehaviour
{
    // Referencias configuradas desde el Inspector.
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject portrait;
    [SerializeField, TextArea(4, 6)] private string[] dialogueLines;

    private bool isPlayerInRange;
    private bool didDialogueStart;
    private int lineIndex;
    private bool isTyping;

    private float typingTime = 0.05f;

    // Esta funcion se ejecuta cada frame y revisa la entrada o el estado actual.
    private void Update()
    {
        if (!didDialogueStart && isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            StartDialogue();
        }
        else if (didDialogueStart && Input.GetKeyDown(KeyCode.Space))
        {

            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = dialogueLines[lineIndex];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    // Esta funcion inicia un dialogo de prueba.
    private void StartDialogue()
    {
        didDialogueStart = true;
        dialoguePanel.SetActive(true);
        portrait.SetActive(true );

        lineIndex = 0;
        StartCoroutine(ShowLine());
    }

    // Esta funcion avanza a la siguiente linea del dialogo.
    private void NextLine()
    {
        if (isTyping == false)
        {
            lineIndex++;

            if (lineIndex < dialogueLines.Length)
            {
                StartCoroutine(ShowLine());
            }
            else
            {
                EndDialogue();
            }
        }
    }

    // Esta funcion muestra una linea escribiendola poco a poco.
    private IEnumerator ShowLine()
    {
        isTyping = true;

        dialogueText.text = "";
        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingTime);
        }

        isTyping = false;
    }

    // Esta funcion termina el dialogo actual.
    private void EndDialogue()
    {
        didDialogueStart = false;
        dialoguePanel.SetActive(false);
        portrait.SetActive(false );
    }

    // Esta funcion detecta cuando otro objeto entra en el trigger.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;

            StartDialogue();
        }
    }

    // Esta funcion detecta cuando otro objeto sale del trigger.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
