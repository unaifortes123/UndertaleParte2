using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PapyrusCombat : EnemyVars
{
    // Aplica los valores por defecto si la escena no los trae y luego deja a la base poner curHP a maxHP.
    protected override void Awake()
    {
        ApplyDefaultValues();
        base.Awake();
    }

    // Se ejecuta al pulsar Reset en el componente desde Unity, util para autorrellenar campos vacios.
    void Reset()
    {
        ApplyDefaultValues();
    }

    // Papyrus nunca muere, se queda con 1 de vida, se rie y te deja ir.
    public void HandleDefeat()
    {
        BattleManager battle;

        curHP = 1;
        battle = BattleManager.battleInstance;

        if (battle != null)
        {
            battle.LockMenuInput();
            battle.StopAllCoroutines();

            if (AttackManager.instance != null)
            {
                AttackManager.instance.StopAllCoroutines();
            }

            battle.StartCoroutine(PapyrusLaugh());
        }
    }

    // Esta funcion espera un momento, muestra la risa de Papyrus y luego sale del combate.
    IEnumerator PapyrusLaugh()
    {
        BattleManager battle;

        yield return new WaitForSeconds(0.5f);

        battle = BattleManager.battleInstance;

        // Al pulsar FIGHT se desactiva el texto del combate, lo volvemos a encender para que se vea la risa.
        if (battle != null && battle.actingMgr != null && battle.actingMgr.actingText != null)
        {
            battle.actingMgr.actingText.gameObject.SetActive(true);
        }

        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.dialogueTxt = "*NYEH HEH HEH HEH HEH! You cannot defeat THE GREAT PAPYRUS!";
            DialogueManager.instance.enemyTxt = "";
            DialogueManager.instance.Talking(FinishPapyrusDefeat);
        }
        else
        {
            FinishPapyrusDefeat();
        }
    }

    // Esta funcion se llama cuando termina el dialogo de la risa y cierra el combate.
    void FinishPapyrusDefeat()
    {
        if (BattleManager.battleInstance != null)
        {
            BattleManager.battleInstance.EndBattle();
        }
    }

    // Rellena nombre, stats, dialogos y flavor text con los valores tipicos de Papyrus si estaban vacios.
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
