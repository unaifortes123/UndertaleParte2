using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MikuAttack", menuName = "Attacks/Miku")]
public class MikuAttacks : Attacks
{
    // Esta funcion devuelve el ataque de Miku que toca.
    public override IEnumerator GetAttack()
    {
        IEnumerator attack;
        int attackNumber;

        attackNumber = Random.Range(0, 3);
        attack = MikuDefault();

        if (attackNumber == 1)
        {
            attack = MikuWave();
        }

        if (attackNumber == 2)
        {
            attack = MikuStars();
        }

        return attack;
    }

    // Esta funcion lanza notas que caen y persiguen al player.
    IEnumerator MikuDefault()
    {
        SpawnPellet(new Vector2(Random.Range(-1.3f, 1.3f), 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(0.75f);
        SpawnPellet(new Vector2(Random.Range(-1.3f, 1.3f), 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(0.75f);
        SpawnPellet(new Vector2(Random.Range(-1.3f, 1.3f), 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(3f);
    }

    // Esta funcion hace llover microfonos desde los lados.
    IEnumerator MikuWave()
    {
        SpawnPellet(new Vector2(-1.55f, 0.55f), PelletType.SideRain, 2);
        yield return Wait(0.3f);
        SpawnPellet(new Vector2(1.55f, 0.45f), PelletType.SideRain, 2);
        yield return Wait(0.3f);
        SpawnPellet(new Vector2(-1.45f, 0.65f), PelletType.SideRain, 2);
        yield return Wait(0.3f);
        SpawnPellet(new Vector2(1.45f, 0.6f), PelletType.SideRain, 2);
        yield return Wait(0.3f);
        SpawnPellet(new Vector2(-1.6f, 0.5f), PelletType.SideRain, 2);
        SpawnPellet(new Vector2(1.6f, 0.5f), PelletType.SideRain, 2);
        yield return Wait(2.4f);
    }

    // Esta funcion tira tres notas desde el centro de la caja.
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
