using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attacks")]
public class Attacks : ScriptableObject
{
    public virtual IEnumerator GetAttack()
    {
        IEnumerator attack;
        int attackNumber;

        attackNumber = Random.Range(0, 2);
        attack = FroggitOne();

        if (attackNumber == 1)
        {
            attack = FroggitTwo();
        }

        return attack;
    }

    protected void SpawnPellet(Vector2 position, PelletType type, int pelletType)
    {
        AttackManager.instance.SpawnPellet(position, type, pelletType);
    }

    protected WaitForSeconds Wait(float seconds)
    {
        return new WaitForSeconds(seconds);
    }

    IEnumerator FroggitOne()
    {
        SpawnPellet(new Vector2(Random.Range(-1.4f, 1.4f), 0), PelletType.FallFollowDirect, 0);
        yield return Wait(1);
        SpawnPellet(new Vector2(Random.Range(-1.4f, 1.4f), 0), PelletType.FallFollowDirect, 0);
        yield return Wait(1);
        SpawnPellet(new Vector2(Random.Range(-1.4f, 1.4f), 0), PelletType.FallFollowDirect, 0);
        yield return Wait(5);
    }

    IEnumerator FroggitTwo()
    {
        Vector2 pos = new Vector2(1.013f, -2.725f);

        Debug.Log("this is froggit's 2nd attack");
        SpawnPellet(pos, PelletType.JumpDirect, 1);
        yield return Wait(2);
    }
}
