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
    [SerializeField] string returnScene = "MainLevel";

    void Start()
    {
        FindPlayerReferences();
    }

    // mira cada frame si la vida ha llegado a 0 para disparar la secuencia de muerte
    void Update()
    {
        // si por lo que sea se pierde la referencia (cambio de escena, etc.) la vuelve a pillar
        if (player == null || playerSprite == null)
        {
            FindPlayerReferences();
        }

        if (PlayerVars.instance != null)
        {
            if (PlayerVars.instance.playerData.health <= 0 && !initiating)
            {
                StartCoroutine(DeathSequence());
            }
        }
    }

    void FindPlayerReferences()
    {
        player = FindObjectOfType<PlayerMovement>();

        if (player != null)
        {
            playerSprite = player.GetComponent<SpriteRenderer>();
        }
    }

    // secuencia de muerte: alma rota, sonido, particulas, pantalla de game over y carga del ultimo save
    IEnumerator DeathSequence()
    {
        Rigidbody2D rb;
        PlayerVars pv;
        bool loadedSavedGame;

        rb = null;
        pv = null;
        loadedSavedGame = false;

        if (player != null)
        {
            rb = player.GetComponent<Rigidbody2D>();
            pv = PlayerVars.instance;

            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }

            if (pv != null && playerSprite != null)
            {
                playerSprite.color = pv.soulOriginal;
            }

            initiating = true;

            if (battleObjects != null)
            {
                battleObjects.SetActive(false);
            }

            player.enabled = false;

            if (musicPlayer != null)
            {
                musicPlayer.clip = null;
            }

            yield return new WaitForSeconds(1);

            if (playerSprite != null)
            {
                playerSprite.sprite = brokenSoul;
            }

            if (sfxPlayer != null)
            {
                sfxPlayer.clip = soulBreakSfx;
                sfxPlayer.Play();
            }

            yield return new WaitForSeconds(1.015f);

            if (playerSprite != null)
            {
                playerSprite.enabled = false;
            }

            if (deathParticles != null)
            {
                deathParticles.Play();
            }

            yield return new WaitForSeconds(1.5f);

            if (musicPlayer != null)
            {
                musicPlayer.clip = gameOverMusic;
                musicPlayer.Play();
            }

            if (gameOverScreen != null)
            {
                gameOverScreen.SetActive(true);
            }

            yield return new WaitForSeconds(1.5f);

            if (SaveManager.instance != null)
            {
                loadedSavedGame = SaveManager.instance.LoadGameAndScene(returnScene);
            }

            if (loadedSavedGame == false)
            {
                SceneManager.LoadScene(returnScene);
            }
        }
    }
}
