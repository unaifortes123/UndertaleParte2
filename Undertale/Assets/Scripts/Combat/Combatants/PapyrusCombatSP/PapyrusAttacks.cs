using System.Collections;
using UnityEngine;

// scriptable que mete los ataques de papyrus al menu de Unity (Create > Attacks > Papyrus)
[CreateAssetMenu(fileName = "PapyrusAttack", menuName = "Attacks/Papyrus")]
public class PapyrusAttacks : Attacks
{
    int bonePelletIndex = 0; // indice del prefab de hueso que va a usar (papyrus solo tiene uno)
    int attackTurn = 0; // turno actual, lo usa para ir rotando entre los 4 ataques

    // va alternando los 4 ataques en orden (no aleatorio), asi siempre toca cada uno
    public override IEnumerator GetAttack()
    {
        IEnumerator attack;

        // por defecto el primer ataque (turno 0)
        attack = PapyrusColumns();

        if (attackTurn == 1)
        {
            attack = PapyrusBoneRain();
        }

        if (attackTurn == 2)
        {
            attack = PapyrusBoneWave();
        }

        if (attackTurn == 3)
        {
            attack = PapyrusBoneStorm();
        }

        attackTurn++;

        // cuando pasa del ultimo vuelve al principio
        if (attackTurn > 3)
        {
            attackTurn = 0;
        }

        return attack;
    }

    // 6 oleadas de huesos cruzando desde la derecha, alternando arriba y abajo. Al acabar pone el alma azul.
    IEnumerator PapyrusColumns()
    {
        int wave;
        float rightSide;
        float topY;
        float bottomY;
        bool shouldTurnSoulBlue;

        wave = 0; // contador de oleadas (va a hacer 6)
        rightSide = 4.2f; // X donde aparece el hueso, fuera de la caja por la derecha
        topY = -1.2f; // altura del hueso "de arriba"
        bottomY = -2.45f; // altura del hueso "de abajo"
        // solo cambia a alma azul si todavia no estaba azul (no quiere hacerlo dos veces)
        shouldTurnSoulBlue = BattleManager.battleInstance != null && !BattleManager.battleInstance.IsSoulBlue();

        yield return Wait(0.5f);

        // estira la caja en horizontal para que entren los huesos cruzando
        if (BattleManager.battleInstance != null)
        {
            BattleManager.battleInstance.ChangeBattleBoxSize(new Vector2(7f, 2f));
        }

        yield return Wait(0.25f);

        while (wave < 6)
        {
            if (wave == 0)
            {
                SpawnPellet(new Vector2(rightSide, bottomY), PelletType.BoneLeft, bonePelletIndex);
            }

            if (wave == 1)
            {
                SpawnPellet(new Vector2(rightSide, bottomY), PelletType.BoneLeft, bonePelletIndex);
                SpawnPellet(new Vector2(rightSide + 1.1f, topY), PelletType.BoneTopLeft, bonePelletIndex);
            }

            if (wave == 2)
            {
                SpawnPellet(new Vector2(rightSide, topY), PelletType.BoneTopLeft, bonePelletIndex);
                SpawnPellet(new Vector2(rightSide + 1.2f, bottomY), PelletType.BoneLeft, bonePelletIndex);
            }

            if (wave == 3)
            {
                SpawnPellet(new Vector2(rightSide, bottomY), PelletType.BoneLeft, bonePelletIndex);
                SpawnPellet(new Vector2(rightSide + 0.7f, topY), PelletType.BoneTopLeft, bonePelletIndex);
            }

            if (wave == 4)
            {
                SpawnPellet(new Vector2(rightSide, topY), PelletType.BoneTopLeft, bonePelletIndex);
                SpawnPellet(new Vector2(rightSide + 0.9f, bottomY), PelletType.BoneLeft, bonePelletIndex);
            }

            if (wave == 5)
            {
                SpawnPellet(new Vector2(rightSide, bottomY), PelletType.BoneLeft, bonePelletIndex);
                SpawnPellet(new Vector2(rightSide + 0.8f, topY), PelletType.BoneTopLeft, bonePelletIndex);
            }

            wave++;
            yield return Wait(0.85f);
        }

        // si era la primera vez, prepara el cambio a alma azul (afecta a la fisica del corazon)
        if (shouldTurnSoulBlue)
        {
            yield return Wait(3f);

            if (BattleManager.battleInstance != null)
            {
                BattleManager.battleInstance.SetNextPostTurnText("*NYEH HEH HEH! Papyrus turned your soul blue!");
                BattleManager.battleInstance.PrepareSoulBlueForMenu();
            }
        }
        else
        {
            // si ya era azul no hace falta el mensajito, espera un poco y ya
            yield return Wait(1f);
        }
    }

