using System.Collections.Generic;
using UnityEngine;

public class PapyrusCombat : EnemyVars
{
    // Esta funcion prepara los valores del enemigo antes de empezar.
    protected override void Awake()
    {
        ApplyDefaultValues();
        base.Awake();
    }

    // Esta funcion rellena los valores cuando reinicias el componente en Unity.
    void Reset()
    {
        ApplyDefaultValues();
    }

    // Esta funcion pone valores por defecto si faltan en la escena.
    void ApplyDefaultValues()
    {
        if (string.IsNullOrWhiteSpace(enemyName))
        {
            enemyName = "Papyrus";
        }

        if (maxHP <= 0)
        {
            maxHP = 30;
        }

        if (attackValue <= 0)
        {
            attackValue = 5;
        }

        if (defendValue <= 0)
        {
            defendValue = 2;
        }

        if (enemyDialogue == null || enemyDialogue.Count == 0)
        {
            enemyDialogue = new List<string>
            {
                "*Papyrus strikes a dramatic pose.",
                "*Papyrus raises his chin proudly.",
                "*Papyrus is absolutely brimming with confidence.",
                "*Papyrus prepares a truly special attack."
            };
        }

        if (string.IsNullOrWhiteSpace(spareMessage))
        {
            spareMessage = "*Papyrus is willing to accept your mercy.";
        }

        if (flavorText == null || flavorText.Count == 0)
        {
            flavorText = new List<string>
            {
                "*Papyrus polishes his battle body.",
                "*Papyrus adjusts his scarf dramatically.",
                "*Papyrus watches you with sparkling confidence."
            };
        }
    }
}
