using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVars : MonoBehaviour
{
    [Header("Stats")]
    public float maxHP;
    public float curHP;
    public float attackValue;   
    public float defendValue;

    [Header("Combat Setup")]
    public string enemyName;
    public Attacks attacksScriptable;
    public Pellet[] pelletPrefabs;
    public List<string> enemyDialogue = new List<string>();
    public string spareMessage;
    public List<string> flavorText = new List<string>();

    // Si curHP esta vacia al empezar la rellena con maxHP, asi no nace muerto si se olvido configurarlo.
    protected virtual void Awake()
    {
        if (maxHP > 0 && curHP <= 0)
        {
            curHP = maxHP;
        }
    }

    // Pasa los datos del enemigo (ataques, balas, dialogos) a los managers del combate al empezar.
    public virtual void ApplyCombatSetup(BattleManager battleManager, AttackManager attackManager, ActingManager actingManager)
    {
        if (maxHP > 0 && curHP <= 0)
        {
            curHP = maxHP;
        }

        if (attackManager != null && attacksScriptable != null)
        {
            attackManager.attacksScriptable = attacksScriptable;
        }

        if (attackManager != null && pelletPrefabs != null && pelletPrefabs.Length > 0)
        {
            attackManager.pelletPrefab = pelletPrefabs;
        }

        if (battleManager != null && enemyDialogue != null && enemyDialogue.Count > 0)
        {
            battleManager.enemyDialogue = enemyDialogue;
        }

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
