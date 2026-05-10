using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public List<ItemButtons> buttons;
    [HideInInspector]
    public static ItemManager instance;
    // Esta funcion guarda este manager de items para los demas scripts.
    void Awake()
    {
        instance = this;
    }
    int maxSelectionInt;
    int minSelectionInt;
    public int selectionInt;
    public SpriteRenderer soul;
    public TextMeshPro useText;
    public GameObject itemObjects;
    bool isFighting;
    public float time;
    public bool isMenu;
    public bool canAct = true;
    private PlayerVars playerStats;

    // Esta funcion prepara los limites del menu ITEMS.
    void Start()
    {
        isFighting = BattleManager.battleInstance.isFighting;
        maxSelectionInt = buttons.Count - 1;
        minSelectionInt = 0;
        playerStats = PlayerVars.instance;
    }

    // Esta funcion mueve el menu ITEMS y detecta Enter.
    void Update()
    {
        if (BattleManager.battleInstance != null)
        {
            isFighting = BattleManager.battleInstance.isFighting;
        }

        if (playerStats == null)
        {
            playerStats = PlayerVars.instance;
        }

        if (!isFighting && isMenu)
        {
            maxSelectionInt = buttons.Count - 1;

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
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectionInt++;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectionInt -= 2;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectionInt += 2;
            }
            MoveSelectionToAvailableItem();
            Selection();
            time += Time.deltaTime;
            if (time > 0.25f)
            {
                if (canAct && DialogueManager.instance.done)
                {
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        canAct = false;
                        Selected();
                    }
                }


            }

        }

    }

    // Esta funcion pone el corazon al lado del item marcado.
    void Selecting(int selectedInt)
    {
        Vector3 fallbackPosition;
        bool canSelect;

        RefreshSoulReference();
        canSelect = soul != null && buttons != null && selectedInt >= 0 && buttons.Count > selectedInt && buttons[selectedInt] != null && CanUseItem(buttons[selectedInt]);

        if (canSelect == true && buttons[selectedInt].selected)
        {
            fallbackPosition = buttons[selectedInt].transform.position + Vector3.left * 1.6f;
            ShowSoul(GetSoulPosition(buttons[selectedInt].soulPosition, buttons[selectedInt].transform, fallbackPosition));
        }
    }
    // Esta funcion quita la marca de un item.
    void Deselecting(int deselectionInt)
    {
        if (buttons != null && deselectionInt >= 0 && buttons.Count > deselectionInt && buttons[deselectionInt] != null)
        {
            buttons[deselectionInt].selected = false;
        }
    }
    // Esta funcion actualiza que item esta marcado.
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

    // Esta funcion abre el menu de objetos.
    public void OpenMenu()
    {
        if (buttons != null && buttons.Count > 0)
        {
            if (!HasAvailableItems())
            {
                useText.text = "*You have no items left.";
                useText.gameObject.SetActive(true);
                itemObjects.SetActive(false);
                isMenu = false;

                if (BattleManager.battleInstance != null)
                {
                    BattleManager.battleInstance.UnlockMenuInput();
                }
            }
            else
            {
                maxSelectionInt = buttons.Count - 1;
                selectionInt = 0;
                MoveSelectionToAvailableItem();
                time = 0;
                canAct = true;
                isMenu = true;
                itemObjects.SetActive(true);
                useText.gameObject.SetActive(false);
                RefreshSoulReference();

                if (soul != null)
                {
                    soul.enabled = true;
                }

                Selection();
            }
        }
    }

    // Esta funcion usa el item seleccionado.
    void Selected()
    {
        ItemButtons selectedButton;

        selectedButton = null;

        if (buttons != null && selectionInt >= 0 && buttons.Count > selectionInt && buttons[selectionInt] != null)
        {
            selectedButton = buttons[selectionInt];

            if (!CanUseItem(selectedButton))
            {
                canAct = true;
                MoveSelectionToAvailableItem();
                Selection();
            }
            else
            {
                isMenu = false;
                canAct = false;

                if (itemObjects != null)
                {
                    itemObjects.SetActive(false);
                }

                HideSoul();

                if (playerStats == null)
                {
                    playerStats = PlayerVars.instance;
                }

                if (playerStats != null)
                {
                    playerStats.playerData.health += selectedButton.itemHeal;

                    if (playerStats.playerData.health > 20)
                    {
                        playerStats.playerData.health = 20;
                    }
                }

                DisableUsedItem(selectedButton);

                DialogueManager.instance.dialogueTxt = GetItemText(selectedButton);

                if (DialogueManager.instance.text != null)
                {
                    DialogueManager.instance.text.gameObject.SetActive(true);
                }

                DialogueManager.instance.enemyTxt = BattleManager.battleInstance.GetRandomEnemyDialogue();
                DialogueManager.instance.shouldTalk = true;
                DialogueManager.instance.Talking(FinishItemDialogue);
            }
        }
    }

    // Esta funcion pasa de ITEMS al ataque del enemigo.
    void FinishItemDialogue()
    {
        DialogueManager.instance.shouldTalk = false;
        StartCoroutine(BattleManager.battleInstance.ItemSequence());
    }

    // Esta funcion crea el texto que sale al usar un item.
    string GetItemText(ItemButtons selectedButton)
    {
        string itemName;

        itemName = string.IsNullOrWhiteSpace(selectedButton.itemName) ? "item" : selectedButton.itemName;
        return "*You used the " + itemName + ". You healed " + selectedButton.itemHeal + " HP.";
    }

    // Esta funcion calcula donde va el corazon en ITEMS.
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
        }
        else if (soul != null)
        {
            soul.transform.position = position;
            soul.enabled = true;
        }
    }

    // Esta funcion esconde el corazon en ITEMS.
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

    // Esta funcion recupera el corazon desde el BattleManager.
    void RefreshSoulReference()
    {
        if (soul == null && BattleManager.battleInstance != null)
        {
            soul = BattleManager.battleInstance.soul;
        }
    }

    // Esta funcion dice si aun queda algun item usable.
    bool HasAvailableItems()
    {
        bool hasItem;
        int i;

        hasItem = false;
        i = 0;

        while (i < buttons.Count)
        {
            if (CanUseItem(buttons[i]))
            {
                hasItem = true;
            }

            i++;
        }

        return hasItem;
    }

    // Esta funcion dice si este item se puede usar.
    bool CanUseItem(ItemButtons itemButton)
    {
        bool canUse;

        canUse = false;

        if (itemButton != null && !itemButton.used && itemButton.gameObject.activeSelf)
        {
            canUse = true;
        }

        return canUse;
    }

    // Esta funcion desactiva el item usado y los que sean del mismo tipo.
    void DisableUsedItem(ItemButtons selectedButton)
    {
        int i;
        ItemButtons itemButton;

        i = 0;

        while (i < buttons.Count)
        {
            itemButton = buttons[i];

            if (itemButton != null)
            {
                if (itemButton == selectedButton || itemButton.itemName == selectedButton.itemName)
                {
                    itemButton.used = true;
                    itemButton.selected = false;
                    itemButton.gameObject.SetActive(false);
                }
            }

            i++;
        }
    }

    // Esta funcion mueve el corazon a un item que exista.
    void MoveSelectionToAvailableItem()
    {
        int tries;

        tries = 0;

        if (selectionInt > maxSelectionInt)
        {
            selectionInt = minSelectionInt;
        }

        if (selectionInt < minSelectionInt)
        {
            selectionInt = maxSelectionInt;
        }

        while (buttons != null && buttons.Count > 0 && !CanUseItem(buttons[selectionInt]) && tries < buttons.Count)
        {
            selectionInt++;

            if (selectionInt > maxSelectionInt)
            {
                selectionInt = minSelectionInt;
            }

            tries++;
        }
    }
}
