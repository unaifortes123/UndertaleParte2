using System.Collections.Generic;
using UnityEngine;

public class LesserDogCombat : EnemyVars
{
    protected override void Awake()
    {
        ApplyDefaultValues();
        base.Awake();
    }

    void Reset()
    {
        ApplyDefaultValues();
    }

    void ApplyDefaultValues()
    {
        // Nombre
        if (string.IsNullOrWhiteSpace(enemyName))
        {
            enemyName = "Lesser Dog";
        }

        // Vida (poca pero “molesta”)
        if (maxHP <= 0)
        {
            maxHP = 20;
        }

        // Ataque
        if (attackValue <= 0)
        {
            attackValue = 3;
        }

        // Defensa baja
        if (defendValue <= 0)
        {
            defendValue = 1;
        }

        // Diálogo estilo Undertale
        if (enemyDialogue == null || enemyDialogue.Count == 0)
        {
            enemyDialogue = new List<string>
            {
                "* Lesser Dog barks excitedly.",
                "* It stares at you with intense enthusiasm.",
                "* Lesser Dog is ready to play.",
                "* It doesn't understand what's happening."
            };
        }

        // Mensaje de perdón
        if (string.IsNullOrWhiteSpace(spareMessage))
        {
            spareMessage = "* Lesser Dog wags its entire body uncontrollably.";
        }

        // Flavor text (caos adorable)
        if (flavorText == null || flavorText.Count == 0)
        {
            flavorText = new List<string>
            {
                "* The dog is spinning slightly.",
                "* Bark. Bark.",
                "* It looks like it wants attention.",
                "* The dog is vibrating with energy."
            };
        }
    }
}