using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class PlayerVars : MonoBehaviour
{
    public float atkValue;
    public float defValue;

    public Color soulOriginal;
    float soulFlashTimer;
    float time;
    public float maxTime;
    private SpriteRenderer soulSprite;
    bool soulFlashVisible;
    bool invincible;
    const float SOUL_FLASH_INTERVAL = 0.08f;
    const float SOUL_FLASH_ALPHA = 0.35f;
    [HideInInspector]
    public static PlayerVars instance;

    [System.Serializable]
    public class PlayerData
    {
        public float health;
        public float score;
        public string playerName;

        public List<string> completedFights = new List<string>();

        // Esta funcion devuelve un resumen de los datos del player.
        public override string ToString()
        {
            return "Name: " + playerName + "HP:" + health + "Score: " + score;
        }

        // Esta funcion devuelve la vida actual.
        public float GetHealth()
        {
            return health;
        }

        // Esta funcion cambia la vida actual.
        public void SetHealth(float hp)
        {
            this.health = hp;
        }

        // Esta funcion pone la vida al maximo.
        public void getMaxhealth()
        {
            this.health = 20;
        }

        // Esta funcion devuelve la puntuacion.
        public float setScore()
        {
            return score;
        }

        // Esta funcion devuelve el nombre del player.
        public string GetPlayerName()
        {
            return playerName;
        }

        // Esta funcion cambia el nombre del player.
        public void SetPlayerName(string playerName)
        {
            this.playerName = playerName;
        }
    }

    public PlayerData playerData = new PlayerData();

    // Esta funcion deja una sola instancia de PlayerVars.
    void Awake()
    {
        bool canInitialize;

        canInitialize = true;

        if (instance != null && instance != this)
        {
            Destroy(this);
            canInitialize = false;
        }

        if (canInitialize == true)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            playerData.health = 20;
        }
    }

    // Esta funcion prepara el tiempo de invulnerabilidad.
    void Start()
    {
        time = maxTime;
    }

    // Esta funcion guarda el sprite del alma.
    public void SetSoulSprite(SpriteRenderer newSoulSprite)
    {
        soulSprite = newSoulSprite;

        if (soulSprite != null)
        {
            soulOriginal = soulSprite.color;
            soulFlashTimer = 0;
            soulFlashVisible = true;
            time = maxTime;
        }
    }

    // Esta funcion limpia el sprite del alma.
    public void ClearSoulSprite()
    {
        soulSprite = null;
    }

    // Esta funcion quita vida al player.
    public void TakeDamage(float damageTaken)
    {
        if (!invincible)
        {
            playerData.health -= Mathf.Max(0, damageTaken);

            if (AudioManager.instance != null)
            {
                AudioManager.instance.takingDamage();
            }

            invincible = true;
            soulFlashTimer = 0;
            soulFlashVisible = true;
        }
    }

    // Esta funcion hace parpadear el alma.
    void FlashSoul()
    {
        Color flashColor;

        flashColor = soulOriginal;

        if (soulSprite == null)
        {
            invincible = false;
            time = maxTime;
        }
        else if (time > 0)
        {
            soulFlashTimer += Time.deltaTime;

            if (soulFlashTimer >= SOUL_FLASH_INTERVAL)
            {
                soulFlashTimer = 0;
                soulFlashVisible = !soulFlashVisible;
            }

            if (!soulFlashVisible)
            {
                flashColor.a = SOUL_FLASH_ALPHA;
            }

            soulSprite.color = flashColor;
        }
        else
        {
            soulSprite.color = soulOriginal;
            soulFlashTimer = 0;
            soulFlashVisible = true;
            time = maxTime;
            invincible = false;
        }
    }

    // Esta funcion actualiza la invulnerabilidad.
    void Update()
    {
        if (invincible)
        {
            time -= Time.deltaTime;
            FlashSoul();
        }
    }
}
