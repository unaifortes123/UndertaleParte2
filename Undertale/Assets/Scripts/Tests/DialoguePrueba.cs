using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialoguePrueba : MonoBehaviour
{
	[SerializeField] private GameObject dialoguePanel;
	[SerializeField] private TMP_Text dialogueText;
	[SerializeField] private GameObject portrait;
	[SerializeField, TextArea(4, 6)] private string[] dialogueLines;

	private bool isPlayerInRange;
	private bool didDialogueStart;
	private int lineIndex;
	private bool isTyping;

	private float typingTime = 0.05f;

	public UnityEvent onFinishDialogue;

	// 🔵 NUEVO: control cutscene
	private bool isCutsceneMode = false;

	private void Update()
	{
		// 🔵 SI ESTAMOS EN CUTSCENE, IGNORAMOS INPUT
		if (isCutsceneMode) return;

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

	private void StartDialogue()
	{
		didDialogueStart = true;
		dialoguePanel.SetActive(true);
		portrait.SetActive(true);

		lineIndex = 0;
		StartCoroutine(ShowLine());
	}

	private void NextLine()
	{
		if (isTyping) return;

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

	private void EndDialogue()
	{
		onFinishDialogue.Invoke();

		didDialogueStart = false;
		dialoguePanel.SetActive(false);
		portrait.SetActive(false);
	}

	// 🟢 MODO MUNDO (trigger)
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			isPlayerInRange = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			isPlayerInRange = false;
		}
	}

	// -----------------------
	//  MODO CUTSCENE (las escenas que hay por el mapa)
	// -----------------------

	public void StartCutsceneDialogue(string[] lines)
	{
		isCutsceneMode = true;

		dialogueLines = lines;

		didDialogueStart = true;
		dialoguePanel.SetActive(true);
		portrait.SetActive(true);

		lineIndex = 0;
		StartCoroutine(CutsceneDialogueFlow());
	}

	private IEnumerator CutsceneDialogueFlow()
	{
		while (lineIndex < dialogueLines.Length)
		{
			yield return StartCoroutine(ShowLine());

			yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || isCutsceneMode);

			lineIndex++;
		}

		EndCutsceneDialogue();
	}

	private void EndCutsceneDialogue()
	{
		onFinishDialogue.Invoke();

		didDialogueStart = false;
		isCutsceneMode = false;

		dialoguePanel.SetActive(false);
		portrait.SetActive(false);
	}

	// 🔵 Para cutscenes: esperar a que termine
	public IEnumerator WaitUntilFinished()
	{
		yield return new WaitUntil(() => !didDialogueStart);
	}
}