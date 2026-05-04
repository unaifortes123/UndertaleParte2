using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public static AttackManager instance;
    public Pellet[] pelletPrefab;
    public Attacks attacksScriptable;
    public bool attackFinished;

    List<IFightObject> attackObject = new List<IFightObject>();

    void Awake()
    {
        instance = this;
    }

    public IEnumerator GetAttack()
    {
        IEnumerator attack;

        attack = EmptyAttack();

        if (attacksScriptable == null)
        {
            Debug.LogError("No attack scriptable assigned.");
        }
        else
        {
            attack = attacksScriptable.GetAttack();
        }

        return attack;
    }

    public void StartAttack(IEnumerator attack, Action onFinish)
    {
        attackFinished = false;
        StartCoroutine(StartAttackEnumerator(attack, onFinish));
    }

    void Update()
    {
        int i;
        IFightObject curObject;

        for (i = 0; i < attackObject.Count; i++)
        {
            curObject = attackObject[i];

            if (curObject == null)
            {
                Debug.LogWarning($"THE ATTACK OBJECT AT {i} IS NULL!!!!");
            }
            else
            {
                curObject.Tick();
            }
        }

    }

    public void SpawnPellet(Vector2 position, PelletType type, int pelletType)
    {
        Pellet newPellet;
        IFightObject pelletAsObj;

        newPellet = Instantiate(pelletPrefab[pelletType], position, Quaternion.identity).GetComponent<Pellet>();
        newPellet.type = type;

        pelletAsObj = (IFightObject)newPellet;
        pelletAsObj.Spawn();
        attackObject.Add(pelletAsObj);
    }

    IEnumerator EmptyAttack()
    {
        yield return null;
    }

    IEnumerator StartAttackEnumerator(IEnumerator attack, Action onFinish)
    {
        int i;

        while (attack.MoveNext())
        {
            yield return attack.Current;
        }

        onFinish?.Invoke();
        for (i = 0; i < attackObject.Count; i++)
        {
            attackObject[i].Remove();
        }

        attackObject.Clear();
    }
}
