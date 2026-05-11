using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// el minijuego de FIGHT: la barra con el puntero que se mueve y das al espacio en el centro para hacer mas daño
public class Attacking : MonoBehaviour
{
    public static Attacking instance;
    void Awake()
    {
        instance = this;
    }
    float time;
    public float maxTime; // cuanto tarda el puntero en cruzar la barra
    float progress; // 0 = izquierda, 1 = derecha
    float playerDamage; // ataque base del player que nos pasa el FightButton
    float damageDealt; // daño final que se aplica al enemigo
    float curTime;
    bool flicker; // toggle del parpadeo del puntero cuando aciertas
    bool finished = true;
    public Sprite original;
    public Sprite reverse;
    public Transform pointerObject; // la flecha que se mueve
    public Vector2 leftPos; // extremo izquierdo de la barra
    public Vector2 rightPos; // extremo derecho
    EnemyVars enemy;
    public bool isAttacking; // true mientras la barra esta moviendose
    [SerializeField] private SpriteRenderer attackBg;
    public GameObject normal; // sprite del enemigo en estado normal
    public GameObject damaged; // sprite del enemigo cuando recibe daño
    public TextMeshPro damageTxt; // texto que sale con el numero de daño o "MISS"
    public Color missColor;
    public Color damageColor;
    private EnemyVars stats;
    private PlayerVars statsPl;

    void Start()
    {
        // pilla al enemigo activo para luego poder restarle defensa al calcular daño
        enemy = FindObjectOfType<EnemyVars>();
    }

    // formula del daño segun donde pares el puntero. Maximo en el centro y baja en los extremos (forma triangular)
    float PointerProgressToAttackMultiplier(float progress)
    {
        return Mathf.Min(progress * (playerDamage * 2), (1 - progress) * (playerDamage * 2));
    }

    void Update()
    {
        // va calculando el daño potencial cada frame segun la posicion actual del puntero
        damageDealt = Mathf.Round(PointerProgressToAttackMultiplier(progress)) - enemy.defendValue;
        if (!finished)
        {
            curTime += Time.deltaTime;
        }

        if (isAttacking)
        {
            // mueve el puntero de izquierda a derecha interpolando con el tiempo
            progress = time / maxTime;
            pointerObject.position = Vector2.Lerp(leftPos, rightPos, progress * 1.2f);

            time += Time.deltaTime;
            if (time > 0.1f) // pequeño margen para que no se pueda atacar antes de que arranque la barra
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    AudioManager.instance.Slashing();
                    curTime = 0;
                    finished = false;
                    StartCoroutine(Flashing());
                    StartCoroutine(AfterAttack());
                    StartCoroutine(Damage());
                }
            }
        }

    }

    // se llama al pulsar FIGHT, activa la barra y prepara el ataque
    public void StartAttacking(float playerDmg)
    {
        isAttacking = true;
        playerDamage = playerDmg;
        attackBg.enabled = true;
        pointerObject.gameObject.SetActive(true);
    }

    // cambia el sprite del enemigo al de "golpeado" y muestra el numero de daño (o MISS si fue 0)
    IEnumerator Damage()
    {
        normal.SetActive(false);
        damaged.SetActive(true);
        if (damageDealt <= 0)
        {
            damageTxt.text = "MISS";
            damageTxt.color = missColor;
        }

        if (damageDealt > 0)
        {
            damageTxt.text = damageDealt.ToString();
        }
        yield return new WaitForSeconds(0.5f);
        // vuelve al estado normal
        normal.SetActive(true);
        damaged.SetActive(false);
        damageTxt.text = "";
        damageTxt.color = damageColor;
    }

    // resta vida al enemigo, oculta la barra y le da la vuelta para el siguiente turno (asi va alternando direccion)
    IEnumerator AfterAttack()
    {
        if (damageDealt > 0)
        {
            enemy.curHP -= damageDealt;
        }
        isAttacking = false;
        yield return new WaitForSeconds(1);
        attackBg.enabled = false;
        pointerObject.gameObject.SetActive(false);
        time = 0;
        // invierte los extremos para que el proximo turno el puntero vaya al otro lado
        leftPos.x = leftPos.x * -1;
        rightPos.x = rightPos.x * -1;
    }

    // parpadeo del puntero durante 0.75s alternando dos sprites para que se vea el "golpe"
    IEnumerator Flashing()
    {
        while (curTime < 0.75f)
        {
            flicker = !flicker;
            pointerObject.GetComponent<SpriteRenderer>().sprite = flicker ? original : reverse;

            yield return new WaitForSeconds(0.1f);
        }
        pointerObject.GetComponent<SpriteRenderer>().sprite = original;
        finished = true;
    }
}
