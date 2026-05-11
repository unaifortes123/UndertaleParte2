using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [HideInInspector]
    public bool shouldTalk;
    public TextMeshPro text;
    public GameObject enemyTextBackground;
    public TextMeshPro textEnemy;
    public string dialogueTxt;
    public string enemyTxt;
    public AudioClip clip;
    public AudioClip enemyClip;
    private GameObject audioHolder;
    private List<AudioSource> sources;
    private Coroutine talkingRoutine;
    public bool done = true;
    private bool canNarrate = true;
    public float talkingSpeed = 0.1f;
    [HideInInspector]
    public static DialogueManager instance;

    void Awake()
    {
        instance = this;
    }

    // arranca un dialogo nuevo. Si habia uno en marcha lo corta y limpia los audios viejos
    public void Talking(Action talkAction)
    {
        EnsureReady();

        if (talkingRoutine != null)
        {
            StopCoroutine(talkingRoutine);
        }

        ClearAudioSources();
        talkingRoutine = StartCoroutine(DialogueRoutine(talkAction));
    }

    void Start()
    {
        EnsureReady();
        done = true;
        canNarrate = true;

        // si la escena ya trae texto inicial puesto, lo lanza directamente
        if (!string.IsNullOrWhiteSpace(dialogueTxt))
        {
            Talking(null);
        }
    }

    // primero escribe el texto del player y, si shouldTalk esta a true, luego el del enemigo en su bocadillo
    IEnumerator DialogueRoutine(Action action)
    {
        done = false;
        canNarrate = false;

        yield return TypeText(text, dialogueTxt, clip);
        yield return WaitForAudioSources();

        if (shouldTalk)
        {
            if (text != null)
            {
                text.text = "";
            }

            yield return EnemyTalking();
        }
        else if (enemyTextBackground != null)
        {
            enemyTextBackground.SetActive(false);
        }

        done = true;
        canNarrate = true;
        talkingRoutine = null;
        if (action != null)
        {
            action();
        }
    }

    // activa el bocadillo blanco del enemigo, lo pone por delante con sortingOrder y escribe la frase letra a letra
    IEnumerator EnemyTalking()
    {
        Renderer enemyBackgroundRenderer;
        Renderer enemyTextRenderer;

        if (enemyTextBackground != null)
        {
            enemyTextBackground.SetActive(true);

            enemyBackgroundRenderer = enemyTextBackground.GetComponent<Renderer>();

            if (enemyBackgroundRenderer != null)
            {
                enemyBackgroundRenderer.sortingOrder = 60;
            }
        }

        if (textEnemy != null)
        {
            textEnemy.color = Color.black;
            enemyTextRenderer = textEnemy.GetComponent<Renderer>();

            if (enemyTextRenderer != null)
            {
                enemyTextRenderer.sortingOrder = 61;
            }
        }

        yield return TypeText(textEnemy, enemyTxt, enemyClip != null ? enemyClip : clip);
        yield return WaitForAudioSources();

        if (enemyTextBackground != null)
        {
            enemyTextBackground.SetActive(false);
        }
    }

    // animacion clasica de Undertale: añade letra por letra al texto y suena un pitido por cada una
    IEnumerator TypeText(TextMeshPro targetText, string message, AudioClip textClip)
    {
        char[] chars;

        if (targetText != null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "* ...";
            }

            chars = message.ToCharArray();
            targetText.text = "";

            for (int i = 0; i < chars.Length; i++)
            {
                AudioSource s = audioHolder.AddComponent<AudioSource>();
                targetText.text += chars[i];
                s.clip = textClip;
                s.pitch = UnityEngine.Random.Range(0.99f, 1f);

                if (s.clip != null)
                {
                    s.Play();
                }

                sources.Add(s);
                yield return new WaitForSeconds(talkingSpeed);
            }
        }
    }

    // espera a que se acaben todos los pitidos antes de pasar a la siguiente fase, asi no se solapan dialogos
    IEnumerator WaitForAudioSources()
    {
        int playingSources;

        do
        {
            playingSources = 0;

            for (int i = 0; i < sources.Count; i++)
            {
                if (sources[i] == null || !sources[i].isPlaying)
                {
                    if (sources[i] != null)
                    {
                        Destroy(sources[i]);
                    }

                    sources.RemoveAt(i);
                    i--;
                }
                else
                {
                    playingSources++;
                }
            }

            yield return null;
        }
        while (playingSources > 0);
    }

    // limpia los AudioSources de la frase anterior para que no se acumule en memoria
    void ClearAudioSources()
    {
        if (sources == null)
        {
            sources = new List<AudioSource>();
        }
        else
        {
            for (int i = 0; i < sources.Count; i++)
            {
                if (sources[i] != null)
                {
                    Destroy(sources[i]);
                }
            }

            sources.Clear();
        }
    }

    // por si alguien llama Talking() antes de que se ejecute Start, inicializa la lista y el GameObject de audios
    void EnsureReady()
    {
        if (sources == null)
        {
            sources = new List<AudioSource>();
        }

        if (audioHolder == null)
        {
            audioHolder = new GameObject("Audio Holder");
            audioHolder.transform.parent = transform;
        }
    }
}
