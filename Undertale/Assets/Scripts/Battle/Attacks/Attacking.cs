using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Attacking : MonoBehaviour
{
    public static Attacking instance;
    // Esta funcion guarda este ataque para que otros scripts puedan usarlo.
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

    // Esta funcion busca los datos del enemigo.
    void Start()
    {
        enemy = FindObjectOfType<EnemyVars>();
    }

    // Esta funcion calcula cuanto dano hace segun donde paras la barra.
    float PointerProgressToAttackMultiplier(float progress)
    {
        return Mathf.Min(progress * (playerDamage * 2), (1 - progress) * (playerDamage * 2));
    }

    // Esta funcion mueve la barra de ataque y detecta cuando pulsas Enter.
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

    // Esta funcion empieza el minijuego de atacar.
    public void StartAttacking(float playerDmg)
    {
        isAttacking = true;
        playerDamage = playerDmg;
        attackBg.enabled = true;
        pointerObject.gameObject.SetActive(true);
    }

    // Esta funcion ensena el dano o MISS encima del enemigo.
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

    // Esta funcion aplica el dano y cierra la barra de ataque.
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

    // Esta funcion hace parpadear el puntero despues de atacar.
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
