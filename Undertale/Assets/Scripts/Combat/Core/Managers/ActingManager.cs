using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ActingManager : MonoBehaviour
{
    int maxSelectionInt;
    int minSelectionInt;
    public int selectionInt;
    bool isFighting;
    public string spareMessage;
    public List<ActingButtons> buttons;
    public SpriteRenderer soul;
    public TextMeshPro actingText;
    public bool isActing;
    public GameObject actObjects;
    public int totalMercy;
    public int totalMercyMax;
    public List<string> flavorText;
    public float time;
    public bool canAct = true;

    void Start()
    {
        // los limites del menu dependen de cuantas opciones ACT haya
        maxSelectionInt = buttons.Count - 1;
        minSelectionInt = 0;
    }

    // movimiento por el menu ACT con flechas + enter para confirmar
    void Update()
    {
        isFighting = BattleManager.battleInstance.isFighting;

        if (!isFighting && isActing)
        {
            if (selectionInt > maxSelectionInt)
            {
                selectionInt = 0;
            }
            if (selectionInt < minSelectionInt)
            {
                selectionInt = maxSelectionInt;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectionInt--;
                time = 0;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectionInt++;
                time = 0;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectionInt -= 2;
                time = 0;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectionInt += 2;
                time = 0;
            }

            Selection();

            time += Time.deltaTime;

            // pequeño margen para que no se ejecute el ACT en el mismo frame que abres el menu
            if (time > 0.25f)
            {
                if (canAct && Input.GetKeyDown(KeyCode.Return))
                {
                    canAct = false;
                    Selected();
                }
            }
        }
    }

    // pone el corazon al lado de la opcion ACT marcada
    void Selecting(int selectedInt)
    {
        Vector3 fallbackPosition;
        bool canSelect;

        RefreshSoulReference();
        canSelect = soul != null && buttons != null && selectedInt >= 0 && buttons.Count > selectedInt && buttons[selectedInt] != null;

        if (canSelect == true && buttons[selectedInt].selected)
        {
            fallbackPosition = buttons[selectedInt].transform.position + Vector3.left * 1.6f;
            ShowSoul(GetSoulPosition(buttons[selectedInt].soulPosition, buttons[selectedInt].transform, fallbackPosition));
        }
    }

    void Deselecting(int deselectionInt)
    {
        if (buttons != null && deselectionInt >= 0 && buttons.Count > deselectionInt && buttons[deselectionInt] != null)
        {
            buttons[deselectionInt].selected = false;
        }
    }

    // recorre las 4 opciones, deja seleccionada la actual y quita el resto
    void Selection()
    {

        if (selectionInt == 0)
        {
            buttons[selectionInt].selected = true;
            Selecting(0);
        }
        else
        {
            Deselecting(0);
        }
        if (selectionInt == 1)
        {
            buttons[selectionInt].selected = true;
            Selecting(1);
        }
        else
        {
            Deselecting(1);
        }
        if (selectionInt == 2)
        {
            buttons[selectionInt].selected = true;
            Selecting(2);
        }
        else
        {
            Deselecting(2);
        }
        if (selectionInt == 3)
        {
            buttons[selectionInt].selected = true;
            Selecting(3);
        }
        else
        {
            Deselecting(3);
        }
    }

    // ejecuta la opcion ACT seleccionada y suma mercy/mercyMax al total
    void Selected()
    {
        if (selectionInt == 0)
        {
            OnActing(0);
            totalMercy += buttons[0].actVars.curMercy;
        }
        if (selectionInt == 1)
        {
            OnActing(1);
            totalMercy += buttons[1].actVars.curMercy;
            totalMercyMax += buttons[1].actVars.mercyMax;
        }
        if (selectionInt == 2)
        {
            OnActing(2);
            totalMercy += buttons[2].actVars.curMercy;
            totalMercyMax += buttons[2].actVars.mercyMax;
        }
        if (selectionInt == 3)
        {
            OnActing(3);
            totalMercy += buttons[3].actVars.curMercy;
            totalMercyMax += buttons[3].actVars.mercyMax;
        }
    }

    // abre el menu de ACT desde fuera (lo llama BattleManager al pulsar ACT)
    public void OpenMenu()
    {
        if (buttons != null && buttons.Count > 0)
        {
            maxSelectionInt = buttons.Count - 1;
            selectionInt = 0;
            time = 0;
            canAct = true;
            isActing = true;
            actObjects.SetActive(true);
            actingText.gameObject.SetActive(false);
            RefreshSoulReference();

            if (soul != null)
            {
                soul.enabled = true;
            }

            Selection();
        }
    }
    // se llama al elegir una opcion ACT: muestra el texto, suma mercy y mete el ataque del enemigo en cola
    public void OnActing(int selectedInt)
    {
        isActing = false;
        canAct = false;

        HideSoul();

        if (buttons[selectedInt].actVars.mercyValue != null && buttons[selectedInt].actVars.mercyValue.Count > 0)
        {
            buttons[selectedInt].actVars.curMercy += buttons[selectedInt].actVars.mercyValue[0];
        }
        actingText.gameObject.SetActive(true);
        DialogueManager.instance.dialogueTxt = GetActText(selectedInt);
        DialogueManager.instance.enemyTxt = BattleManager.battleInstance.GetRandomEnemyDialogue();
        DialogueManager.instance.shouldTalk = true;
        DialogueManager.instance.Talking(FinishActDialogue);
        actObjects.SetActive(false);

        // rota los textos del ACT: si hay pocos los duplica al final, si hay varios elimina el usado
        if (buttons[selectedInt].actVars.actTxt.Count > 0 && buttons[selectedInt].actVars.mercyValue.Count > 0)
        {
            if (buttons[selectedInt].actVars.actTxt.Count <= 2 || buttons[selectedInt].actVars.mercyValue.Count <= 2)
            {
                buttons[selectedInt].actVars.actTxt.Add(buttons[selectedInt].actVars.actTxt[0]);
                buttons[selectedInt].actVars.mercyValue.Add(buttons[selectedInt].actVars.mercyValue[0]);
            }
            else
            {
                buttons[selectedInt].actVars.actTxt.RemoveAt(0);
                buttons[selectedInt].actVars.mercyValue.RemoveAt(0);
            }
        }
    }

    // callback al acabar la frase del ACT, pasa al ataque del enemigo
    void FinishActDialogue()
    {
        DialogueManager.instance.shouldTalk = false;
        StartCoroutine(BattleManager.battleInstance.ActingSequence());
    }

    // saca el texto de la opcion ACT actual, o un generico "You used X" si no esta puesto
    string GetActText(int selectedInt)
    {
        string optionName;

        optionName = buttons[selectedInt].gameObject.name;

        if (buttons[selectedInt].actVars != null && buttons[selectedInt].actVars.actTxt != null && buttons[selectedInt].actVars.actTxt.Count > 0)
        {
            if (!string.IsNullOrWhiteSpace(buttons[selectedInt].actVars.actTxt[0]))
            {
                return buttons[selectedInt].actVars.actTxt[0];
            }
        }

        return "*You used " + optionName + ".";
    }

    // si el boton tiene un Transform marcado para el alma usa ese, si no lo deja en el fallback (a su izquierda)
    Vector3 GetSoulPosition(Transform soulPosition, Transform optionTransform, Vector3 fallbackPosition)
    {
        if (soulPosition != null && soulPosition.IsChildOf(optionTransform))
        {
            return soulPosition.position;
        }

        return fallbackPosition;
    }

    // mete el alma en la posicion indicada, prefiere delegar en el BattleManager si esta vivo
    void ShowSoul(Vector3 position)
    {
        if (BattleManager.battleInstance != null)
        {
            BattleManager.battleInstance.ShowSoulInMenu(position);
        }
        else if (soul != null)
        {
            soul.transform.position = position;
            soul.enabled = true;
        }
    }

    void HideSoul()
    {
        if (BattleManager.battleInstance != null)
        {
            BattleManager.battleInstance.HideSoulForMenu();
        }
        else if (soul != null)
        {
            soul.enabled = false;
        }
    }

    // si se queda sin referencia al alma la pilla del BattleManager
    void RefreshSoulReference()
    {
        if (soul == null && BattleManager.battleInstance != null)
        {
            soul = BattleManager.battleInstance.soul;
        }
    }


}
