using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialoguePrueba : MonoBehaviour
{
    //SerializeFields privados de los gameobjects necesarios
    //donde introduciremos en el editor lo que deseamos que se le aplique
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject portrait;
    [SerializeField, TextArea(4, 6)] private string[] dialogueLines;
    //strings donde pondremos lo que queremos que digan

    private bool isPlayerInRange;
    //comprobar si esta en rango el jugador
    private bool didDialogueStart;
    //comprobar si ha empezado el dialogo
    private int lineIndex;
    //la línea x la q va
    private bool isTyping;
    //comprobar si esta escribiendo

    private float typingTime = 0.05f;

    public UnityEvent onFinishDialogue;

    private void Update()
    {
        if (!didDialogueStart && isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            //comenzamos dialogo en el siguiente frame cuando esté en rango y no haya empezado
            //el dialogo, también debe cumplirse que se presione el espacio
            StartDialogue();
        }
        else if (didDialogueStart && Input.GetKeyDown(KeyCode.Space))
        {

            if (isTyping)
            {
                //sino, se salta la animación y muestra la línea completa
                StopAllCoroutines();
                dialogueText.text = dialogueLines[lineIndex];
                isTyping = false;
            }
            else
            {
                NextLine();//pasamos a la siguiente línea
            }
        }
    }

    private void StartDialogue()
    {//método que usaremos donde definimos cuales estan en true cuando comienza el dialogo
        didDialogueStart = true;
        dialoguePanel.SetActive(true);
        portrait.SetActive(true);

        lineIndex = 0;
        StartCoroutine(ShowLine());
    }

    private void NextLine()
    {
        //siguiente línea
        if (isTyping) return;

        lineIndex++;

        if (lineIndex < dialogueLines.Length)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            EndDialogue();//cuando sea igual o mayor se acaba el diálogo
        }
    }

    private IEnumerator ShowLine()
    {//mostrar línea
        isTyping = true;

        dialogueText.text = "";
        //escribir letra x letra
        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingTime);
        }

        isTyping = false;//terminar de escribir
    }

    private void EndDialogue() // Esto le puedo meter lo que quiera, por ejemplo cuando termina la chapa, puedo hacer que arranque la timeline
    {//método para cuando termine el dialogo, ponemos todo en false
        onFinishDialogue.Invoke();
        didDialogueStart = false;
        dialoguePanel.SetActive(false);
        portrait.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {//Cuando colisione con el collider y esté en Trigger
        if (collision.CompareTag("Player"))
        {
            //si es con el player estará en rango
            isPlayerInRange = true;

            StartDialogue(); // AUTO START
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //si no colisiona no ta en rango
            isPlayerInRange = false;
        }
    }
}
