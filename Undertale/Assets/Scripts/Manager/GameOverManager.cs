using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    private PlayerMovement player;
    public Sprite brokenSoul;
    public AudioClip soulBreakSfx;
    public AudioClip gameOverMusic;
    public AudioSource sfxPlayer;
    public AudioSource musicPlayer;
    public ParticleSystem deathParticles;
    public GameObject battleObjects;
    private SpriteRenderer playerSprite;
    public GameObject gameOverScreen;
    bool initiating;
    [SerializeField] string returnScene = "PruebaEntradaEnCombate";

    // Esta funcion busca el player y su sprite.
    void Start()
    {
        FindPlayerReferences();
    }

    // Esta funcion comprueba si el player ha muerto.
    void Update()
    {
        if (player == null || playerSprite == null)
        {
            FindPlayerReferences();
        }

        if (PlayerVars.instance == null) return;

        if (PlayerVars.instance.playerData.health <= 0 && !initiating)
        {
            StartCoroutine(DeathSequence());
        }
    }

    // Esta funcion vuelve a buscar las referencias del player.
    void FindPlayerReferences()
    {
        player = FindObjectOfType<PlayerMovement>();

        if (player != null)
        {
            playerSprite = player.GetComponent<SpriteRenderer>();
        }
    }

    // Esta funcion reproduce la muerte y vuelve al ultimo guardado.
    IEnumerator DeathSequence()
    {
        if (player == null) yield break;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        PlayerVars pv = PlayerVars.instance;

        if (rb != null)
            rb.velocity = Vector2.zero;

        if (pv != null && playerSprite != null)
            playerSprite.color = pv.soulOriginal;

        initiating = true;

        if (battleObjects != null)
            battleObjects.SetActive(false);

        player.enabled = false;

        if (musicPlayer != null)
            musicPlayer.clip = null;
        yield return new WaitForSeconds(1);

        if (playerSprite != null)
            playerSprite.sprite = brokenSoul;

        if (sfxPlayer != null)
        {
            sfxPlayer.clip = soulBreakSfx;
            sfxPlayer.Play();
        }

        yield return new WaitForSeconds(1.015f);

        if (playerSprite != null)
            playerSprite.enabled = false;

        if (deathParticles != null)
            deathParticles.Play();

        yield return new WaitForSeconds(1.5f);

        if (musicPlayer != null)
        {
            musicPlayer.clip = gameOverMusic;
            musicPlayer.Play();
        }

        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        if (SaveManager.instance != null && SaveManager.instance.LoadGameAndScene(returnScene))
        {
            yield break;
        }

        SceneManager.LoadScene(returnScene);
    }
}
