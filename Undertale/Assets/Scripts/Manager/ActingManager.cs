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
    // Esta funcion prepara los limites del menu ACT.
    void Start()
    {
        maxSelectionInt = buttons.Count - 1;
        minSelectionInt = 0;
    }

    // Esta funcion mueve el menu ACT y detecta Enter.
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

    // Esta funcion pone el corazon al lado de la opcion ACT marcada.
    void Selecting(int selectedInt)
    {
        Vector3 fallbackPosition;

        RefreshSoulReference();

        if (soul == null) return;
        if (buttons == null || buttons.Count <= selectedInt) return;
        if (buttons[selectedInt] == null) return;

        if (buttons[selectedInt].selected)
        {
            fallbackPosition = buttons[selectedInt].transform.position + Vector3.left * 1.6f;
            ShowSoul(GetSoulPosition(buttons[selectedInt].soulPosition, buttons[selectedInt].transform, fallbackPosition));
        }
    }

    // Esta funcion quita la marca de una opcion ACT.
    void Deselecting(int deselectionInt)
    {
        if (buttons == null || buttons.Count <= deselectionInt) return;
        if (buttons[deselectionInt] == null) return;

        buttons[deselectionInt].selected = false;
    }
    // Esta funcion actualiza que opcion ACT esta marcada.
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

    // Esta funcion ejecuta la opcion ACT elegida.
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

    // Esta funcion abre el menu de ACT.
    public void OpenMenu()
    {
        if (buttons == null || buttons.Count == 0)
        {
            return;
        }

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
    // Esta funcion muestra el texto de ACT y prepara el turno del enemigo.
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

        if (buttons[selectedInt].actVars.actTxt.Count > 0 && buttons[selectedInt].actVars.mercyValue.Count > 0)
        {
            if (buttons[selectedInt].actVars.actTxt.Count <= 2 || buttons[selectedInt].actVars.mercyValue.Count <= 2)
            {
                Debug.Log("We added");
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

    // Esta funcion pasa de ACT al ataque del enemigo.
    void FinishActDialogue()
    {
        Debug.Log("action initiated");
        DialogueManager.instance.shouldTalk = false;
        StartCoroutine(BattleManager.battleInstance.ActingSequence());
    }

    // Esta funcion consigue el texto de la opcion ACT.
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

    // Esta funcion calcula donde va el corazon en el menu ACT.
    Vector3 GetSoulPosition(Transform soulPosition, Transform optionTransform, Vector3 fallbackPosition)
    {
        if (soulPosition != null && soulPosition.IsChildOf(optionTransform))
        {
            return soulPosition.position;
        }

        return fallbackPosition;
    }

    // Esta funcion muestra el corazon en la posicion indicada.
    void ShowSoul(Vector3 position)
    {
        if (BattleManager.battleInstance != null)
        {
            BattleManager.battleInstance.ShowSoulInMenu(position);
            return;
        }

        if (soul != null)
        {
            soul.transform.position = position;
            soul.enabled = true;
        }
    }

    // Esta funcion esconde el corazon en ACT.
    void HideSoul()
    {
        if (BattleManager.battleInstance != null)
        {
            BattleManager.battleInstance.HideSoulForMenu();
            return;
        }

        if (soul != null)
        {
            soul.enabled = false;
        }
    }

    // Esta funcion recupera el corazon desde el BattleManager.
    void RefreshSoulReference()
    {
        if (soul == null && BattleManager.battleInstance != null)
        {
            soul = BattleManager.battleInstance.soul;
        }
    }


}
