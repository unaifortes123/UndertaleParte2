	using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
	public GameObject sueloRojoMinijuego;
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

	// Guardar sprites para la escena

	[SerializeField] private Sprite sansUpSprite;
	[SerializeField] private Sprite papyrusUpSprite;

	[SerializeField] private Sprite sansDownSprite;
	[SerializeField] private Sprite papyrusDownSprite;
    

    [SerializeField] private SpriteRenderer sansSprite;
	[SerializeField] private SpriteRenderer papyrusSprite;
	[SerializeField] private SpriteRenderer palyerSprite;

	//Posicion de sans y papyrus
	[SerializeField] private Transform sans;
	[SerializeField] private Transform papyrus;


	[SerializeField] private PapyrusAnimationController papyrusAnimation; //vincular el cdigo para coger las animaciones de papyrus
    [SerializeField] private AniamtionPlayerController palyerAnimator;
    // ESTO SON LOS DIALOGOS, LISTAS QUE CONTIENEN LA INFO DE LO QUE DICEN, esta clasificado por las zonas :)


    // zona intro 1
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


	//zona intro 2
	[SerializeField] private DialogueLine[] sansIntro2_1;
	[SerializeField] private DialogueLine[] sansIntro2_2;
	[SerializeField] private DialogueLine[] sansIntro2_3;
	[SerializeField] private DialogueLine[] sansIntro2_4;

	[SerializeField] private DialogueLine[] papyrusIntro2_1;
	[SerializeField] private DialogueLine[] papyrusIntro2_2;
	[SerializeField] private DialogueLine[] papyrusIntro2_3;
	[SerializeField] private DialogueLine[] papyrusIntro2_4;
	[SerializeField] private DialogueLine[] papyrusIntro2_5;

	//Zona MinijuegoElectrico
	[SerializeField] private DialogueLine[] sansMinijuegoElectrico_1;
	[SerializeField] private DialogueLine[] sansMinijuegoElectrico_2;


	[SerializeField] private DialogueLine[] papyrusMinijuegoElectrico_1;
	[SerializeField] private DialogueLine[] papyrusMinijuegoElectrico_2;
	[SerializeField] private DialogueLine[] papyrusMinijuegoElectrico_3;
	[SerializeField] private DialogueLine[] papyrusMinijuegoElectrico_4;
	[SerializeField] private DialogueLine[] papyrusMinijuegoElectrico_5;
	[SerializeField] private DialogueLine[] papyrusMinijuegoElectrico_6;
	[SerializeField] private DialogueLine[] papyrusMinijuegoElectrico_7;
	[SerializeField] private DialogueLine[] papyrusMinijuegoElectrico_8;
	[SerializeField] private DialogueLine[] papyrusMinijuegoElectrico_9;


	//Zona sopa de letras 1
	[SerializeField] private DialogueLine[] sansSopaDeLetras1_1;

	[SerializeField] private DialogueLine[] papyrusSopaDeLetras1_1;
	[SerializeField] private DialogueLine[] papyrusSopaDeLetras1_2;

	//Zona sopa de letras 2
	[SerializeField] private DialogueLine[] sansSopaDeLetras2_1;
	[SerializeField] private DialogueLine[] sansSopaDeLetras2_2;
	

	[SerializeField] private DialogueLine[] papyrusSopaDeLetras2_1;
	[SerializeField] private DialogueLine[] papyrusSopaDeLetras2_2;
	[SerializeField] private DialogueLine[] papyrusSopaDeLetras2_3;
	[SerializeField] private DialogueLine[] papyrusSopaDeLetras2_4;

	[SerializeField] private DialogueLine[] palyerSopaDeLetras2_1;



	//Zona Pinchos 1
	[SerializeField] private DialogueLine[] papyrusPinchos1_1;
    [SerializeField] private DialogueLine[] papyrusPinchos1_2;

    [SerializeField] private DialogueLine[] playerPinchos1_1;


    //zona pinchos 2
    [SerializeField] private DialogueLine[] papyrusPinchos2_1;

    //zona minijuegoColores
    [SerializeField] private DialogueLine[] papyrusColores_1;
    [SerializeField] private DialogueLine[] papyrusColores_2;

    // DIALOGOS ZONAPUENTE
    [SerializeField] private DialogueLine[] puenteSans1;
    [SerializeField] private DialogueLine[] puenteSans2;
    [SerializeField] private DialogueLine[] puentePapyrus1;
    [SerializeField] private DialogueLine[] puentePapyrus2;
    [SerializeField] private DialogueLine[] puentePapyrus3;
    [SerializeField] private DialogueLine[] puentePapyrus4;
    [SerializeField] private DialogueLine[] puentePapyrus5;
   



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
		DialogueManager.instance.SetDialogueTop();

		timelinesSans[0].time = 0;
		timelinesSans[0].Play();
		yield return new WaitWhile(() => timelinesSans[0].state == PlayState.Playing);

		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansIntro1_1)
		);
		palyerAnimator.PlayAnimation("IdleLeft");
        Debug.Log(palyerSprite.sprite);
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

		DialogueManager.instance.ResetDialoguePosition();
		playerController.SetCanMove(true); // bloquear movimiento


		// Fin cutscene
		Debug.Log("Fin SansIntro");
		
	}



	private IEnumerator SansIntro2()
		{
		Debug.Log("Empieza SansIntro 2");
        playerController.SetCanMove(false); // bloquear movimiento
		sansSprite.sprite = sansDownSprite;
		papyrusSprite.sprite = papyrusUpSprite;
		sans.position = new Vector3(188f, 2.37f, 0f);
		papyrus.position = new Vector3(188.021f, 1.4f, 0f);
		DialogueManager.instance.SetDialogueTop();

		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusIntro2_1)
		);

		timelinesSans[2].time = 0;
		timelinesSans[2].Play();
		yield return new WaitWhile(() => timelinesSans[2].state == PlayState.Playing);

		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusIntro2_2)
		);

		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansIntro2_1)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusIntro2_3)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansIntro2_2)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusIntro2_4)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansIntro2_3)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusIntro2_5)
		);
		timelinesPapyrus[2].time = 0;
		timelinesPapyrus[2].Play();

		yield return new WaitWhile(() => timelinesPapyrus[2].state == PlayState.Playing);

		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansIntro2_4)
		);

		timelinesSans[3].time = 0;
		timelinesSans[3].Play();
		yield return new WaitWhile(() => timelinesSans[2].state == PlayState.Playing);
		DialogueManager.instance.ResetDialoguePosition();
		playerController.SetCanMove(true); // desblooquear movimiento
        Debug.Log("Termina SansIntro 2");
	}


	private IEnumerator MinijuegoElectrico(){
		Debug.Log("Empieza MinijuegoElectrico");

		playerController.SetCanMove(false); // bloquear movimiento
		papyrusSprite.sprite = papyrusUpSprite;
		sansSprite.sprite = sansDownSprite;
		sans.position = new Vector3(273.73f, 5.07f, 0f);
		papyrus.position = new Vector3(273.79f, 3.09f, 0f);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusMinijuegoElectrico_1)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansMinijuegoElectrico_1)
		);
		papyrusSprite.sprite = papyrusUpSprite;
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusMinijuegoElectrico_2)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusMinijuegoElectrico_3)
		);

		timelinesPlayer[2].time = 0;
		timelinesPlayer[2].Play();

		yield return new WaitWhile(() => timelinesPlayer[2].state == PlayState.Playing);

		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusMinijuegoElectrico_4)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusMinijuegoElectrico_5)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansMinijuegoElectrico_1)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusMinijuegoElectrico_6)
		);

		timelinesPapyrus[3].time = 0;
		timelinesPapyrus[3].Play();

		yield return new WaitWhile(() => timelinesPapyrus[3].state == PlayState.Playing);

		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusMinijuegoElectrico_7)
		);

		timelinesPapyrus[4].time = 0;
		timelinesPapyrus[4].Play();

		yield return new WaitWhile(() => timelinesPapyrus[4].state == PlayState.Playing);

		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusMinijuegoElectrico_8)
		);

		timelinesPlayer[3].time = 0;
		timelinesPlayer[3].Play();

		yield return new WaitWhile(() => timelinesPlayer[3].state == PlayState.Playing);

		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusMinijuegoElectrico_9)
		);

		timelinesPapyrus[5].time = 0;
		timelinesPapyrus[5].Play();

		yield return new WaitWhile(() => timelinesPapyrus[5].state == PlayState.Playing);

		playerController.SetCanMove(true); // desbloquear movimiento
		Debug.Log("Termina MinijuegoElectrico");
	}

	private IEnumerator SopaDeLetras1(){
		Debug.Log("Empieza SopaDeLetras1");
		papyrusSprite.sprite = papyrusUpSprite;
		sansSprite.sprite = sansDownSprite;
		sans.position = new Vector3(328f, -11.46f, 0f);
		papyrus.position = new Vector3(328f, -13.28f, 0f);
		playerController.SetCanMove(false); // bloquear movimiento
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusSopaDeLetras1_1)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusSopaDeLetras1_2)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansSopaDeLetras1_1)
		);
		playerController.SetCanMove(true); // desbloquear movimiento
		
		Debug.Log("Termina SopaDeLetras1");
	}


	private IEnumerator SopaDeLetras2()
	{
		Debug.Log("Empieza SopaDeLetras2");
		playerController.SetCanMove(false); // bloquear movimiento
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusSopaDeLetras2_1)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansSopaDeLetras2_1)
		);

		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusSopaDeLetras2_2)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(sansSopaDeLetras2_2)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusSopaDeLetras2_3)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(palyerSopaDeLetras2_1)
		);
		yield return StartCoroutine(
			DialogueManager.instance.ShowDialogue(papyrusSopaDeLetras2_4)
		);

		timelinesPapyrus[6].time = 0;
		timelinesPapyrus[6].Play();

		yield return new WaitWhile(() => timelinesPapyrus[6].state == PlayState.Playing);

		playerController.SetCanMove(true); // desbloquear movimiento
		Debug.Log("Termina SopaDeLetras2");
	}

	private IEnumerator Pinchos1()
	{
		Debug.Log("Empieza Pinchos1");
        papyrusSprite.sprite = papyrusUpSprite;
        papyrus.position = new Vector3(388.6f, -40.56f, 0f); 


        playerController.SetCanMove(false); // bloquear movimiento

        yield return StartCoroutine(
            DialogueManager.instance.ShowDialogue(papyrusPinchos1_1)
        );

        yield return StartCoroutine(
           DialogueManager.instance.ShowDialogue(playerPinchos1_1)
        );
        yield return StartCoroutine(
          DialogueManager.instance.ShowDialogue(papyrusPinchos1_2)
       );

        timelinesPapyrus[7].time = 0;
		timelinesPapyrus[7].Play();

		yield return new WaitWhile(() => timelinesPapyrus[7].state == PlayState.Playing);

        papyrus.position = new Vector3(0f, 0f, 0f);
        playerController.SetCanMove(true); // desbloquear movimiento
        Debug.Log("Termina Pinchos1");
	}

	private IEnumerator Pinchos2()
	{
		Debug.Log("Empieza Pinchos2");
        playerController.SetCanMove(false); // desbloquear movimiento


        timelinesPapyrus[8].time = 0;
		timelinesPapyrus[8].Play();

		yield return new WaitWhile(() => timelinesPapyrus[8].state == PlayState.Playing);

        yield return StartCoroutine(
           DialogueManager.instance.ShowDialogue(papyrusPinchos2_1)
        );

        timelinesPapyrus[9].time = 0;
        timelinesPapyrus[9].Play();

        yield return new WaitWhile(() => timelinesPapyrus[9].state == PlayState.Playing);
        papyrus.position = new Vector3(446.14f, -40.08f, 0f);
        playerController.SetCanMove(true); // desbloquear movimiento
        Debug.Log("Termina Pinchos2");
	}

	private IEnumerator Colores()
	{
		Debug.Log("Empieza Colores");
		playerController.SetCanMove(false); // bloquear movimiento

        yield return StartCoroutine(
           DialogueManager.instance.ShowDialogue(papyrusColores_1)
        );

        sueloRojoMinijuego.SetActive(true);

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


        yield return StartCoroutine(
           DialogueManager.instance.ShowDialogue(puentePapyrus1)
        );
        timelinesPuente[0].time = 0;
		timelinesPuente[0].Play();

        yield return new WaitWhile(() => timelinesPuente[0].state == PlayState.Playing);

        yield return StartCoroutine(
           DialogueManager.instance.ShowDialogue(puentePapyrus2)
        );
        yield return StartCoroutine(
           DialogueManager.instance.ShowDialogue(puenteSans1)
        );
        yield return StartCoroutine(
           DialogueManager.instance.ShowDialogue(puentePapyrus3)
        );
        yield return StartCoroutine(
           DialogueManager.instance.ShowDialogue(puenteSans2)
        );
        yield return StartCoroutine(
           DialogueManager.instance.ShowDialogue(puentePapyrus4)
        );

        timelinesPuente[1].time = 0;
        timelinesPuente[1].Play();

        yield return new WaitWhile(() => timelinesPuente[1].state == PlayState.Playing);

        yield return StartCoroutine(
           DialogueManager.instance.ShowDialogue(puentePapyrus5)
        );


        timelinesPapyrus[11].time = 0;
        timelinesPapyrus[11].Play();
        playerController.SetCanMove(true); // bloquear movimiento

        Debug.Log("Termina Puente");
    }
}

