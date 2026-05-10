using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MikuAttack", menuName = "Attacks/Miku")]
public class MikuAttacks : Attacks
{
    // Devuelve uno de los 3 ataques de Miku al azar (default, micros laterales, foco con micros y puerros).
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

    // Lluvia de puerros mas densa que se alternan y persiguen al player, no le dejan parar quieto.
    IEnumerator MikuDefault()
    {
        SpawnPellet(new Vector2(-1.4f, 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(0.3f);
        SpawnPellet(new Vector2(1.4f, 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(0.3f);
        SpawnPellet(new Vector2(0f, 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(0.3f);
        SpawnPellet(new Vector2(-0.7f, 0.25f), PelletType.FallFollowDirect, 0);
        SpawnPellet(new Vector2(0.7f, 0.25f), PelletType.FallFollowDirect, 0);
        yield return Wait(2.8f);
    }

    // Llueven microfonos desde los lados a alturas variables, los ultimos dos vienen a la vez de ambos lados.
    IEnumerator MikuWave()
    {
        SpawnPellet(new Vector2(-1.55f, 0.55f), PelletType.SideRain, 1);
        yield return Wait(0.3f);
        SpawnPellet(new Vector2(1.55f, 0.45f), PelletType.SideRain, 1);
        yield return Wait(0.3f);
        SpawnPellet(new Vector2(-1.45f, 0.65f), PelletType.SideRain, 1);
        yield return Wait(0.3f);
        SpawnPellet(new Vector2(1.45f, 0.6f), PelletType.SideRain, 1);
        yield return Wait(0.3f);
        SpawnPellet(new Vector2(-1.6f, 0.5f), PelletType.SideRain, 1);
        SpawnPellet(new Vector2(1.6f, 0.5f), PelletType.SideRain, 1);
        yield return Wait(2.4f);
    }

    // Encoge la caja como un foco de escenario y lanza puerros desde arriba y micros desde los lados a la vez.
    IEnumerator MikuStars()
    {
        if (BattleManager.battleInstance != null)
        {
            BattleManager.battleInstance.ChangeBattleBoxSize(new Vector2(2.5f, 3f));
        }

        yield return Wait(0.5f);

        SpawnPellet(new Vector2(-0.8f, 0.5f), PelletType.FallFollowDirect, 0);
        SpawnPellet(new Vector2(-1.55f, 0.55f), PelletType.SideRain, 1);
        yield return Wait(0.4f);
        SpawnPellet(new Vector2(0.8f, 0.5f), PelletType.FallFollowDirect, 0);
        SpawnPellet(new Vector2(1.55f, 0.45f), PelletType.SideRain, 1);
        yield return Wait(0.4f);
        SpawnPellet(new Vector2(0f, 0.5f), PelletType.FallFollowDirect, 0);
        yield return Wait(0.4f);
        SpawnPellet(new Vector2(-0.4f, 0.5f), PelletType.FallFollowDirect, 0);
        SpawnPellet(new Vector2(0.4f, 0.5f), PelletType.FallFollowDirect, 0);
        yield return Wait(2.5f);
    }
}
