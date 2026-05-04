using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MikuAttack", menuName = "Attacks/Miku")]
public class MikuAttacks : Attacks
{
    // Debuelve el ataque que toca.
    public override IEnumerator GetAttack()
    {
        IEnumerator attack;
        int attackNumber;

        attackNumber = Random.Range(0, 3);
        attack = MikuDefault();

        if (attackNumber == 1)
        {
            attack = MikuRabano();
        }

        if (attackNumber == 2)
        {
            attack = MikuStars();
        }

        return attack;
    }

    // Ajusta la animacion del PeterVomitando.
    IEnumerator MikuDefault()
    {
        SpawnPellet(new Vector2(Random.Range(-1.3f, 1.3f), 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(0.75f);
        SpawnPellet(new Vector2(Random.Range(-1.3f, 1.3f), 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(0.75f);
        SpawnPellet(new Vector2(Random.Range(-1.3f, 1.3f), 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(3f);
    }

    // Ajusta la animacion del Peter.
    IEnumerator MikuRabano()
    {
        SpawnPellet(new Vector2(-1.05f, -2.725f), PelletType.JumpDirect, 1);
        yield return Wait(0.8f);
        SpawnPellet(new Vector2(1.05f, -2.725f), PelletType.JumpDirect, 1);
        yield return Wait(2f);
    }

    IEnumerator MikuStars()
    {
        SpawnPellet(new Vector2(-1.3f, 0), PelletType.FallFollowDirect, 0);
        yield return Wait(0.5f);
        SpawnPellet(new Vector2(0, 0), PelletType.FallFollowDirect, 0);
        yield return Wait(0.5f);
        SpawnPellet(new Vector2(1.3f, 0), PelletType.FallFollowDirect, 0);
        yield return Wait(3f);
    }
}