    // ataque 2: caja mas pequeña y huesos que giran y entran desde los lados alternando izq/der
    IEnumerator PapyrusBoneRain()
    {
        float leftSide;
        float rightSide;

        leftSide = -3.4f; // X izquierdo, fuera de la caja
        rightSide = 3.4f; // X derecho, fuera de la caja

        // encoge la caja para que el ataque sea mas dificil de esquivar
        if (BattleManager.battleInstance != null)
        {
            BattleManager.battleInstance.ChangeBattleBoxSize(new Vector2(5.5f, 2f));
        }

        yield return Wait(0.25f);

        SpawnPellet(new Vector2(leftSide, -1.35f), PelletType.BoneSpinRight, bonePelletIndex);
        yield return Wait(0.45f);
        SpawnPellet(new Vector2(rightSide, -1.85f), PelletType.BoneSpinLeft, bonePelletIndex);
        yield return Wait(0.45f);
        SpawnPellet(new Vector2(leftSide, -2.15f), PelletType.BoneSpinRight, bonePelletIndex);
        yield return Wait(0.45f);
        SpawnPellet(new Vector2(rightSide, -1.55f), PelletType.BoneSpinLeft, bonePelletIndex);
        yield return Wait(0.45f);
        SpawnPellet(new Vector2(leftSide, -1.75f), PelletType.BoneSpinRight, bonePelletIndex);
        SpawnPellet(new Vector2(rightSide, -2.05f), PelletType.BoneSpinLeft, bonePelletIndex);
        yield return Wait(2.4f);
    }

    // ataque 4: caja estrecha y alta, lluvia de huesos cayendo desde arriba en posiciones random
    IEnumerator PapyrusBoneStorm()
    {
        // caja muy estrechita para que tenga que esquivar a lo loco
        if (BattleManager.battleInstance != null)
        {
            BattleManager.battleInstance.ChangeBattleBoxSize(new Vector2(2f, 3.5f));
        }

        yield return Wait(0.5f);

        // 5 huesos cayendo, cada uno en una X random dentro de la caja
        SpawnPellet(new Vector2(Random.Range(-0.7f, 0.7f), 0.5f), PelletType.BoneDown, bonePelletIndex);
        yield return Wait(0.4f);
        SpawnPellet(new Vector2(Random.Range(-0.7f, 0.7f), 0.5f), PelletType.BoneDown, bonePelletIndex);
        yield return Wait(0.4f);
        SpawnPellet(new Vector2(Random.Range(-0.7f, 0.7f), 0.5f), PelletType.BoneDown, bonePelletIndex);
        yield return Wait(0.4f);
        SpawnPellet(new Vector2(Random.Range(-0.7f, 0.7f), 0.5f), PelletType.BoneDown, bonePelletIndex);
        yield return Wait(0.4f);
        SpawnPellet(new Vector2(Random.Range(-0.7f, 0.7f), 0.5f), PelletType.BoneDown, bonePelletIndex);
        yield return Wait(2f); // espera al final para que los huesos terminen de caer antes de cerrar el turno
    }

    // ataque 3: 5 huesos saltando desde abajo en fila, de izquierda a derecha
    IEnumerator PapyrusBoneWave()
    {
        // los lanza uno a uno con un pequeño delay para que parezcan una "ola"
        SpawnPellet(new Vector2(-1.05f, -2.725f), PelletType.JumpDirect, bonePelletIndex);
        yield return Wait(0.3f);
        SpawnPellet(new Vector2(-0.52f, -2.725f), PelletType.JumpDirect, bonePelletIndex);
        yield return Wait(0.3f);
        SpawnPellet(new Vector2(0f, -2.725f), PelletType.JumpDirect, bonePelletIndex);
        yield return Wait(0.3f);
        SpawnPellet(new Vector2(0.52f, -2.725f), PelletType.JumpDirect, bonePelletIndex);
        yield return Wait(0.3f);
        SpawnPellet(new Vector2(1.05f, -2.725f), PelletType.JumpDirect, bonePelletIndex);
        yield return Wait(1.5f);
    }
}
