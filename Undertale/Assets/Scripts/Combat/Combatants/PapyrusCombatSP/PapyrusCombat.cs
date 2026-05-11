using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PapyrusCombat : EnemyVars
{
    // mete los valores por defecto antes de que la clase base ponga curHP = maxHP
    protected override void Awake()
    {
        ApplyDefaultValues();
        base.Awake();
    }

    // se llama cuando le das Reset al componente en Unity, autorrellena los campos
    void Reset()
    {
        ApplyDefaultValues();
    }

    // papyrus nunca muere, se queda con 1 de vida, se rie y te deja ir
    public void HandleDefeat()
    {
        BattleManager battle;

        curHP = 1; // lo deja vivo de milagro
        battle = BattleManager.battleInstance;

        if (battle != null)
        {
            // bloquea el menu para que no pueda volver a pegarle mientras se rie
            battle.LockMenuInput();
            battle.StopAllCoroutines();

            // tambien corta los ataques que pudieran estar a medias
            if (AttackManager.instance != null)
            {
                AttackManager.instance.StopAllCoroutines();
            }

            battle.StartCoroutine(PapyrusLaugh());
        }
    }

    // espera un momento, muestra el "NYEH HEH HEH" y al acabar el dialogo cierra el combate
    IEnumerator PapyrusLaugh()
    {
        BattleManager battle;

        yield return new WaitForSeconds(0.5f);

        battle = BattleManager.battleInstance;

        // al pulsar FIGHT se desactiva el texto del combate, lo vuelve a encender para que se vea la risa
        if (battle != null && battle.actingMgr != null && battle.actingMgr.actingText != null)
        {
            battle.actingMgr.actingText.gameObject.SetActive(true);
        }

        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.dialogueTxt = "*NYEH HEH HEH HEH HEH! You cannot defeat THE GREAT PAPYRUS!";
            DialogueManager.instance.enemyTxt = "";
            DialogueManager.instance.Talking(FinishPapyrusDefeat); // callback para cerrar el combate al final
        }
        else
        {
            FinishPapyrusDefeat();
        }
    }

    // callback que se llama cuando termina la frase, sale del combate
    void FinishPapyrusDefeat()
    {
        if (BattleManager.battleInstance != null)
        {
            BattleManager.battleInstance.EndBattle();
        }
    }

    // si en el inspector estan vacios, rellena nombre, stats, dialogos y flavor text con los tipicos de Papyrus
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
