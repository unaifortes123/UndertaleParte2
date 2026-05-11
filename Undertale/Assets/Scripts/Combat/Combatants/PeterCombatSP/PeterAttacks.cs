using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PeterAttack", menuName = "Attacks/Peter")]
public class PeterAttacks : Attacks
{

    // ataque aleatorio de los 3 que tiene Peter
    public override IEnumerator GetAttack()
    {
        IEnumerator attack;
        int attackNumber;

        attackNumber = Random.Range(0, 3);
        attack = PeterVomit();

        if (attackNumber == 1)
        {
            attack = PeterJump();
        }

        if (attackNumber == 2)
        {
            attack = PeterPressure();
        }

        return attack;
    }

    // 3 vomitos cayendo en X aleatoria que persiguen al player (pellet 0 = vomito)
    IEnumerator PeterVomit()
    {
        SpawnPellet(new Vector2(Random.Range(-1.3f, 1.3f), 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(0.75f);
        SpawnPellet(new Vector2(Random.Range(-1.3f, 1.3f), 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(0.75f);
        SpawnPellet(new Vector2(Random.Range(-1.3f, 1.3f), 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(3f);
    }

    // 2 balas saltando desde abajo, una en cada esquina (pellet 1 = bala saltarina)
    IEnumerator PeterJump()
    {
        SpawnPellet(new Vector2(-1.05f, -2.725f), PelletType.JumpDirect, 1);
        yield return Wait(0.8f);
        SpawnPellet(new Vector2(1.05f, -2.725f), PelletType.JumpDirect, 1);
        yield return Wait(2f);
    }

    // 3 vomitos cayendo al mismo nivel, izq-centro-der, obligan al player a moverse en horizontal
    IEnumerator PeterPressure()
    {
        SpawnPellet(new Vector2(-1.3f, 0), PelletType.FallFollowDirect, 0);
        yield return Wait(0.5f);
        SpawnPellet(new Vector2(0, 0), PelletType.FallFollowDirect, 0);
        yield return Wait(0.5f);
        SpawnPellet(new Vector2(1.3f, 0), PelletType.FallFollowDirect, 0);
        yield return Wait(3f);
    }
}
