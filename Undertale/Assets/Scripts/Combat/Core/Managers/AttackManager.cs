using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// se encarga de los ataques del enemigo: ejecuta la corrutina, spawnea balas y las mueve cada frame
public class AttackManager : MonoBehaviour
{
    public static AttackManager instance;
    // las balas las mete el enemigo desde EnemyVars.pelletPrefabs cuando empieza el combate
    [HideInInspector] public Pellet[] pelletPrefab;
    public Attacks attacksScriptable; // scriptable con los ataques del enemigo de turno
    public bool attackFinished;

    List<IFightObject> attackObject = new List<IFightObject>(); // todas las balas vivas ahora mismo

    void Awake()
    {
        instance = this;
    }

    // pide al scriptable del enemigo el siguiente ataque, si no hay devuelve uno vacio para no petar
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

    // arranca la corrutina del ataque y guarda el callback para cuando acabe
    public void StartAttack(IEnumerator attack, Action onFinish)
    {
        attackFinished = false;
        StartCoroutine(StartAttackEnumerator(attack, onFinish));
    }

    // cada frame mueve todas las balas vivas llamando a su Tick
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

    // instancia una bala del prefab indicado en la posicion dada y la registra para mover cada frame
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
            // si el indice se pasa o es negativo usa el primero para que no crashee el combate
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
                attackObject.Add(pelletAsObj); // la mete en la lista para que la mueva el Update
            }
        }
    }

    // corrutina vacia que se devuelve cuando un enemigo no tiene ataques configurados, asi evita nullref
    IEnumerator EmptyAttack()
    {
        yield return null;
    }

    // ejecuta paso a paso la corrutina del ataque, al final llama al callback y borra todas las balas
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

        // limpia todas las balas que quedaran vivas al acabar el ataque
        for (i = 0; i < attackObject.Count; i++)
        {
            attackObject[i].Remove();
        }

        attackObject.Clear();
    }
}
