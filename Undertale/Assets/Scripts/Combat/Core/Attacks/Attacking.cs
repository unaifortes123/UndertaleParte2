using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Attacking : MonoBehaviour
{
    public static Attacking instance;
    // Singleton, deja una referencia para que cualquier script pueda llamar a este componente.
    void Awake()
    {
        instance = this;
    }
    float time;
    public float maxTime;
    float progress;
    float playerDamage;
    float damageDealt;
    float curTime;
    bool flicker;
    bool finished = true;
    public Sprite original;
    public Sprite reverse;
    public Transform pointerObject;
    public Vector2 leftPos;
    public Vector2 rightPos;
    EnemyVars enemy;
    public bool isAttacking;
    [SerializeField] private SpriteRenderer attackBg;
    public GameObject normal;
    public GameObject damaged;
    public TextMeshPro damageTxt;
    public Color missColor;
    public Color damageColor;
    private EnemyVars stats;
    private PlayerVars statsPl;

    // Localiza al enemigo de la escena para poder consultar su defensa al calcular el dano.
    void Start()
    {
        enemy = FindObjectOfType<EnemyVars>();
    }

    // Devuelve el dano segun donde paras el puntero, maximo en el centro y baja a los extremos.
    float PointerProgressToAttackMultiplier(float progress)
    {
        return Mathf.Min(progress * (playerDamage * 2), (1 - progress) * (playerDamage * 2));
    }

    // Mueve el puntero por la barra y, al pulsar Enter, dispara el calculo de dano y los efectos visuales.
    void Update()
    {
        damageDealt = Mathf.Round(PointerProgressToAttackMultiplier(progress)) - enemy.defendValue;
        if (!finished)
        {
            curTime += Time.deltaTime;
        }

        if (isAttacking)
        {
            progress = time / maxTime;
            pointerObject.position = Vector2.Lerp(leftPos, rightPos, progress * 1.2f);

            time += Time.deltaTime;
            if (time > 0.1f)
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

    // Activa la barra y el puntero del minijuego de FIGHT, llamado al pulsar el boton de ataque.
    public void StartAttacking(float playerDmg)
    {
        isAttacking = true;
        playerDamage = playerDmg;
        attackBg.enabled = true;
        pointerObject.gameObject.SetActive(true);
    }

    // Cambia el sprite del enemigo a la version "golpeado" y muestra el numero de dano (o MISS si fue 0).
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
        normal.SetActive(true);
        damaged.SetActive(false);
        damageTxt.text = "";
        damageTxt.color = damageColor;
    }

    // Resta vida al enemigo, espera 1 segundo y oculta la barra invirtiendo los extremos para el siguiente turno.
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
        leftPos.x = leftPos.x * -1;
        rightPos.x = rightPos.x * -1;
    }

    // Hace parpadear el sprite del puntero durante 0.75s alternando dos imagenes para el efecto visual.
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
