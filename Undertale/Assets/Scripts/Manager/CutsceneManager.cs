using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
	public static CutsceneManager Instance;
	[SerializeField]
	public List<PlayableDirector> timelinesSans; //las timelines de sans
	[SerializeField]
	public List<PlayableDirector> timelinesPapyrus; //las timelines de papyrus
	[SerializeField]
	public List<PlayableDirector> timelinesPlayer; //las timelines del player

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	public void Play(string cutsceneID)
	{
		Debug.Log("Reproduciendo: " + cutsceneID);
		StartCoroutine(PlayCutscene(cutsceneID));
	}

	private IEnumerator PlayCutscene(string id)
	{
		if (id == "SansIntro")
		{
			yield return StartCoroutine(SansIntro());
		}
	}

	private IEnumerator SansIntro()
	{
		
		Debug.Log("Empieza SansIntro");

		timelinesSans[0].time = 0;
		timelinesSans[0].Play();
		yield return new WaitWhile(() => timelinesSans[0].state == PlayState.Playing);


		timelinesPlayer[0].time = 0;
		timelinesPlayer[0].Play();
		
		timelinesSans[1].time = 0;
		timelinesSans[1].Play();

		yield return new WaitWhile(() => timelinesPlayer[0].state == PlayState.Playing);

		timelinesPlayer[1].time = 0;
		timelinesPlayer[1].Play();

		yield return new WaitWhile(() => timelinesPlayer[1].state == PlayState.Playing);

		timelinesPapyrus[0].time = 0;
		timelinesPapyrus[0].Play();

		yield return new WaitWhile(() => timelinesPapyrus[0].state == PlayState.Playing);

		timelinesPapyrus[1].time = 0;
		timelinesPapyrus[1].Play();




		// 1. Bloquear jugador (si tienes sistema)
		// player.SetCanMove(false);

		// 2. Sans habla
		//dialogueManager.Show("Sans: hey kid...");

		//yield return dialogueManager.WaitUntilFinished();

		// 3. Animación Sans
		//sansAnimator.SetTrigger("Talk");

		//yield return new WaitForSeconds(1f);

		// 4. Papyrus entra
		//papyrusAnimator.SetTrigger("Enter");

		//yield return new WaitForSeconds(1f);

		// 5. Papyrus habla
		//dialogueManager.Show("Papyrus: NYEH HEH HEH!");

		//yield return dialogueManager.WaitUntilFinished();

		// 6. Fin cutscene
		Debug.Log("Fin SansIntro");

		yield return null;
		// player.SetCanMove(true);
	}
}

