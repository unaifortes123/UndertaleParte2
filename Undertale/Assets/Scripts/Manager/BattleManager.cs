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
    private string currentFightName;
    private PlayerMovement soulMovement;
    private Rigidbody2D soulRigidbody;
    private bool menuInputLocked;
    private const int SOUL_SORTING_ORDER = 50;

    // Esta funcion guarda este manager para que los demas scripts lo encuentren.
    void Awake()
    {
        battleInstance = this;
    }

    // Esta funcion empieza la opcion FIGHT.
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

    // Esta funcion abre la opcion ACT.
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

    // Esta funcion abre la opcion ITEMS.
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

    // Esta funcion intenta perdonar al enemigo.
    public void Mercy()
    {
        if (actingMgr.totalMercy >= actingMgr.totalMercyMax)
        {
            DialogueManager.instance.enemyTxt = "*Blushes Deeply*";
            DialogueManager.instance.Talking(null);
            audioMgr.Selecting();
        }
    }

    // Esta funcion prepara el combate al empezar.
    void Start()
    {
        selectionInt = 0;
        maxSelectionInt = 3;
        minSelectionInt = 0;

        attackMgr = AttackManager.instance;
        enemyStats = FindObjectOfType<EnemyVars>();
        playerVariables = PlayerVars.instance;
        saveManager = SaveManager.instance;
        currentFightName = SceneManager.GetActiveScene().name;
        ConfigureEnemyCombat();
        ConfigureSoulReferences();

        Selection();
    }

    // Esta funcion busca los botones en la escena si hace falta.
    void GetButtonsFromScene()
    {
        Buttons fightButtons;
        Buttons actButtons;
        Buttons itemButtons;
        Buttons mercyButtons;

        fightButtons = GetButtonFromScene("Fight Btn");
        actButtons = GetButtonFromScene("Act Btn");
        itemButtons = GetButtonFromScene("Item Btn");
        mercyButtons = GetButtonFromScene("Mercy Btn");

        if (fightButtons != null && actButtons != null && itemButtons != null && mercyButtons != null)
        {
            buttons = new List<Buttons>();

            buttons.Add(fightButtons);
            buttons.Add(actButtons);
            buttons.Add(itemButtons);
            buttons.Add(mercyButtons);

            Debug.Log("BOTONES CARGADOS DESDE LA ESCENA");
        }
    }

    // Esta funcion busca un boton y avisa si falta algo.
    Buttons GetButtonFromScene(string buttonName)
    {
        GameObject buttonObject;
        Buttons buttonScript;

        buttonScript = null;
        buttonObject = GameObject.Find(buttonName);

        if (buttonObject == null)
        {
            Debug.LogError("No se ha encontrado " + buttonName);
        }
        else
        {
            buttonScript = buttonObject.GetComponent<Buttons>();

            if (buttonScript == null)
            {
                Debug.LogError(buttonName + " no tiene el script Buttons");
            }
        }

        return buttonScript;
    }

    // Esta funcion controla el menu principal y actualiza la vida.
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

    // Esta funcion evita que la seleccion se salga de los botones.
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

    // Esta funcion esconde el corazon y le quita movimiento.
    void HideSoul()
    {
        if (soul != null)
        {
            soul.enabled = false;
        }

        SetSoulMovement(false);
    }

    // Esta funcion dice si ahora se puede mover el menu principal.
    bool CanReadMainMenuInput()
    {
        bool dialogueReady;

        dialogueReady = DialogueManager.instance == null || DialogueManager.instance.done;
        return !menuInputLocked && dialogueReady && !isFighting && !actingMgr.isActing && !ItemManager.instance.isMenu;
    }

    // Esta funcion bloquea el menu para que no se mueva mientras hay texto.
    public void LockMenuInput()
    {
        menuInputLocked = true;
    }

    // Esta funcion vuelve a dejar usar el menu.
    public void UnlockMenuInput()
    {
        menuInputLocked = false;
    }

    // Esta funcion pone el corazon dentro de la caja de batalla.
    void ShowSoulInBattleBox()
    {
        if (soul != null && battleBox != null)
        {
            soul.transform.position = battleBox.transform.position;
            soul.enabled = true;
            SetSoulMovement(true);
        }
    }

    // Esta funcion pone el corazon al lado de una opcion del menu.
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

    // Esta funcion esconde el corazon cuando se cierra un menu.
    public void HideSoulForMenu()
    {
        HideSoul();
    }

    // Esta funcion prepara las referencias del corazon.
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

    // Esta funcion activa o desactiva el movimiento del corazon.
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

    // Esta funcion carga en los managers los datos del enemigo actual.
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

    // Esta funcion devuelve un dialogo aleatorio del enemigo.
    public string GetRandomEnemyDialogue()
    {
        string dialogue;

        dialogue = GetRandomNotEmptyText(enemyDialogue);

        if (string.IsNullOrWhiteSpace(dialogue))
        {
            dialogue = "*" + GetEnemyDisplayName() + " no dice nada por un momento.";
        }

        return dialogue;
    }

    // Esta funcion devuelve el texto que sale despues del turno.
    public string GetPostTurnText()
    {
        string dialogue;

        dialogue = "";

        if (actingMgr != null && actingMgr.totalMercy >= actingMgr.totalMercyMax && !string.IsNullOrWhiteSpace(actingMgr.spareMessage))
        {
            dialogue = actingMgr.spareMessage;
        }

        if (string.IsNullOrWhiteSpace(dialogue) && actingMgr != null)
        {
            dialogue = GetRandomNotEmptyText(actingMgr.flavorText);
        }

        if (string.IsNullOrWhiteSpace(dialogue))
        {
            dialogue = "*" + GetEnemyDisplayName() + " espera tu siguiente movimiento.";
        }

        return dialogue;
    }

    // Esta funcion escoge un texto que no este vacio.
    string GetRandomNotEmptyText(List<string> texts)
    {
        List<string> validTexts;
        string text;
        int i;

        text = "";
        if (texts == null || texts.Count == 0)
        {
            validTexts = new List<string>();
        }
        else
        {
            validTexts = new List<string>();

            for (i = 0; i < texts.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(texts[i]))
                {
                    validTexts.Add(texts[i]);
                }
            }
        }

        if (validTexts.Count > 0)
        {
            text = validTexts[UnityEngine.Random.Range(0, validTexts.Count)];
        }

        return text;
    }

    // Esta funcion coge el nombre del enemigo para los textos de emergencia.
    string GetEnemyDisplayName()
    {
        string enemyDisplayName;

        enemyDisplayName = "El enemigo";
        if (enemyStats != null && !string.IsNullOrWhiteSpace(enemyStats.enemyName))
        {
            enemyDisplayName = enemyStats.enemyName;
        }

        return enemyDisplayName;
    }

    // Esta funcion selecciona un boton y mueve el corazon a su lado.
    void Selecting(int selectedInt)
    {
        if (buttons != null && buttons.Count > selectedInt && buttons[selectedInt] != null)
        {
            if (soul != null && buttons[selectedInt].soulPosition != null)
            {
                ShowSoulInMenu(buttons[selectedInt].soulPosition.position);
            }

            buttons[selectedInt].SelectButton();
        }
    }

    // Esta funcion quita la seleccion de un boton.
    void Deselecting(int deselectionInt)
    {
        if (buttons != null && buttons.Count > deselectionInt && buttons[deselectionInt] != null)
        {
            buttons[deselectionInt].DeselectButton();
        }
    }

    // Esta funcion actualiza cual boton esta seleccionado.
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


    // Esta funcion ejecuta la opcion marcada del menu.
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

    // Esta funcion inicia la secuencia de ataque del player y luego la del enemigo.
    IEnumerator AttackSequence()
    {
        isFighting = true;

        HideSoul();
        isFinished = FinishAttackSequence;

        attackingSys.StartAttacking(playerVariables.atkValue);

        yield return new WaitForSeconds(attackingSys.maxTime);

        ShowSoulInBattleBox();

        StartCoroutine(ResizeBattleBox(new Vector2(3, 3), null));
        actingMgr.actingText.gameObject.SetActive(false);

        attackMgr.StartAttack(attackMgr.GetAttack(), isFinished);
    }

    // Esta funcion cierra ACT y empieza el turno del enemigo.
    public IEnumerator ActingSequence()
    {
        HideSoul();
        isFinished = FinishActingSequence;

        yield return new WaitForSeconds(1);

        ShowSoulInBattleBox();

        StartCoroutine(ResizeBattleBox(new Vector2(3, 3), SetPostTurnText));
        actingMgr.actingText.gameObject.SetActive(false);
        actingMgr.isActing = false;
        isFighting = true;
        actingMgr.time = 0;

        attackMgr.StartAttack(attackMgr.GetAttack(), isFinished);
    }

    // Esta funcion cierra ITEMS y empieza el turno del enemigo.
    public IEnumerator ItemSequence()
    {
        HideSoul();
        isFinished = FinishItemSequence;

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

    // Esta funcion vuelve al menu despues del ataque FIGHT.
    void FinishAttackSequence()
    {
        HideSoul();
        StartCoroutine(ResizeBattleBox(new Vector2(11.5f, 3), ShowActingText));
        attackMgr.attackFinished = !attackMgr.attackFinished;
        isFighting = false;
        UnlockMenuInput();
    }

    // Esta funcion vuelve al menu despues de ACT.
    void FinishActingSequence()
    {
        HideSoul();
        StartCoroutine(ResizeBattleBox(new Vector2(11.5f, 3f), SetPostTurnText));
        actingMgr.isActing = false;
        isFighting = false;
        actingMgr.actingText.gameObject.SetActive(true);
        DialogueManager.instance.Talking(null);
        actingMgr.actObjects.SetActive(false);
        actingMgr.canAct = true;
        UnlockMenuInput();
    }

    // Esta funcion vuelve al menu despues de usar un item.
    void FinishItemSequence()
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
    }

    // Esta funcion vuelve a ensenar el texto principal.
    void ShowActingText()
    {
        actingMgr.actingText.gameObject.SetActive(true);
    }

    // Esta funcion prepara el texto despues del turno.
    void SetPostTurnText()
    {
        DialogueManager.instance.dialogueTxt = GetPostTurnText();
    }

    // Esta funcion cambia el tamano de la caja de batalla.
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
        if (onFinish != null)
        {
            onFinish();
        }

    }

    // Esta funcion termina el combate, guarda y vuelve al mapa.
    public void EndBattle()
    {
        bool canEndBattle;

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

        canEndBattle = playerVariables != null && saveManager != null;

        if (playerVariables == null)
        {
            Debug.LogError("playerVariables is null");
        }

        if (saveManager == null)
        {
            Debug.LogError("saveManager is null");
        }

        if (canEndBattle)
        {
            isFighting = false;

            playerVariables.playerData.health = 20;
            playerVariables.playerData.score += 100;

            if (string.IsNullOrWhiteSpace(currentFightName))
            {
                currentFightName = SceneManager.GetActiveScene().name;
            }

            if (!playerVariables.playerData.completedFights.Contains(currentFightName))
            {
                playerVariables.playerData.completedFights.Add(currentFightName);
            }

            json = JsonUtility.ToJson(playerVariables.playerData, true);
            saveManager.SaveGame(json);

            Debug.Log(json);
        
            DialogueManager.instance.enemyTxt = "";
            DialogueManager.instance.Talking(null);

            playerVariables.ClearSoulSprite();


            SceneManager.LoadScene("PruebaEntradaEnCombate");
        }

    }

    // Esta funcion limpia el sprite del corazon al salir de la escena.
    void OnDestroy()
    {
        if (playerVariables != null)
        {
            playerVariables.ClearSoulSprite();
        }
    }

}
