using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PeterAttack", menuName = "Attacks/Peter")]
public class PeterAttacks : Attacks
{

    // Devuelve uno de los 3 ataques de Peter al azar (vomito, salto, presion desde arriba).
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

    // Tira 3 vomitos en X aleatoria que caen y persiguen al player, con pausa entre cada uno.
    IEnumerator PeterVomit()
    {
        SpawnPellet(new Vector2(Random.Range(-1.3f, 1.3f), 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(0.75f);
        SpawnPellet(new Vector2(Random.Range(-1.3f, 1.3f), 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(0.75f);
        SpawnPellet(new Vector2(Random.Range(-1.3f, 1.3f), 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(3f);
    }

    // Dos balas saltan desde abajo, una en cada esquina, intentando alcanzar al player.
    IEnumerator PeterJump()
    {
        SpawnPellet(new Vector2(-1.05f, -2.725f), PelletType.JumpDirect, 1);
        yield return Wait(0.8f);
        SpawnPellet(new Vector2(1.05f, -2.725f), PelletType.JumpDirect, 1);
        yield return Wait(2f);
    }

    // Tres balas caen al mismo nivel desde izquierda, centro y derecha, presionando al player a moverse.
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
