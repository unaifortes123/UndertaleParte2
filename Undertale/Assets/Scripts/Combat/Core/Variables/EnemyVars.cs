using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// clase base de todos los enemigos, guarda stats y se los pasa a los managers cuando empieza el combate
public class EnemyVars : MonoBehaviour
{
    [Header("Stats")]
    public float maxHP; // vida maxima del enemigo
    public float curHP; // vida actual, va bajando cuando le pega
    public float attackValue; // cuanto pega el enemigo
    public float defendValue; // cuanto resiste cuando le pega

    [Header("Combat Setup")]
    public string enemyName; // nombre que sale en el combate
    public Attacks attacksScriptable; // scriptable con los ataques que tiene
    public Pellet[] pelletPrefabs; // los huesos / balas que va a soltar
    public List<string> enemyDialogue = new List<string>(); // frases que dice entre turnos
    public string spareMessage; // mensaje que sale cuando ya lo puedes perdonar
    public List<string> flavorText = new List<string>(); // textos de "Check" / descripcion

    // si nos deja curHP a 0 en el inspector, lo rellena con maxHP para que no nazca muerto
    protected virtual void Awake()
    {
        if (maxHP > 0 && curHP <= 0)
        {
            curHP = maxHP;
        }
    }

    // pasa los datos del enemigo (ataques, balas, dialogos, perdon) a los managers cuando empieza el combate
    public virtual void ApplyCombatSetup(BattleManager battleManager, AttackManager attackManager, ActingManager actingManager)
    {
        // por si acaso, comprueba otra vez la vida (Awake a veces no se ha llamado todavia)
        if (maxHP > 0 && curHP <= 0)
        {
            curHP = maxHP;
        }

        // le pasa al AttackManager el scriptable con los ataques de este enemigo
        if (attackManager != null && attacksScriptable != null)
        {
            attackManager.attacksScriptable = attacksScriptable;
        }

        // y tambien los prefabs de las balas/huesos que usa este enemigo en concreto
        if (attackManager != null && pelletPrefabs != null && pelletPrefabs.Length > 0)
        {
            attackManager.pelletPrefab = pelletPrefabs;
        }

        // los dialogos que dice el enemigo entre turnos van al BattleManager
        if (battleManager != null && enemyDialogue != null && enemyDialogue.Count > 0)
        {
            battleManager.enemyDialogue = enemyDialogue;
        }

        // mensaje de spare y textos de check se los pasa al ActingManager (el del menu ACT)
        if (actingManager != null)
        {
            if (!string.IsNullOrWhiteSpace(spareMessage))
            {
                actingManager.spareMessage = spareMessage;
            }

            if (flavorText != null && flavorText.Count > 0)
            {
                actingManager.flavorText = flavorText;
            }
        }
    }
}
