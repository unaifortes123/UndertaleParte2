using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using static PlayerVars;

public class BattleManager : MonoBehaviour
{
    [HideInInspector]
    public bool isFighting;
    [HideInInspector]
    public static BattleManager battleInstance;
    private AttackManager attackMgr;
    public ActingManager actingMgr;
    public AudioManager audioMgr;
    public SpriteRenderer soul;
    public SpriteRenderer battleBox;
    public List<Buttons> buttons;
    int maxSelectionInt;
    int minSelectionInt;
    public int selectionInt;
    public GameObject mercyMenu;
    public GameObject damageSprite;
    const float SIZE_INCREASE = 18f;
    public List<string> enemyDialogue;
    private SaveManager saveManager;
    private string json;
    private float xScale;
    private bool leftPressed;
    private bool rightPressed;
    private bool acceptPressed;


    [HideInInspector]
    public Action isFinished;
    public Attacking attackingSys;
    PlayerVars playerVariables;
    private PlayerController stats;
    public GameObject healthMeter;
    public TextMeshPro healthTxt;
    private EnemyVars enemyStats;
    private bool enemyCombatConfigured;
    private float damage;
    private bool battleEnding;
    private PlayerMovement soulMovement;
    private Rigidbody2D soulRigidbody;
    private bool menuInputLocked;
    private const int SOUL_SORTING_ORDER = 50;

    void Awake()
    {
        battleInstance = this;
    }


    public void Attacking()
    {
        if (!isFighting)
        {
            menuInputLocked = true;
            actingMgr.actingText.gameObject.SetActive(false);
            AudioManager.instance.Selecting();
            StartCoroutine(AttackSequence());
        }

    }
    /// <summary>
    /// The acting method, gets called when the player selects the act button, responsible for initiating acts
    /// </summary>
    public void Acting()
    {
        if (actingMgr == null)
        {
            return;
        }

        actingMgr.OpenMenu();
        menuInputLocked = true;
        audioMgr.Selecting();
    }
    /// <summary>
    /// The item method, does nothing so far
    /// </summary>
    public void Item()
    {
        if (ItemManager.instance == null)
        {
            return;
        }

        isFighting = false;
        ItemManager.instance.OpenMenu();
        menuInputLocked = true;
        audioMgr.Selecting();
    }

    // Es la funcion del Mercy, se encargara de la ruta pacifista.
    public void Mercy()
    {
        if (actingMgr.totalMercy >= actingMgr.totalMercyMax)
        {
            DialogueManager.instance.enemyTxt = "*Blushes Deeply*";
            DialogueManager.instance.Talking(null);
            audioMgr.Selecting();
        }
    }

    // Se executara al principi.
    void Start()
    {
        selectionInt = 0;
        maxSelectionInt = 3;
        minSelectionInt = 0;

        attackMgr = AttackManager.instance;
        enemyStats = FindObjectOfType<EnemyVars>();
        playerVariables = PlayerVars.instance;
        saveManager = SaveManager.instance;
        ConfigureEnemyCombat();
        ConfigureSoulReferences();

        Selection();
    }

    void GetButtonsFromScene()
    {
        GameObject fightButton;
        GameObject actButton;
        GameObject itemButton;
        GameObject mercyButton;

        Buttons fightButtons;
        Buttons actButtons;
        Buttons itemButtons;
        Buttons mercyButtons;

        fightButton = GameObject.Find("Fight Btn");
        actButton = GameObject.Find("Act Btn");
        itemButton = GameObject.Find("Item Btn");
        mercyButton = GameObject.Find("Mercy Btn");

        // Todos estos if, son para saber por consola si no encuntra los GameObjects (o no estan).
        if (fightButton == null)
        {
            Debug.LogError("No se ha encontrado Fight Btn");
            return;
        }

        if (actButton == null)
        {
            Debug.LogError("No se ha encontrado Act Btn");
            return;
        }

        if (itemButton == null)
        {
            Debug.LogError("No se ha encontrado Item Btn");
            return;
        }

        if (mercyButton == null)
        {
            Debug.LogError("No se ha encontrado Mercy Btn");
            return;
        }

        fightButtons = fightButton.GetComponent<Buttons>();
        actButtons = actButton.GetComponent<Buttons>();
        itemButtons = itemButton.GetComponent<Buttons>();
        mercyButtons = mercyButton.GetComponent<Buttons>();

        if (fightButtons == null)
        {
            Debug.LogError("Fight Btn no tiene el script Buttons");
            return;
        }

        if (actButtons == null)
        {
            Debug.LogError("Act Btn no tiene el script Buttons");
            return;
        }

        if (itemButtons == null)
        {
            Debug.LogError("Item Btn no tiene el script Buttons");
            return;
        }

        if (mercyButtons == null)
        {
            Debug.LogError("Mercy Btn no tiene el script Buttons");
            return;
        }

        buttons = new List<Buttons>();

        buttons.Add(fightButtons);
        buttons.Add(actButtons);
        buttons.Add(itemButtons);
        buttons.Add(mercyButtons);

        Debug.Log("BOTONES CARGADOS DESDE LA ESCENA");
    }

