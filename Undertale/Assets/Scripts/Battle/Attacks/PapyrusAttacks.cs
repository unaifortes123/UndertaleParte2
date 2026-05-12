using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PapyrusAttack", menuName = "Attacks/Papyrus")]
public class PapyrusAttacks : Attacks
{
    const int BonePelletIndex = 2;
    const int timeAttack = 6;

    // Esta funcion devuelve el ataque de Papyrus que toca.
    public override IEnumerator GetAttack()
    {
        IEnumerator attack;
        int attackNumber;

        attackNumber = Random.Range(0, 3);
        attack = PapyrusBoneRain();

        if (attackNumber == 1)
        {
            attack = PapyrusColumns();
        }

        if (attackNumber == 2)
        {
            attack = PapyrusBoneJump();
        }

        return attack;
    }

    // Esta funcion deja caer huesos desde arriba para luego perseguir al player.
    IEnumerator PapyrusBoneRain()
    {
        SpawnPellet(new Vector2(Random.Range(-1.25f, 1.25f), 0.35f), PelletType.FallFollowDirect, BonePelletIndex);
        yield return Wait(0.45f);
        SpawnPellet(new Vector2(Random.Range(-1.25f, 1.25f), 0.35f), PelletType.FallFollowDirect, BonePelletIndex);
        yield return Wait(0.45f);
        SpawnPellet(new Vector2(Random.Range(-1.25f, 1.25f), 0.35f), PelletType.FallFollowDirect, BonePelletIndex);
        yield return Wait(2.6f);
    }

    // Esta funcion hace el primer ataque de Papyrus, que aparecen columnas en linea recta.
    IEnumerator PapyrusColumns()
    {
        float timeCounter = Time.time;

        while (timeCounter < timeAttack) {

            SpawnPellet(new Vector2(0f, 0.2f), PelletType.SideRain, BonePelletIndex);
            yield return Wait(0.25f);

        }

    }

    // Esta funcion hace saltar dos huesos desde abajo.
    IEnumerator PapyrusBoneJump()
    {
        SpawnPellet(new Vector2(-1.05f, -2.725f), PelletType.JumpDirect, BonePelletIndex);
        yield return Wait(0.65f);
        SpawnPellet(new Vector2(1.05f, -2.725f), PelletType.JumpDirect, BonePelletIndex);
        yield return Wait(2.2f);
    }
}
