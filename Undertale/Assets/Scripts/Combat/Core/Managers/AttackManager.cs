using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public static AttackManager instance;
    // Las balas se las pasa el enemigo al empezar el combate desde EnemyVars.pelletPrefabs.
    [HideInInspector] public Pellet[] pelletPrefab;
    public Attacks attacksScriptable;
    public bool attackFinished;

    List<IFightObject> attackObject = new List<IFightObject>();

    // Guarda la referencia singleton para que cualquier script pueda llamar al manager.
    void Awake()
    {
        instance = this;
    }

    // Pide al ScriptableObject del enemigo el ataque del turno actual, o uno vacio si no hay nada asignado.
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

    // Lanza la corrutina del ataque del enemigo y guarda el callback de cuando termine.
    public void StartAttack(IEnumerator attack, Action onFinish)
    {
        attackFinished = false;
        StartCoroutine(StartAttackEnumerator(attack, onFinish));
    }

    // Cada frame mueve todas las balas activas llamando a su Tick.
    void Update()
    {
        int i;
        IFightObject curObject;

        for (i = 0; i < attackObject.Count; i++)
        {
            curObject = attackObject[i];

            if (curObject == null)
            {
                Debug.LogWarning("Attack object at index " + i + " is null.");
            }
            else
            {
                curObject.Tick();
            }
        }

    }

    // Instancia un prefab de bala en la posicion indicada y lo registra para que se mueva cada frame.
    public void SpawnPellet(Vector2 position, PelletType type, int pelletType)
    {
        Pellet newPellet;
        IFightObject pelletAsObj;
        int safePelletType;

        safePelletType = pelletType;

        if (pelletPrefab == null || pelletPrefab.Length == 0)
        {
            Debug.LogError("No hay prefabs de balas en AttackManager.");
        }
        else
        {
            if (safePelletType < 0 || safePelletType >= pelletPrefab.Length)
            {
                Debug.LogWarning("No existe pelletPrefab " + pelletType + ". Uso el primero para no romper el combate.");
                safePelletType = 0;
            }

            newPellet = Instantiate(pelletPrefab[safePelletType], position, Quaternion.identity).GetComponent<Pellet>();

            if (newPellet == null)
            {
                Debug.LogError("El prefab de bala no tiene el script Pellet.");
            }
            else
            {
                newPellet.type = type;
                pelletAsObj = (IFightObject)newPellet;
                pelletAsObj.Spawn();
                attackObject.Add(pelletAsObj);
            }
        }
    }

    // Corrutina vacia que se devuelve cuando un enemigo no tiene ataques configurados, evita nullref.
    IEnumerator EmptyAttack()
    {
        yield return null;
    }

    // Va ejecutando paso a paso la corrutina del ataque, al terminar llama al callback y borra todas las balas.
    IEnumerator StartAttackEnumerator(IEnumerator attack, Action onFinish)
    {
        int i;

        while (attack.MoveNext())
        {
            yield return attack.Current;
        }

        if (onFinish != null)
        {
            onFinish();
        }

        for (i = 0; i < attackObject.Count; i++)
        {
            attackObject[i].Remove();
        }

        attackObject.Clear();
    }
}