    void Update()
    {
        if (playerVariables == null)
        {
            playerVariables = PlayerVars.instance;
        }

        if (enemyStats == null)
        {
            enemyStats = FindObjectOfType<EnemyVars>();
        }

        if (attackMgr == null)
        {
            attackMgr = AttackManager.instance;
        }

        ConfigureEnemyCombat();

        if (CanReadMainMenuInput())
        {
            if (selectionInt > maxSelectionInt)
            {
                selectionInt = 0;
            }

            if (selectionInt < minSelectionInt)
            {
                selectionInt = 3;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectionInt--;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectionInt++;
            }

            Selection();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                Selected();
            }

            if (attackMgr != null)
            {
                if (!attackMgr.attackFinished)
                {
                    battleBox.size = new Vector2(11.5f, 3);
                }
            }
        }

        if (playerVariables != null)
        {
            float xScale = (1f * playerVariables.playerData.health) / 20;

            healthTxt.text = playerVariables.playerData.health + "   /   20";

            healthMeter.transform.localScale = new Vector3(xScale, healthMeter.transform.localScale.y, healthMeter.transform.localScale.z);
        }

        // Si la salud del enemigo es 0 o menor, se acaba el combate.
        if (enemyStats != null && enemyStats.curHP <= 0 && !battleEnding)
        {
            EndBattle();
        }
            
    }

    void CheckSelectionLimits()
    {
        if (selectionInt > maxSelectionInt)
        {
            selectionInt = minSelectionInt;
        }

        if (selectionInt < minSelectionInt)
        {
            selectionInt = maxSelectionInt;
        }
    }

    void HideSoul()
    {
        if (soul != null)
        {
            soul.enabled = false;
        }

        SetSoulMovement(false);
    }

    bool CanReadMainMenuInput()
    {
        bool dialogueReady;

        dialogueReady = DialogueManager.instance == null || DialogueManager.instance.done;
        return !menuInputLocked && dialogueReady && !isFighting && !actingMgr.isActing && !ItemManager.instance.isMenu;
    }

    public void LockMenuInput()
    {
        menuInputLocked = true;
    }

    public void UnlockMenuInput()
    {
        menuInputLocked = false;
    }

    void ShowSoulInBattleBox()
    {
        if (soul != null && battleBox != null)
        {
            soul.transform.position = battleBox.transform.position;
            soul.enabled = true;
            SetSoulMovement(true);
        }
    }

    public void ShowSoulInMenu(Vector3 position)
    {
        if (soul == null)
        {
            return;
        }

        soul.transform.position = position;
        soul.enabled = true;
        SetSoulMovement(false);
    }

    public void HideSoulForMenu()
    {
        HideSoul();
    }

    void ConfigureSoulReferences()
    {
        if (soul == null)
        {
            return;
        }

        if (soulMovement == null)
        {
            soulMovement = soul.GetComponent<PlayerMovement>();
        }

        if (soulRigidbody == null)
        {
            soulRigidbody = soul.GetComponent<Rigidbody2D>();
        }

        soul.sortingOrder = SOUL_SORTING_ORDER;
    }

    void SetSoulMovement(bool canMove)
    {
        ConfigureSoulReferences();

        if (soulMovement != null)
        {
            soulMovement.enabled = canMove;
        }

        if (!canMove && soulRigidbody != null)
        {
            soulRigidbody.velocity = Vector2.zero;
        }
    }

    void ConfigureEnemyCombat()
    {
        bool canConfigure;

        canConfigure = false;

        if (!enemyCombatConfigured)
        {
            if (enemyStats == null)
            {
                enemyStats = FindObjectOfType<EnemyVars>();
            }

            if (enemyStats != null && attackMgr != null && actingMgr != null)
            {
                canConfigure = true;
            }

            if (canConfigure)
            {
                enemyStats.ApplyCombatSetup(this, attackMgr, actingMgr);
                enemyCombatConfigured = true;
            }
        }
    }

    public string GetRandomEnemyDialogue()
    {
        string dialogue;

        dialogue = GetRandomNotEmptyText(enemyDialogue);

        if (!string.IsNullOrWhiteSpace(dialogue))
        {
            return dialogue;
        }

        return "*Peter se queda callado por un momento.";
    }

    public string GetPostTurnText()
    {
        string dialogue;

        if (actingMgr != null && actingMgr.totalMercy >= actingMgr.totalMercyMax && !string.IsNullOrWhiteSpace(actingMgr.spareMessage))
        {
            return actingMgr.spareMessage;
        }

        dialogue = actingMgr != null ? GetRandomNotEmptyText(actingMgr.flavorText) : "";

        if (!string.IsNullOrWhiteSpace(dialogue))
        {
            return dialogue;
        }

        return "*Peter espera tu siguiente movimiento.";
    }

    string GetRandomNotEmptyText(List<string> texts)
    {
        List<string> validTexts;

        if (texts == null || texts.Count == 0)
        {
            return "";
        }

        validTexts = texts.FindAll(textValue => !string.IsNullOrWhiteSpace(textValue));

        if (validTexts.Count == 0)
        {
            return "";
        }

        return validTexts[UnityEngine.Random.Range(0, validTexts.Count)];
    }

    /// <summary>
    /// The method responsible for selecting, when a method is selected the soul will be positioned on the selected button.
    /// </summary>
    /// <param name="selectedInt"></param>

    void Selecting(int selectedInt)
    {
        if (buttons == null || buttons.Count <= selectedInt)
        {
            return;
        }

        if (buttons[selectedInt] == null)
        {
            return;
        }

        if (soul != null && buttons[selectedInt].soulPosition != null)
        {
            ShowSoulInMenu(buttons[selectedInt].soulPosition.position);
        }

        buttons[selectedInt].SelectButton();
    }

    /// <summary>
    /// The method responsible for deSelecting
    /// </summary>
    /// <param name="deselectionInt"></param>
    void Deselecting(int deselectionInt)
    {
        if (buttons == null || buttons.Count <= deselectionInt)
        {
            return;
        }

        if (buttons[deselectionInt] == null)
        {
            return;
        }

        buttons[deselectionInt].DeselectButton();
    }

    // Esta funcion se encarga de volver al 
    void Selection()
    {
        int i;

        if (buttons == null)
        {
            return;
        }

        for (i = 0; i < buttons.Count; i++)
        {
            if (i == selectionInt)
            {
                Selecting(i);
            }
            else
            {
                Deselecting(i);
            }
        }
    }


    // Esta funcion, llama a los metodos en el inicio para iniciar cada accion respectiva de cada boton.
    void Selected()
    {
        Debug.Log("SELECTION INT: " + selectionInt);
        if (selectionInt == 0)
        {
            Debug.Log("FIGHT SELECTED");
            Attacking();
        }
        if (selectionInt == 1)
        {
            Debug.Log("ACT SELECTED");
            Acting();
        }
        if (selectionInt == 2)
        {
            Debug.Log("ITEM SELECTED");
            Item();
        }
        if (selectionInt == 3)
        {
            Debug.Log("MERCY SELECTED");
            Mercy();
        }
    }

    // Esta funcion se encarga de iniciar la secuencia de ataque.
    IEnumerator AttackSequence()
    {
        isFighting = true;

        HideSoul();

        Action onBoxFinish = () =>
        {
            actingMgr.actingText.gameObject.SetActive(true);
        };

        isFinished = () =>
        {
            HideSoul();

            StartCoroutine(ResizeBattleBox(new Vector2(11.5f, 3), onBoxFinish));
            attackMgr.attackFinished = !attackMgr.attackFinished;
            isFighting = false;
            UnlockMenuInput();
        };

        attackingSys.StartAttacking(playerVariables.atkValue);

        yield return new WaitForSeconds(attackingSys.maxTime);

        ShowSoulInBattleBox();

        StartCoroutine(ResizeBattleBox(new Vector2(3, 3), null));
        actingMgr.actingText.gameObject.SetActive(false);

        attackMgr.StartAttack(attackMgr.GetAttack(), isFinished);
    }

    public IEnumerator ActingSequence()
    {
        Action boxAction = () =>
        {
            DialogueManager.instance.dialogueTxt = GetPostTurnText();
        };

        HideSoul();

        isFinished = () =>
        {
            HideSoul();

            StartCoroutine(ResizeBattleBox(new Vector2(11.5f, 3f), boxAction));
            actingMgr.isActing = false;
            isFighting = false;
            actingMgr.actingText.gameObject.SetActive(true);
            DialogueManager.instance.Talking(null);
            actingMgr.actObjects.SetActive(false);
            actingMgr.canAct = true;
            UnlockMenuInput();
        };

        yield return new WaitForSeconds(1);

        ShowSoulInBattleBox();

        StartCoroutine(ResizeBattleBox(new Vector2(3, 3), boxAction));
        actingMgr.actingText.gameObject.SetActive(false);
        actingMgr.isActing = false;
        isFighting = true;
        actingMgr.time = 0;

        attackMgr.StartAttack(attackMgr.GetAttack(), isFinished);
    }

    public IEnumerator ItemSequence()
    {
        HideSoul();

        isFinished = () =>
        {
            DialogueManager.instance.dialogueTxt = GetPostTurnText();

            ItemManager.instance.time = 0;

            HideSoul();

            StartCoroutine(ResizeBattleBox(new Vector2(11.5f, 3f), null));
            actingMgr.isActing = false;
            isFighting = false;
            actingMgr.actingText.gameObject.SetActive(true);
            DialogueManager.instance.Talking(null);
            actingMgr.actObjects.SetActive(false);
            ItemManager.instance.itemObjects.SetActive(false);
            actingMgr.canAct = true;
            UnlockMenuInput();
        };

        yield return new WaitForSeconds(1);

        ShowSoulInBattleBox();

        StartCoroutine(ResizeBattleBox(new Vector2(3, 3), null));
        ItemManager.instance.itemObjects.SetActive(false);
        isFighting = true;
        ItemManager.instance.isMenu = false;
        ItemManager.instance.useText.text = "";
        actingMgr.time = 0;

        attackMgr.StartAttack(attackMgr.GetAttack(), isFinished);
    }


    IEnumerator ResizeBattleBox(Vector2 targetSize, Action onFinish)
    {
        Vector2 startSize = battleBox.size;
        float xSign = Mathf.Sign(targetSize.x - startSize.x);
        float ySign = Mathf.Sign(targetSize.y - startSize.y);

        Vector2 size = startSize;
        while (size.x != targetSize.x || size.y != targetSize.y)
        {
            size.x += xSign * SIZE_INCREASE * Time.deltaTime;
            size.y += ySign * SIZE_INCREASE * Time.deltaTime;

            if ((xSign == 1 && size.x > targetSize.x) || (xSign == -1 && size.x < targetSize.x))
            {
                size.x = targetSize.x;
            }
            if ((ySign == 1 && size.y > targetSize.y) || (ySign == -1 && size.y < targetSize.y))
            {
                size.y = targetSize.y;
            }

            battleBox.size = size;
            yield return null;
        }
        onFinish?.Invoke();

    }

    public void EndBattle() // Funcion que hace que salgas del combate.
    {

        Debug.Log("ENDING BATTLE...");
        battleEnding = true;


        if (playerVariables == null)
        {
            playerVariables = PlayerVars.instance;
        }

        if (saveManager == null)
        {
            saveManager = SaveManager.instance;
        }

        if (playerVariables == null)
        {
            Debug.LogError("playerVariables is null");
            return;
        }

        if (saveManager == null)
        {
            Debug.LogError("saveManager is null");
            return;
        }


        isFighting = false;

        playerVariables.playerData.health = 20;
        playerVariables.playerData.score += 100;

        if (!playerVariables.playerData.completedFights.Contains("Combat2"))
        {
            playerVariables.playerData.completedFights.Add("Combat2");
        }

        json = JsonUtility.ToJson(playerVariables.playerData, true);
        saveManager.SaveGame(json);

        Debug.Log(json);
        
        DialogueManager.instance.enemyTxt = "";
        DialogueManager.instance.Talking(null);

        playerVariables.ClearSoulSprite();


        SceneManager.LoadScene("PruebaEntradaEnCombate");

    }

    void OnDestroy()
    {
        if (playerVariables != null)
        {
            playerVariables.ClearSoulSprite();
        }
    }

}
