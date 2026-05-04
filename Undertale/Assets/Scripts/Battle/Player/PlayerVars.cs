using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class PlayerVars : MonoBehaviour
{
    public float atkValue;
    public float defValue;

    [SerializeField] Color soulFlashing;
    public Color soulOriginal;
    float soulAlpha;
    float soulAlphaFlash;
    float time;
    public float maxTime;
    private SpriteRenderer soulSprite;
    bool invincible;
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
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        playerData.health = 20;
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
            soulAlpha = soulSprite.color.a;
            soulAlphaFlash = soulAlpha / 2;
            soulOriginal = soulSprite.color;
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
        }
    }

    // Esta funcion hace parpadear el alma.
    void FlashSoul()
    {
        if (soulSprite == null)
        {
            invincible = false;
            time = maxTime;
            return;
        }

        if (time > 0)
        {
            soulAlphaFlash = soulAlphaFlash * -1;
            soulSprite.color = soulFlashing;
        }
        else
        {
            soulSprite.color = soulOriginal;
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
