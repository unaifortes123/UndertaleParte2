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




		

		// 6. Fin cutscene
		Debug.Log("Fin SansIntro");

		yield return null;
		
	}
	private IEnumerator SansIntro2()
		{
		Debug.Log("Empieza SansIntro 2");

		timelinesSans[2].time = 0;
		timelinesSans[2].Play();
		yield return new WaitWhile(() => timelinesSans[2].state == PlayState.Playing);
	}
}

