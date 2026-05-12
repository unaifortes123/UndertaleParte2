using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PeterAttack", menuName = "Attacks/Peter")]
public class PeterAttacks : Attacks
{

    // Debuelve el ataque que toca.
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

    // Ajusta la animacion del PeterVomitando.
    IEnumerator PeterVomit()
    {
        SpawnPellet(new Vector2(Random.Range(-1.3f, 1.3f), 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(0.75f);
        SpawnPellet(new Vector2(Random.Range(-1.3f, 1.3f), 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(0.75f);
        SpawnPellet(new Vector2(Random.Range(-1.3f, 1.3f), 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(3f);
    }

    // Ajusta la animacion del Peter.
    IEnumerator PeterJump()
    {
        SpawnPellet(new Vector2(-1.05f, -2.725f), PelletType.JumpDirect, 1);
        yield return Wait(0.8f);
        SpawnPellet(new Vector2(1.05f, -2.725f), PelletType.JumpDirect, 1);
        yield return Wait(2f);
    }

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
