using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PapyrusAttack", menuName = "Attacks/Papyrus")]
public class PapyrusAttacks : Attacks
{
    int bonePelletIndex = 0;
    int attackTurn = 0;

    // Va rotando los 4 ataques de Papyrus en orden (no aleatorio) usando attackTurn.
    public override IEnumerator GetAttack()
    {
        IEnumerator attack;

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

        if (attackTurn > 3)
        {
            attackTurn = 0;
        }

        return attack;
    }

    // Hace 6 oleadas de huesos cruzando desde la derecha en patron arriba/abajo, y al final convierte el alma en azul.
    IEnumerator PapyrusColumns()
    {
        int wave;
        float rightSide;
        float topY;
        float bottomY;
        bool shouldTurnSoulBlue;

        wave = 0;
        rightSide = 4.2f;
        topY = -1.2f;
        bottomY = -2.45f;
        shouldTurnSoulBlue = BattleManager.battleInstance != null && !BattleManager.battleInstance.IsSoulBlue();

        yield return Wait(0.5f);

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
            yield return Wait(1f);
        }
    }

    // Encoge la caja a 5.5x2 y suelta huesos giratorios alternando lado izquierdo y derecho.
    IEnumerator PapyrusBoneRain()
    {
        float leftSide;
        float rightSide;

        leftSide = -3.4f;
        rightSide = 3.4f;

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

    // Esta funcion encoge la caja y deja caer huesos desde arriba mientras el jugador tiene poco espacio.
    IEnumerator PapyrusBoneStorm()
    {
        if (BattleManager.battleInstance != null)
        {
            BattleManager.battleInstance.ChangeBattleBoxSize(new Vector2(2f, 3.5f));
        }

        yield return Wait(0.5f);

        SpawnPellet(new Vector2(Random.Range(-0.7f, 0.7f), 0.5f), PelletType.BoneDown, bonePelletIndex);
        yield return Wait(0.4f);
        SpawnPellet(new Vector2(Random.Range(-0.7f, 0.7f), 0.5f), PelletType.BoneDown, bonePelletIndex);
        yield return Wait(0.4f);
        SpawnPellet(new Vector2(Random.Range(-0.7f, 0.7f), 0.5f), PelletType.BoneDown, bonePelletIndex);
        yield return Wait(0.4f);
        SpawnPellet(new Vector2(Random.Range(-0.7f, 0.7f), 0.5f), PelletType.BoneDown, bonePelletIndex);
        yield return Wait(0.4f);
        SpawnPellet(new Vector2(Random.Range(-0.7f, 0.7f), 0.5f), PelletType.BoneDown, bonePelletIndex);
        yield return Wait(2f);
    }

    // Esta funcion lanza cinco huesos desde abajo en fila, de izquierda a derecha.
    IEnumerator PapyrusBoneWave()
    {
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
