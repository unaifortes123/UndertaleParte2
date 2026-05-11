using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class PlayerVars : MonoBehaviour
{
    // ataque y defensa del player que usa para calcular daño
    public float atkValue;
    public float defValue;

    // color original del alma para volverlo cuando deje de parpadear
    public Color soulOriginal;
    float soulFlashTimer; // contador para ir alternando visible / no visible
    float time; // cuanto le queda de invulnerable
    public float maxTime; // tiempo total que dura la invulnerabilidad cuando le pegan
    private SpriteRenderer soulSprite; // sprite del corazon que va a hacer parpadear
    bool soulFlashVisible; // toggle para saber si toca verlo o no en este frame
    bool invincible; // bandera de si ahora mismo no puede recibir daño
    const float SOUL_FLASH_INTERVAL = 0.08f; // cada cuanto cambia visible <-> oculto
    const float SOUL_FLASH_ALPHA = 0.35f; // que tan transparente se ve cuando "desaparece"
    [HideInInspector]
    public static PlayerVars instance; // singleton para que todos accedan al player

    // clase que mete dentro y que se guarda/carga en el JSON del save
    [System.Serializable]
    public class PlayerData
    {
        public float health; // vida actual del player
        public float score; // puntuacion que lleva
        public string playerName; // como se llama (lo escribe al principio)

        // lista de combates que ya ha pasado para no repetirlos
        public List<string> completedFights = new List<string>();

        // resumen rapido por si lo quiere imprimir en consola
        public override string ToString()
        {
            return "Name: " + playerName + "HP:" + health + "Score: " + score;
        }

        // devuelve la vida que tiene ahora
        public float GetHealth()
        {
            return health;
        }

        // le pone la vida que nos pasen
        public void SetHealth(float hp)
        {
            this.health = hp;
        }

        // lo cura al maximo, deja 20 que es la vida tope
        public void getMaxhealth()
        {
            this.health = 20;
        }

        // devuelve el score
        public float setScore()
        {
            return score;
        }

        // devuelve el nombre del player
        public string GetPlayerName()
        {
            return playerName;
        }

        // le pone el nombre que nos pasen
        public void SetPlayerName(string playerName)
        {
            this.playerName = playerName;
        }
    }

    // datos del player que se serializan (vida, score, nombre, combates pasados)
    public PlayerData playerData = new PlayerData();

    // patron singleton, solo deja un PlayerVars vivo en todo el juego
    void Awake()
    {
        bool canInitialize;

        canInitialize = true;

        // si ya hay otro PlayerVars se carga este para no duplicar
        if (instance != null && instance != this)
        {
            Destroy(this);
            canInitialize = false;
        }

        if (canInitialize == true)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // que sobreviva entre escenas

            playerData.health = 20; // empieza con la vida a tope
        }
    }

    // arranca el contador de invulnerabilidad en su maximo
    void Start()
    {
        time = maxTime;
    }

    // pilla el sprite del alma desde fuera para luego hacerlo parpadear
    public void SetSoulSprite(SpriteRenderer newSoulSprite)
    {
        soulSprite = newSoulSprite;

        // guarda el color original para volver a el cuando deje de parpadear
        if (soulSprite != null)
        {
            soulOriginal = soulSprite.color;
            soulFlashTimer = 0;
            soulFlashVisible = true;
            time = maxTime;
        }
    }

    // suelta el sprite del alma (cuando se cambia de turno o algo asi)
    public void ClearSoulSprite()
    {
        soulSprite = null;
    }

    // le quita vida al player y lo deja invulnerable un rato para que no muera de golpe
    public void TakeDamage(float damageTaken)
    {
        if (!invincible)
        {
            playerData.health -= Mathf.Max(0, damageTaken);

            // suena el "ay" cuando recibe daño
            if (AudioManager.instance != null)
            {
                AudioManager.instance.takingDamage();
            }

            invincible = true;
            soulFlashTimer = 0;
            soulFlashVisible = true;
        }
    }

    // hace que el alma vaya alternando entre visible y semi-transparente para el efecto parpadeo
    void FlashSoul()
    {
        Color flashColor;

        flashColor = soulOriginal;

        // si no hay alma no hay nada que parpadear, sale del modo invulnerable
        if (soulSprite == null)
        {
            invincible = false;
            time = maxTime;
        }
        else if (time > 0)
        {
            soulFlashTimer += Time.deltaTime;

            // cuando pasa el intervalo, cambia el toggle
            if (soulFlashTimer >= SOUL_FLASH_INTERVAL)
            {
                soulFlashTimer = 0;
                soulFlashVisible = !soulFlashVisible;
            }

            // si toca "oculto" baja el alpha para que se vea medio transparente
            if (!soulFlashVisible)
            {
                flashColor.a = SOUL_FLASH_ALPHA;
            }

            soulSprite.color = flashColor;
        }
        else
        {
            // se acaba el tiempo, devuelve todo a su sitio y deja de ser invulnerable
            soulSprite.color = soulOriginal;
            soulFlashTimer = 0;
            soulFlashVisible = true;
            time = maxTime;
            invincible = false;
        }
    }

    // si esta invulnerable va restando tiempo y haciendo parpadear el alma
    void Update()
    {
        if (invincible)
        {
            time -= Time.deltaTime;
            FlashSoul();
        }
    }
}
