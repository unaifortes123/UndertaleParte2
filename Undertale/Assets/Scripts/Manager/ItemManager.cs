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
    void Awake() => instance = this;
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

    void Start()
    {
        isFighting = BattleManager.battleInstance.isFighting;
        maxSelectionInt = buttons.Count - 1;
        minSelectionInt = 0;
        playerStats = PlayerVars.instance;
    }

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
    void Deselecting(int deselectionInt)
    {
        if (buttons == null || buttons.Count <= deselectionInt) return;
        if (buttons[deselectionInt] == null) return;

        buttons[deselectionInt].selected = false;
    }
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
        if (buttons == null || buttons.Count == 0)
        {
            return;
        }

        maxSelectionInt = buttons.Count - 1;
        selectionInt = 0;
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

    void Selected()
    {
        ItemButtons selectedButton;

        if (buttons == null || buttons.Count <= selectionInt || buttons[selectionInt] == null)
        {
            return;
        }

        selectedButton = buttons[selectionInt];
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

        Action dialogue = () =>
        {
            DialogueManager.instance.shouldTalk = false;
            StartCoroutine(BattleManager.battleInstance.ItemSequence());
        };

        DialogueManager.instance.dialogueTxt = GetItemText(selectedButton);

        if (DialogueManager.instance.text != null)
        {
            DialogueManager.instance.text.gameObject.SetActive(true);
        }

        DialogueManager.instance.enemyTxt = BattleManager.battleInstance.GetRandomEnemyDialogue();
        DialogueManager.instance.shouldTalk = true;
        DialogueManager.instance.Talking(dialogue);
    }

    string GetItemText(ItemButtons selectedButton)
    {
        string itemName;

        itemName = string.IsNullOrWhiteSpace(selectedButton.itemName) ? "item" : selectedButton.itemName;
        return "*You used the " + itemName + ". You healed " + selectedButton.itemHeal + " HP.";
    }

    Vector3 GetSoulPosition(Transform soulPosition, Transform optionTransform, Vector3 fallbackPosition)
    {
        if (soulPosition != null && soulPosition.IsChildOf(optionTransform))
        {
            return soulPosition.position;
        }

        return fallbackPosition;
    }

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

    void RefreshSoulReference()
    {
        if (soul == null && BattleManager.battleInstance != null)
        {
            soul = BattleManager.battleInstance.soul;
        }
    }
}
