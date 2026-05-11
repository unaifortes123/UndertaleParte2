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
    public Sprite blueSoulSprite;
    public SpriteRenderer battleBox;
    public Vector2 attackBattleBoxSize = new Vector2(3f, 3f);
    public Transform battleColliders;
    public List<Buttons> buttons;
    int maxSelectionInt;
    int minSelectionInt;
    public int selectionInt;
    public GameObject mercyMenu;
    public GameObject damageSprite;
    const float SIZE_INCREASE = 18f;
    const float BATTLE_COLLIDER_THICKNESS = 0.25f;
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
    private SpriteRenderer worldPlayerSprite;
    private bool worldPlayerSpriteWasEnabled;
    private bool worldPlayerHidden;
    private Sprite redSoulSprite;
    private bool blueSoulPending;
    private bool soulIsBlue;
    private BoxCollider2D topBattleCollider;
    private BoxCollider2D bottomBattleCollider;
    private BoxCollider2D leftBattleCollider;
    private BoxCollider2D rightBattleCollider;
    private const int SOUL_SORTING_ORDER = 50;
    private const int MERCY_PRESSES_TO_END_BATTLE = 10;
    private int mercyPressCount;
    private string nextPostTurnText;

    void Awake()
    {
        battleInstance = this;
    }

    // se llama al pulsar FIGHT, arranca la secuencia del ataque
    public void Attacking()
    {
        if (!isFighting)
        {
            menuInputLocked = true;
            actingMgr.actingText.gameObject.SetActive(false); // oculta el texto principal mientras pega
            AudioManager.instance.Selecting();
            StartCoroutine(AttackSequence());
        }

    }

    // abre el submenu de ACT (las acciones especiales tipo "Check", "Talk", etc.)
    public void Acting()
    {
        if (actingMgr != null)
        {
            actingMgr.OpenMenu();
            menuInputLocked = true;
            audioMgr.Selecting();
        }
    }

    // abre el menu de objetos para que el player use alguno
    public void Item()
    {
        if (ItemManager.instance != null)
        {
            isFighting = false;
            menuInputLocked = true;
            ItemManager.instance.OpenMenu();
            audioMgr.Selecting();
        }
    }

    // suma un Mercy, y a las 10 pulsaciones perdona al enemigo y termina el combate (asi no hace falta matarle)
    public void Mercy()
    {
        bool canFinishByMercy;

        canFinishByMercy = false;

        if (!battleEnding)
        {
            mercyPressCount += 1;
            canFinishByMercy = mercyPressCount >= MERCY_PRESSES_TO_END_BATTLE;
            audioMgr.Selecting();

            if (canFinishByMercy == true)
            {
                DialogueManager.instance.dialogueTxt = "*You spared the enemy.";
                DialogueManager.instance.enemyTxt = "";
                DialogueManager.instance.Talking(null);
                EndBattle();
            }
            else
            {
                menuInputLocked = true;
                DialogueManager.instance.dialogueTxt = "*You tried to spare the enemy. (" + mercyPressCount + "/" + MERCY_PRESSES_TO_END_BATTLE + ")";
                DialogueManager.instance.enemyTxt = "";
                DialogueManager.instance.Talking(FinishMercyDialogue);
            }
        }
    }

    // setup inicial del combate: pilla managers, configura enemigo, los colliders de la caja, etc.
    void Start()
    {
        selectionInt = 0;
        maxSelectionInt = 3;
        minSelectionInt = 0;
        mercyPressCount = 0;

        attackMgr = AttackManager.instance;
        enemyStats = FindObjectOfType<EnemyVars>();
        playerVariables = PlayerVars.instance;
        saveManager = SaveManager.instance;
        currentFightName = SceneManager.GetActiveScene().name;
        ConfigureEnemyCombat();
        ConfigureSoulReferences();
        SaveOriginalSoulSprite();
        ConfigureBattleColliders();

        if (battleBox != null)
        {
            SetBattleBoxSize(battleBox.size);
        }

        HideWorldPlayer();

        Selection();
    }

    // por si la lista de botones esta vacia, los busca por nombre en la escena (Fight, Act, Item, Mercy)
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

        if (fightButtons != null && actButtons != null && itemButtons != null && mercyButtons != null) // Si se han encontrado todos los botones, se guardan en la lista.
        {
            buttons = new List<Buttons>();

            buttons.Add(fightButtons);
            buttons.Add(actButtons);
            buttons.Add(itemButtons);
            buttons.Add(mercyButtons);

        }
    }

    // helper que busca un GameObject por nombre y devuelve su script Buttons, o un error si no lo pilla
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

    // gestiona el menu (flechas, enter), refresca la vida del player y mira si el enemigo ha muerto
    void Update()
    {
        if (playerVariables == null)
        {
            playerVariables = PlayerVars.instance;
        }

        if (!worldPlayerHidden)
        {
            HideWorldPlayer();
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
                    SetBattleBoxSize(new Vector2(11.5f, 3));
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
            PapyrusCombat papyrus;

            battleEnding = true;
            papyrus = enemyStats.GetComponent<PapyrusCombat>();

            if (papyrus != null)
            {
                papyrus.HandleDefeat();
            }
            else
            {
                EndBattle();
            }
        }
            
    }

    // si el indice del menu se sale (por arriba o por abajo), le da la vuelta para que ciclee
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

    // oculta el alma y le quita el movimiento, lo usa al cambiar de fase
    void HideSoul()
    {
        if (soul != null)
        {
            soul.enabled = false;
        }

        SetSoulMovement(false);
    }

    // solo deja mover la seleccion si no hay menu secundario abierto ni dialogo escribiendose
    bool CanReadMainMenuInput()
    {
        bool dialogueReady;

        dialogueReady = DialogueManager.instance == null || DialogueManager.instance.done;
        return !menuInputLocked && dialogueReady && !isFighting && !actingMgr.isActing && !ItemManager.instance.isMenu;
    }

    // bloquea el menu mientras hay un dialogo o animacion en marcha
    public void LockMenuInput()
    {
        menuInputLocked = true;
    }

    // lo desbloquea cuando ya puede volver a elegir
    public void UnlockMenuInput()
    {
        menuInputLocked = false;
    }

    // mete el alma en el centro de la caja y activa su movimiento para que esquive las balas
    void ShowSoulInBattleBox()
    {
        if (soul != null && battleBox != null)
        {
            ApplyPendingSoulBlue();
            ApplySoulBlueIfNeeded();
            soul.transform.position = battleBox.transform.position;
            soul.enabled = true;
            SetSoulMovement(true);
        }
    }

    // pone el alma al lado del boton seleccionado pero sin moverse, hace de puntero del menu
    public void ShowSoulInMenu(Vector3 position)
    {
        if (soul != null)
        {
            ApplyPendingSoulBlue();
            ApplySoulBlueIfNeeded();
            soul.transform.position = position;
            soul.enabled = true;
            SetSoulMovement(false);
        }
    }

    // atajo publico para que otros menus puedan ocultar el alma al cerrarse
    public void HideSoulForMenu()
    {
        HideSoul();
    }

    // cachea los componentes del alma para no buscarlos cada vez con GetComponent
    void ConfigureSoulReferences()
    {
        if (soul != null)
        {
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
    }

    // oculta el Frisk del mapa durante el combate y guarda el estado para devolverlo despues
    void HideWorldPlayer()
    {
        SpriteRenderer playerSprite;

        if (playerVariables != null)
        {
            playerSprite = playerVariables.GetComponent<SpriteRenderer>();

            if (playerSprite != null && playerSprite != soul)
            {
                worldPlayerSprite = playerSprite;
                worldPlayerSpriteWasEnabled = playerSprite.enabled;
                worldPlayerSprite.enabled = false;
                worldPlayerHidden = true;
            }
        }
    }

    // devuelve el sprite del Frisk al estado que tenia antes del combate
    void ShowWorldPlayer()
    {
        if (worldPlayerHidden && worldPlayerSprite != null)
        {
            worldPlayerSprite.enabled = worldPlayerSpriteWasEnabled;
        }

        worldPlayerHidden = false;
    }

    // activa o desactiva el script de movimiento del alma. Si la para, le frena tambien la velocidad
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

    // solo la primera vez que todos los managers estan listos, pide al enemigo que les pase sus datos
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

    // saca una frase random de la lista del enemigo, si la lista esta vacia devuelve un texto generico
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

    // decide el texto post-turno: prioridad 1 mensaje especial (alma azul), 2 spare si toca, 3 flavor text random
    public string GetPostTurnText()
    {
        string dialogue;

        dialogue = "";

        if (!string.IsNullOrWhiteSpace(nextPostTurnText))
        {
            dialogue = nextPostTurnText;
            nextPostTurnText = "";
        }

        if (string.IsNullOrWhiteSpace(dialogue) && actingMgr != null && actingMgr.totalMercy >= actingMgr.totalMercyMax && !string.IsNullOrWhiteSpace(actingMgr.spareMessage))
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

    // deja apuntado un texto especial para mostrar al acabar el turno (lo usa Papyrus para anunciar el alma azul)
    public void SetNextPostTurnText(string text)
    {
        nextPostTurnText = text;
    }

    // marca que en el proximo menu hay que cambiar el alma a azul (lo aplica cuando vuelva al menu)
    public void PrepareSoulBlueForMenu()
    {
        blueSoulPending = true;
    }

    public bool IsSoulBlue()
    {
        return soulIsBlue;
    }

    // si hay un cambio a azul pendiente lo aplica y baja el flag para no repetirlo
    void ApplyPendingSoulBlue()
    {
        if (blueSoulPending)
        {
            blueSoulPending = false;
            SetSoulBlue();
        }
    }

    // si el alma ya era azul, le vuelve a poner el sprite por si Unity la reseteo al rojo
    void ApplySoulBlueIfNeeded()
    {
        if (soulIsBlue)
        {
            SetSoulBlue();
        }
    }

    // filtra los strings vacios y devuelve uno random de los que queden
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

    // devuelve el nombre del enemigo, o "El enemigo" si no esta puesto, para meterlo en frases generadas
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

    // marca un boton como seleccionado y mueve el alma a su posicion (la usa como puntero)
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

    void Deselecting(int deselectionInt)
    {
        if (buttons != null && buttons.Count > deselectionInt && buttons[deselectionInt] != null)
        {
            buttons[deselectionInt].DeselectButton();
        }
    }

    // recorre los 4 botones, deja seleccionado el actual y quita la seleccion al resto
    void Selection()
    {
        int i;

        if (buttons != null)
        {
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
    }


    // lanza la accion del boton actual: 0 Fight, 1 Act, 2 Item, 3 Mercy
    void Selected()
    {
        ClearSpecialPostTurnText();

        if (selectionInt == 0)
        {
            Attacking();
        }
        if (selectionInt == 1)
        {
            Acting();
        }
        if (selectionInt == 2)
        {
            Item();
        }
        if (selectionInt == 3)
        {
            Mercy();
        }
    }

    // secuencia del FIGHT: minijuego de la barra del player, y al acabar lanza el ataque del enemigo
    IEnumerator AttackSequence()
    {
        isFighting = true;

        HideSoul();
        isFinished = FinishAttackSequence;

        attackingSys.StartAttacking(playerVariables.atkValue);

        yield return new WaitForSeconds(attackingSys.maxTime);

        ShowSoulInBattleBox();

        StartCoroutine(ResizeBattleBox(attackBattleBoxSize, null));
        actingMgr.actingText.gameObject.SetActive(false);

        attackMgr.StartAttack(attackMgr.GetAttack(), isFinished);
    }

    // cierra el menu de ACT, devuelve el alma a la caja y arranca el ataque del enemigo
    public IEnumerator ActingSequence()
    {
        HideSoul();
        isFinished = FinishActingSequence;

        yield return new WaitForSeconds(1);

        ShowSoulInBattleBox();

        StartCoroutine(ResizeBattleBox(attackBattleBoxSize, SetPostTurnText));
        actingMgr.actingText.gameObject.SetActive(false);
        actingMgr.isActing = false;
        isFighting = true;
        actingMgr.time = 0;

        attackMgr.StartAttack(attackMgr.GetAttack(), isFinished);
    }

    // cierra el menu de ITEMS, devuelve el alma a la caja y dispara el ataque del enemigo
    public IEnumerator ItemSequence()
    {
        HideSoul();
        isFinished = FinishItemSequence;

        yield return new WaitForSeconds(1);

        ShowSoulInBattleBox();

        StartCoroutine(ResizeBattleBox(attackBattleBoxSize, null));
        ItemManager.instance.itemObjects.SetActive(false);
        isFighting = true;
        ItemManager.instance.isMenu = false;
        ItemManager.instance.useText.text = "";
        actingMgr.time = 0;

        attackMgr.StartAttack(attackMgr.GetAttack(), isFinished);
    }

    // callback que arranca el turno del enemigo cuando acaba el dialogo del Mercy fallido
    void FinishMercyDialogue()
    {
        StartCoroutine(MercySequence());
    }

    // tras intentar Mercy sin conseguirlo, limpia el texto, devuelve el alma a la caja y el enemigo ataca
    IEnumerator MercySequence()
    {
        HideSoul();
        isFinished = FinishMercySequence;

        yield return new WaitForSeconds(1);

        ClearMainDialogueText();
        ShowSoulInBattleBox();

        StartCoroutine(ResizeBattleBox(attackBattleBoxSize, null));
        isFighting = true;
        actingMgr.time = 0;

        attackMgr.StartAttack(attackMgr.GetAttack(), isFinished);
    }

    // vacia ambos textos para que no se queden sobras del turno anterior al empezar el ataque
    void ClearMainDialogueText()
    {
        DialogueManager.instance.dialogueTxt = "";

        if (DialogueManager.instance.text != null)
        {
            DialogueManager.instance.text.text = "";
        }

        if (actingMgr != null && actingMgr.actingText != null)
        {
            actingMgr.actingText.text = "";
            actingMgr.actingText.gameObject.SetActive(false);
        }
    }

    // guarda el sprite rojo original del alma para poder volver a el si la convierte en azul y quiere rebajar
    void SaveOriginalSoulSprite()
    {
        if (soul != null && redSoulSprite == null)
        {
            redSoulSprite = soul.sprite;
        }
    }

    // al acabar el FIGHT: oculta el alma, vuelve la caja al tamaño del menu y bloquea inputs hasta el post-turno
    void FinishAttackSequence()
    {
        HideSoul();
        ApplyPendingSoulBlue();
        StartCoroutine(ResizeBattleBox(new Vector2(11.5f, 3), FinishAttackResize));
        attackMgr.attackFinished = !attackMgr.attackFinished;
        isFighting = false;
        LockMenuInput();
    }

    // tras un Mercy fallido, devuelve la caja al tamaño del menu y enseña el texto post-turno
    void FinishMercySequence()
    {
        HideSoul();
        ApplyPendingSoulBlue();
        StartCoroutine(ResizeBattleBox(new Vector2(11.5f, 3f), null));
        actingMgr.isActing = false;
        isFighting = false;
        actingMgr.actingText.gameObject.SetActive(true);
        StartPostTurnDialogue();
        actingMgr.actObjects.SetActive(false);
        actingMgr.canAct = true;
        UnlockMenuInput();
    }

    // tras un ACT, devuelve la caja al menu y deja todo listo para el siguiente turno del player
    void FinishActingSequence()
    {
        HideSoul();
        ApplyPendingSoulBlue();
        StartCoroutine(ResizeBattleBox(new Vector2(11.5f, 3f), null));
        actingMgr.isActing = false;
        isFighting = false;
        actingMgr.actingText.gameObject.SetActive(true);
        StartPostTurnDialogue();
        actingMgr.actObjects.SetActive(false);
        actingMgr.canAct = true;
        UnlockMenuInput();
    }

    // tras usar un item, devuelve la caja al menu y resetea las flags del menu de items
    void FinishItemSequence()
    {
        ItemManager.instance.time = 0;
        HideSoul();
        ApplyPendingSoulBlue();
        StartCoroutine(ResizeBattleBox(new Vector2(11.5f, 3f), null));
        actingMgr.isActing = false;
        isFighting = false;
        actingMgr.actingText.gameObject.SetActive(true);
        StartPostTurnDialogue();
        actingMgr.actObjects.SetActive(false);
        ItemManager.instance.itemObjects.SetActive(false);
        actingMgr.canAct = true;
        UnlockMenuInput();
    }

    // reactiva el texto principal que se apaga durante el FIGHT
    void ShowActingText()
    {
        actingMgr.actingText.gameObject.SetActive(true);
    }

    // callback cuando la caja termina de redimensionarse tras el FIGHT
    void FinishAttackResize()
    {
        ShowActingText();
        StartPostTurnText();
    }

    // pide el texto post-turno al DialogueManager y al acabar desbloquea el menu
    void StartPostTurnText()
    {
        if (DialogueManager.instance != null)
        {
            StartPostTurnDialogue();
            UnlockMenuInput();
        }

        if (DialogueManager.instance == null)
        {
            UnlockMenuInput();
        }
    }

    // arranca el dialogo post-turno. Si hay un texto especial pendiente (Papyrus alma azul) usa otro callback
    void StartPostTurnDialogue()
    {
        bool hasSpecialText;

        hasSpecialText = !string.IsNullOrWhiteSpace(nextPostTurnText);

        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.dialogueTxt = GetPostTurnText();
            DialogueManager.instance.enemyTxt = "";

            if (hasSpecialText)
            {
                DialogueManager.instance.Talking(FinishSpecialPostTurnText);
            }
            else
            {
                DialogueManager.instance.Talking(null);
            }
        }
    }

    // tras el aviso especial del alma azul, lo aplica y muestra el dialogo normal del turno
    void FinishSpecialPostTurnText()
    {
        ApplyPendingSoulBlue();
        ShowNormalPostTurnText();
    }

    // mete el texto normal del post-turno directamente, sin escritura tipo maquina, en ambos cuadros
    void ShowNormalPostTurnText()
    {
        string normalText;

        normalText = GetPostTurnText();

        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.dialogueTxt = normalText;
            DialogueManager.instance.enemyTxt = "";

            if (DialogueManager.instance.text != null)
            {
                DialogueManager.instance.text.text = normalText;
            }
        }

        if (actingMgr != null && actingMgr.actingText != null)
        {
            actingMgr.actingText.gameObject.SetActive(true);
            actingMgr.actingText.text = normalText;
        }
    }

    // limpia el texto antes de elegir otra opcion para que no se quede pegado el mensaje del alma azul
    void ClearSpecialPostTurnText()
    {
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.dialogueTxt = "";

            if (DialogueManager.instance.text != null)
            {
                DialogueManager.instance.text.text = "";
            }
        }

        if (actingMgr != null && actingMgr.actingText != null)
        {
            actingMgr.actingText.text = "";
        }
    }

    // callback que se usa tras un ACT para meter el texto post-turno en el DialogueManager
    void SetPostTurnText()
    {
        DialogueManager.instance.dialogueTxt = GetPostTurnText();
    }

    // metodo publico para que los ataques cambien el tamaño de la caja al vuelo (BoneStorm, MikuStars, etc)
    public void ChangeBattleBoxSize(Vector2 size)
    {
        StartCoroutine(ResizeBattleBox(size, null));
    }

    // pone el alma azul (la de Papyrus) y avisa a PlayerVars para que se guarde el cambio
    public void SetSoulBlue()
    {
        Sprite selectedSprite;

        soulIsBlue = true;
        ConfigureSoulReferences();
        SaveOriginalSoulSprite();
        selectedSprite = blueSoulSprite;

        if (soul != null && selectedSprite != null)
        {
            soul.sprite = selectedSprite;
            soul.color = Color.white;

            if (playerVariables != null)
            {
                playerVariables.soulOriginal = Color.white;
                playerVariables.SetSoulSprite(soul);
            }
        }
    }

    // busca el hijo "Colliders" del battleBox y guarda los 4 BoxColliders que limitan la caja
    void ConfigureBattleColliders()
    {
        BoxCollider2D[] colliders;
        Transform battleObjects;

        if (battleColliders == null && battleBox != null)
        {
            battleObjects = battleBox.transform.parent;

            if (battleObjects != null)
            {
                battleColliders = battleObjects.Find("Colliders");
            }
        }

        if (battleColliders != null)
        {
            colliders = battleColliders.GetComponentsInChildren<BoxCollider2D>();

            if (colliders.Length >= 4)
            {
                bottomBattleCollider = colliders[0];
                rightBattleCollider = colliders[1];
                leftBattleCollider = colliders[2];
                topBattleCollider = colliders[3];
            }
        }
    }

    // aplica el tamaño nuevo al sprite blanco de la caja y reajusta los colliders alrededor
    void SetBattleBoxSize(Vector2 size)
    {
        if (battleBox != null)
        {
            battleBox.size = size;
            ResizeBattleColliders(size);
        }
    }

    // recoloca los 4 colliders (top, bottom, left, right) en los bordes para el tamaño dado
    void ResizeBattleColliders(Vector2 size)
    {
        if (topBattleCollider == null || bottomBattleCollider == null || leftBattleCollider == null || rightBattleCollider == null)
        {
            ConfigureBattleColliders();
        }

        if (topBattleCollider != null && bottomBattleCollider != null && leftBattleCollider != null && rightBattleCollider != null)
        {
            SetBattleCollider(bottomBattleCollider, new Vector2(0, -size.y / 2 - BATTLE_COLLIDER_THICKNESS / 2), new Vector2(size.x, BATTLE_COLLIDER_THICKNESS));
            SetBattleCollider(topBattleCollider, new Vector2(0, size.y / 2 + BATTLE_COLLIDER_THICKNESS / 2), new Vector2(size.x, BATTLE_COLLIDER_THICKNESS));
            SetBattleCollider(leftBattleCollider, new Vector2(-size.x / 2 - BATTLE_COLLIDER_THICKNESS / 2, 0), new Vector2(BATTLE_COLLIDER_THICKNESS, size.y));
            SetBattleCollider(rightBattleCollider, new Vector2(size.x / 2 + BATTLE_COLLIDER_THICKNESS / 2, 0), new Vector2(BATTLE_COLLIDER_THICKNESS, size.y));
        }
    }

    // helper que mueve un BoxCollider a una posicion y tamaño, sin rotacion
    void SetBattleCollider(BoxCollider2D boxCollider, Vector2 position, Vector2 size)
    {
        if (boxCollider != null)
        {
            boxCollider.transform.localRotation = Quaternion.identity;
            boxCollider.transform.localPosition = position;
            boxCollider.offset = Vector2.zero;
            boxCollider.size = size;
        }
    }

    // anima el redimensionado de la caja de tamaño actual a targetSize a velocidad constante
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

            SetBattleBoxSize(size);
            yield return null;
        }
        if (onFinish != null)
        {
            onFinish();
        }

    }

    // termina el combate: cura al player, suma 100 puntos, marca este combate como pasado, guarda y vuelve al mapa
    public void EndBattle()
    {
        bool canEndBattle;

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

            DialogueManager.instance.enemyTxt = "";
            DialogueManager.instance.Talking(null);

            playerVariables.ClearSoulSprite();


            SceneManager.LoadScene("MainLevel");
        }

    }

    // al destruirse (cambio de escena) devuelve el Frisk del mapa a la vista y limpia el alma
    void OnDestroy()
    {
        ShowWorldPlayer();

        if (playerVariables != null)
        {
            playerVariables.ClearSoulSprite();
        }
    }

}
