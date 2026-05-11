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
	[SerializeField]
	public List<PlayableDirector> timelinesPuente; //las timelines que aparecen en el puente (por ejemplo el perrito dando vueltas)

    [SerializeField] private PlayerController playerController; // esto principalmente es para dejar al usuario paralizado, en e player controller
																// hay un set para dejar sin movimiento al player


	// ESTO SON LOS DIALOGOS, LISTAS QUE CONTIENEN LA INFO DE LO QUE DICEN, esta clasificado por las zonas :)
	[SerializeField] private DialogueLine[] sansIntro1_1;
	[SerializeField] private DialogueLine[] sansIntro1_2;
	[SerializeField] private DialogueLine[] sansIntro1_3;
	[SerializeField] private DialogueLine[] sansIntro1_4;
	[SerializeField] private DialogueLine[] sansIntro1_5;
	[SerializeField] private DialogueLine[] sansIntro1_6;
	[SerializeField] private DialogueLine[] sansIntro1_7;
	[SerializeField] private DialogueLine[] sansIntro1_8;
	[SerializeField] private DialogueLine[] sansIntro1_9;
	[SerializeField] private DialogueLine[] sansIntro1_10;

	[SerializeField] private DialogueLine[] papyrusIntro_1;
	[SerializeField] private DialogueLine[] papyrusIntro1_2;
	[SerializeField] private DialogueLine[] papyrusIntro1_3;
	[SerializeField] private DialogueLine[] papyrusIntro1_4;
	[SerializeField] private DialogueLine[] papyrusIntro1_5;
	[SerializeField] private DialogueLine[] papyrusIntro1_6;


	// DIALOGOS ZONAPUENTE
	[SerializeField] private DialogueLine[] puenteSans;
    [SerializeField] private DialogueLine[] puentePapyrus;

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
		else if (id == "SansIntro2")
		{
			yield return StartCoroutine(SansIntro2());
		}
		else if (id == "MinijuegoElectrico")
		{
			yield return StartCoroutine(MinijuegoElectrico());
		}

		else if (id == "SopaDeLetras1")
		{
			yield return StartCoroutine(SopaDeLetras1());
		}

		else if (id == "SopaDeLetras2")
		{
			yield return StartCoroutine(SopaDeLetras2());
		}

		else if (id == "Pinchos1")
		{
			yield return StartCoroutine(Pinchos1());
		}
		else if (id == "Pinchos2")
		{
			yield return StartCoroutine(Pinchos2());
		}
		else if (id == "Pinchos3")
		{
			yield return StartCoroutine(Pinchos3());
		}
		else if (id == "Colores")
		{
			yield return StartCoroutine(Colores());
		}
        else if (id == "Puente")
        {
            yield return StartCoroutine(Puente());
        }
    }

	private IEnumerator SansIntro()
	{
		
		Debug.Log("Empieza SansIntro");
        playerController.SetCanMove(false); // bloquear movimiento
        timelinesSans[0].time = 0;
		timelinesSans[0].Play();
		yield return new WaitWhile(() => timelinesSans[0].state == PlayState.Playing);

		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansIntro1_1)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansIntro1_2)
		);

		timelinesPlayer[0].time = 0;
		timelinesPlayer[0].Play();
		
		timelinesSans[1].time = 0;
		timelinesSans[1].Play();

		yield return new WaitWhile(() => timelinesPlayer[0].state == PlayState.Playing);

		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansIntro1_3)
		);

		timelinesPlayer[1].time = 0;
		timelinesPlayer[1].Play();
		yield return new WaitWhile(() => timelinesPlayer[1].state == PlayState.Playing);


		timelinesPapyrus[0].time = 0;
		timelinesPapyrus[0].Play();

		yield return new WaitWhile(() => timelinesPapyrus[0].state == PlayState.Playing);

		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansIntro1_4)
		);

		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusIntro_1)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansIntro1_5)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusIntro1_2)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansIntro1_6)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusIntro1_3)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansIntro1_7)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusIntro1_4)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansIntro1_8)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusIntro1_5)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansIntro1_9)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusIntro1_6)
		);

		timelinesPapyrus[1].time = 0;
		timelinesPapyrus[1].Play();
		yield return new WaitWhile(() => timelinesPapyrus[1].state == PlayState.Playing);

		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansIntro1_10)
		);

		
		playerController.SetCanMove(true); // bloquear movimiento


		// 6. Fin cutscene
		Debug.Log("Fin SansIntro");
		
	}
	private IEnumerator SansIntro2()
		{
		Debug.Log("Empieza SansIntro 2");
        playerController.SetCanMove(false); // bloquear movimiento
        timelinesSans[2].time = 0;
		timelinesSans[2].Play();
		yield return new WaitWhile(() => timelinesSans[2].state == PlayState.Playing);

		timelinesPapyrus[2].time = 0;
		timelinesPapyrus[2].Play();

		yield return new WaitWhile(() => timelinesPapyrus[2].state == PlayState.Playing);

		timelinesSans[3].time = 0;
		timelinesSans[3].Play();
		yield return new WaitWhile(() => timelinesSans[2].state == PlayState.Playing);

        playerController.SetCanMove(true); // bloquear movimiento
        Debug.Log("Termina SansIntro 2");
	}


	private IEnumerator MinijuegoElectrico(){
		Debug.Log("Empieza MinijuegoElectrico");

		timelinesPlayer[2].time = 0;
		timelinesPlayer[2].Play();

		yield return new WaitWhile(() => timelinesPlayer[2].state == PlayState.Playing);

		timelinesPapyrus[3].time = 0;
		timelinesPapyrus[3].Play();

		yield return new WaitWhile(() => timelinesPapyrus[3].state == PlayState.Playing);

		timelinesPapyrus[4].time = 0;
		timelinesPapyrus[4].Play();

		yield return new WaitWhile(() => timelinesPapyrus[4].state == PlayState.Playing);

		timelinesPlayer[3].time = 0;
		timelinesPlayer[3].Play();

		yield return new WaitWhile(() => timelinesPlayer[3].state == PlayState.Playing);

		timelinesPapyrus[5].time = 0;
		timelinesPapyrus[5].Play();

		yield return new WaitWhile(() => timelinesPapyrus[5].state == PlayState.Playing);

		Debug.Log("Termina MinijuegoElectrico");
	}

	private IEnumerator SopaDeLetras1(){
		Debug.Log("Empieza SopaDeLetras1");


		return null;
		Debug.Log("Termina SopaDeLetras1");
	}


	private IEnumerator SopaDeLetras2()
	{
		Debug.Log("Empieza SopaDeLetras2");

		timelinesPapyrus[6].time = 0;
		timelinesPapyrus[6].Play();

		yield return new WaitWhile(() => timelinesPapyrus[6].state == PlayState.Playing);

		Debug.Log("Termina SopaDeLetras2");
	}

	private IEnumerator Pinchos1()
	{
		Debug.Log("Empieza Pinchos1");

		timelinesPapyrus[7].time = 0;
		timelinesPapyrus[7].Play();

		yield return new WaitWhile(() => timelinesPapyrus[7].state == PlayState.Playing);

		Debug.Log("Termina Pinchos1");
	}

	private IEnumerator Pinchos2()
	{
		Debug.Log("Empieza Pinchos2");

		timelinesPapyrus[8].time = 0;
		timelinesPapyrus[8].Play();

		yield return new WaitWhile(() => timelinesPapyrus[8].state == PlayState.Playing);

		Debug.Log("Termina Pinchos2");
	}
	private IEnumerator Pinchos3()
	{
		Debug.Log("Empieza Pinchos3");

		timelinesPapyrus[9].time = 0;
		timelinesPapyrus[9].Play();

		yield return new WaitWhile(() => timelinesPapyrus[9].state == PlayState.Playing);

		Debug.Log("Termina Pinchos3");
	}

	private IEnumerator Colores()
	{
		Debug.Log("Empieza Colores");
		playerController.SetCanMove(false); // bloquear movimiento
		timelinesPapyrus[10].time = 0;
		timelinesPapyrus[10].Play();

		timelinesSans[4].time = 0;
		timelinesSans[4].Play();

		yield return new WaitWhile(() => timelinesPapyrus[10].state == PlayState.Playing);

		Debug.Log("Termina Colores");
	}

	private IEnumerator Puente() {
        Debug.Log("Empieza Puente");
        playerController.SetCanMove(false); // bloquear movimiento

        timelinesPuente[0].time = 0;
		timelinesPuente[0].Play();

        yield return new WaitWhile(() => timelinesPuente[0].state == PlayState.Playing);

        yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(puentePapyrus)
		);

        timelinesPuente[1].time = 0;
        timelinesPuente[1].Play();

        yield return new WaitWhile(() => timelinesPuente[1].state == PlayState.Playing);

        yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(puenteSans)
		);



        timelinesPapyrus[11].time = 0;
        timelinesPapyrus[11].Play();
        playerController.SetCanMove(true); // bloquear movimiento
        Debug.Log("Termina Puente");
    }
}

