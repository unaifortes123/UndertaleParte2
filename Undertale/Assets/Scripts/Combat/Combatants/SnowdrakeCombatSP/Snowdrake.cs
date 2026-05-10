using System.Collections;
using UnityEngine;

public class Snowdrake : MonoBehaviour
{
    [Header("Stats")]
    public int hp = 20;
    public int maxHp = 20;
    public bool isSpareable = false; // Indica si el enemigo se puede perdonar.

    [Header("References")]
    public SnowAttack attack;
    // Texto de la burbuja de dialogo de Snowdrake.
    public TMPro.TextMeshProUGUI speechBubbleText;

    [Header("English Dialogues")]
    private string[] quotes = {
        "'Ice' to meet you!",
        "Better 'snow' your limits!",
        "What a 'n-ice' day!",
        "I'm 'chill' to the bone!"
    };

    // Esta funcion resta vida cuando el personaje recibe dano.
    public void TakeDamage(int dmg)
    {
        hp -= dmg;

        if (hp <= 0)
        {
            Die();
        }
    }

    // Esta funcion desactiva al enemigo cuando se queda sin vida.
    private void Die()
    {
        gameObject.SetActive(false);
    }

    // Esta funcion permite perdonar a Snowdrake despues de usar LAUGH.
    public void GetLaughedAt()
    {
        isSpareable = true;
    }

    // Esta funcion empieza el ataque de Snowdrake.
    public void Attack(BattleManager battle)
    {
        if (attack == null)
        {
            Debug.LogError("SnowAttack no asignado en el Inspector de Snowdrake");
        }
        else
        {
            StartCoroutine(AttackRoutine(battle));
        }
    }

    // Esta funcion muestra un texto corto y ejecuta el ataque.
    IEnumerator AttackRoutine(BattleManager battle)
    {
        if (speechBubbleText != null)
        {
            speechBubbleText.text = quotes[Random.Range(0, quotes.Length)];
            yield return new WaitForSeconds(2f);
            speechBubbleText.text = "";
        }


        attack.ShootSnow();

        yield return new WaitForSeconds(4f);
    }
}
