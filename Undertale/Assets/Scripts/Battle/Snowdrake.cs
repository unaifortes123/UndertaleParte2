using System.Collections;
using UnityEngine;

public class Snowdrake : MonoBehaviour
{
    [Header("Stats")]
    public int hp = 20;
    public int maxHp = 20;
    public bool isSpareable = false; // Para el botón MERCY

    [Header("References")]
    public SnowAttack attack;
    // Referencia al texto de la burbuja de diálogo de Snowdrake
    public TMPro.TextMeshProUGUI speechBubbleText;

    [Header("English Dialogues")]
    private string[] quotes = {
        "'Ice' to meet you!",
        "Better 'snow' your limits!",
        "What a 'n-ice' day!",
        "I'm 'chill' to the bone!"
    };

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        Debug.Log("Snowdrake HP: " + hp);

        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Snowdrake turned into dust... ❄️");
        gameObject.SetActive(false); // O destruirlo
    }

    // Lógica para cuando el jugador usa "LAUGH"
    public void GetLaughedAt()
    {
        isSpareable = true;
        Debug.Log("Snowdrake is happy now!");
    }

    public void Attack(BattleManager2 battle)
    {
        if (attack == null)
        {
            Debug.LogError("SnowAttack no asignado en el Inspector de Snowdrake");
            return;
        }
        StartCoroutine(AttackRoutine(battle));
    }

    IEnumerator AttackRoutine(BattleManager2 battle)
    {
        // 1. Mostrar diálogo aleatorio antes de atacar
        if (speechBubbleText != null)
        {
            speechBubbleText.text = quotes[Random.Range(0, quotes.Length)];
            yield return new WaitForSeconds(2f); // Tiempo para leer
            speechBubbleText.text = "";
        }

        Debug.Log("Snowdrake is attacking! ❄️");

        // 2. Iniciar el ataque (asegúrate de que ShootSnow no bloquee el hilo)
        attack.ShootSnow();

        // 3. Duración del turno defensivo (esquivar)
        yield return new WaitForSeconds(4f);

        // 4. Limpiar balas si es necesario y terminar turno
        // attack.ClearBullets(); 
        battle.EndEnemyTurn();
    }
}
