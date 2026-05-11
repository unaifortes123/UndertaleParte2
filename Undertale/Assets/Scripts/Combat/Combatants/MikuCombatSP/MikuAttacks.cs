using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MikuAttack", menuName = "Attacks/Miku")]
public class MikuAttacks : Attacks
{
    // ataque aleatorio de los 3 que tiene Miku
    public override IEnumerator GetAttack()
    {
        IEnumerator attack;
        int attackNumber;

        attackNumber = Random.Range(0, 3);
        attack = MikuDefault(); // default por si toca 0

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

    // 5 puerros cayendo en patron fijo (izq, der, centro, y luego los dos diagonales). Pelletprefab 0 = puerro.
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

    // microfonos entrando desde los lados a alturas variables, los dos ultimos vienen a la vez (pellet 1 = micro)
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

    // encoge la caja como un foco de escenario y mezcla puerros cayendo + micros desde los lados
    IEnumerator MikuStars()
    {
        // caja estrecha y alta, tipo foco de teatro
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
