using System.Collections;
using UnityEngine;

public class BattleManager2 : MonoBehaviour
{
    public UIBattle ui;

    public GameObject battleBox;
    public GameObject heart;

    private enum BattleState
    {
        Intro,
        PlayerTurn,
        EnemyTurn,
        Mercy
    }

    private BattleState state;

    void Start()
    {
        StartBattle();
    }

    public void StartBattle()
    {
        state = BattleState.Intro;

        ui.ShowText("¡Un Snowdrake aparece!");
        ui.EnableButtons(false);

        battleBox.SetActive(false);
        heart.SetActive(false);

        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(2f);

        ui.ShowText("");
        StartPlayerTurn();
    }

    void StartPlayerTurn()
    {
        state = BattleState.PlayerTurn;

        ui.EnableButtons(true);

        battleBox.SetActive(false);
        heart.SetActive(false);
    }

    public void StartEnemyTurn()
    {
        state = BattleState.EnemyTurn;

        ui.EnableButtons(false);

        battleBox.SetActive(true);
        heart.SetActive(true);

        Debug.Log("Turno enemigo");

        // 👉 aquí luego llamas ataque Snowdrake
        StartCoroutine(EnemyAttackRoutine());
    }

    IEnumerator EnemyAttackRoutine()
    {
        yield return new WaitForSeconds(2f);

        Debug.Log("Snowdrake ataca ❄️");

        // cuando acabas el ataque
        EndEnemyTurn();
    }

    public void EndEnemyTurn()
    {
        StartPlayerTurn();
    }

    public void PlayerAction(string action)
    {
        if (state != BattleState.PlayerTurn)
            return;

        Debug.Log("Jugador eligió: " + action);

        if (action == "MERCY")
        {
            StartMercyBattle();
            return;
        }

        StartEnemyTurn();
    }

    void StartMercyBattle()
    {
        state = BattleState.Mercy;

        ui.EnableButtons(false);
        ui.ShowText("Has perdonado al enemigo...");

        battleBox.SetActive(true);
        heart.SetActive(true);

        Debug.Log("Modo MERCY activado");

        StartCoroutine(MercyEnd());
    }

    IEnumerator MercyEnd()
    {
        yield return new WaitForSeconds(2f);

        Debug.Log("Combate terminado (MERCY)");
        // aquí podrías salir de combate o volver a mapa
    }
}