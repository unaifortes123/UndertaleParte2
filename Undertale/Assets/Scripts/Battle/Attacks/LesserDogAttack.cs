using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "LesserDogAttack", menuName = "Attacks/LesserDog")]
public class LesserDogAttack : Attacks
{
    public override IEnumerator GetAttack()
    {
        int attackNumber = Random.Range(0, 2);

        IEnumerator attack = DogAttackOne();

        if (attackNumber == 1)
        {
            attack = DogAttackTwo();
        }

        return attack;
    }

    IEnumerator DogAttackOne()
    {
        for (int i = 0; i < 5; i++)
        {
            SpawnPellet(
                new Vector2(Random.Range(-1.5f, 1.5f), 3f),
                PelletType.FallFollowDirect,
                0
            );

            yield return Wait(0.4f);
        }

        yield return Wait(1f);
    }

    IEnumerator DogAttackTwo()
    {
        Vector2 pos = new Vector2(Random.Range(-1.5f, 1.5f), 3f);

        for (int i = 0; i < 8; i++)
        {
            SpawnPellet(pos, PelletType.JumpDirect, 1);

            pos.x += Random.Range(-0.5f, 0.5f);

            yield return Wait(0.3f);
        }

        yield return Wait(1f);
    }
}