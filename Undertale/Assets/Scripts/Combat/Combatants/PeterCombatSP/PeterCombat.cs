using System.Collections.Generic;
using UnityEngine;

public class PeterCombat : EnemyVars
{
    // Aplica los valores por defecto antes de que la base ponga curHP a maxHP.
    protected override void Awake()
    {
        ApplyDefaultValues();
        base.Awake();
    }

    // Se llama cuando le das al Reset del componente en el inspector, autorrellena campos vacios.
    void Reset()
    {
        ApplyDefaultValues();
    }

    // Si en la escena faltan stats o dialogos, los rellena con los valores tipicos de Peter.
    void ApplyDefaultValues()
    {
        if (string.IsNullOrWhiteSpace(enemyName))
        {
            enemyName = "Peter";
        }

        if (maxHP <= 0)
        {
            maxHP = 30;
        }

        if (attackValue <= 0)
        {
            attackValue = 4;
        }

        if (defendValue <= 0)
        {
            defendValue = 2;
        }

        if (enemyDialogue == null || enemyDialogue.Count == 0)
        {
            enemyDialogue = new List<string>
            {
                "*Peter coughs loudly.",
                "*Peter looks like he regrets being here.",
                "*Peter is trying to keep standing.",
                "*Peter prepares another attack."
            };
        }

        if (string.IsNullOrWhiteSpace(spareMessage))
        {
            spareMessage = "*Peter seems reluctant to fight you.";
        }

        if (flavorText == null || flavorText.Count == 0)
        {
            flavorText = new List<string>
            {
                "*Peter catches his breath.",
                "*Peter wipes his mouth.",
                "*Peter stares at you awkwardly."
            };
        }
    }
}
