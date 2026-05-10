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

    // Singleton del manager del combate, guarda la referencia para que cualquier script lo encuentre.
    void Awake()
    {
        battleInstance = this;
    }

    // Lanza la secuencia de FIGHT al pulsar el boton, bloquea el menu y oculta el texto principal.
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

    // Abre el submenu de ACT (acciones especiales segun el enemigo) al pulsar el boton.
    public void Acting()
    {
        if (actingMgr != null)
        {
            actingMgr.OpenMenu();
            menuInputLocked = true;
            audioMgr.Selecting();
        }
    }

    // Abre el menu de objetos del jugador para usar uno en el combate.
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

    // Suma una pulsacion de Mercy y, a las 10, perdona al enemigo y termina el combate sin matarle.
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

    // Inicializa todo el combate al cargar la escena, busca managers, configura enemigo, colliders y posiciona el alma.
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

    // Vuelve a localizar los 4 botones del menu (Fight, Act, Item, Mercy) por si la lista se quedo vacia.
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

    // Busca un GameObject por nombre y devuelve su componente Buttons, soltando un error si no lo encuentra.
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

    // Lee las flechas para mover la seleccion del menu, refresca la barra de vida y detecta si el enemigo ha muerto.
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

    // Si la seleccion del menu se sale por arriba o abajo, le da la vuelta para que vuelva al primero o al ultimo.
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

    // Apaga el sprite del alma y desactiva su movimiento, util cuando se cambia de fase del combate.
    void HideSoul()
    {
        if (soul != null)
        {
            soul.enabled = false;
        }

        SetSoulMovement(false);
    }

    // Devuelve true solo si no hay menus secundarios abiertos ni dialogos en curso, asi se puede mover la seleccion.
    bool CanReadMainMenuInput()
    {
        bool dialogueReady;

        dialogueReady = DialogueManager.instance == null || DialogueManager.instance.done;
        return !menuInputLocked && dialogueReady && !isFighting && !actingMgr.isActing && !ItemManager.instance.isMenu;
    }

    // Activa el flag que bloquea el menu, util mientras se escribe un dialogo o se reproduce una animacion.
    public void LockMenuInput()
    {
        menuInputLocked = true;
    }

    // Quita el bloqueo del menu para que el jugador pueda volver a moverse y elegir.
    public void UnlockMenuInput()
    {
        menuInputLocked = false;
    }

    // Coloca el alma en el centro de la caja de batalla y le activa el movimiento para esquivar balas.
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

    // Pone el alma al lado del boton seleccionado y le quita el movimiento, queda como un puntero.
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

    // Atajo publico para que los menus secundarios puedan ocultar el alma al cerrarse.
    public void HideSoulForMenu()
    {
        HideSoul();
    }

    // Cachea los componentes de movimiento y rigidbody del alma, evita buscarlos cada vez.
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

    // Apaga el sprite del Frisk del mapa mientras esta en combate, recuerda su estado para restaurarlo despues.
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

    // Restaura el sprite del Frisk del mapa al estado que tenia antes de empezar el combate.
    void ShowWorldPlayer()
    {
        if (worldPlayerHidden && worldPlayerSprite != null)
        {
            worldPlayerSprite.enabled = worldPlayerSpriteWasEnabled;
        }

        worldPlayerHidden = false;
    }

    // Activa o desactiva el script de movimiento del alma y le frena la velocidad si toca pararla.
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

    // Solo la primera vez que estan listos los managers, le pide al enemigo que les pase sus datos.
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

    // Saca una frase aleatoria de la lista del enemigo, o un texto generico si la lista esta vacia.
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

    // Decide que texto sale tras el turno: especial primero, luego mensaje de spare, luego flavor text aleatorio.
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

    // Apunta un texto puntual para enseñar al final del turno (lo usa Papyrus para anunciar el alma azul).
    public void SetNextPostTurnText(string text)
    {
        nextPostTurnText = text;
    }

    // Deja marcado que en el siguiente menu hay que cambiar el alma a azul, lo aplica al volver al menu.
    public void PrepareSoulBlueForMenu()
    {
        blueSoulPending = true;
    }

    // Devuelve true si el alma ya esta convertida en azul, lo consultan los ataques que dependen de ello.
    public bool IsSoulBlue()
    {
        return soulIsBlue;
    }

    // Si hay cambio a azul pendiente, lo aplica y limpia el flag para no repetirlo.
    void ApplyPendingSoulBlue()
    {
        if (blueSoulPending)
        {
            blueSoulPending = false;
            SetSoulBlue();
        }
    }

    // Si el alma ya es azul, vuelve a aplicar el sprite azul, asi no se vuelve roja al reaparecer.
    void ApplySoulBlueIfNeeded()
    {
        if (soulIsBlue)
        {
            SetSoulBlue();
        }
    }

    // Filtra los strings vacios de la lista y devuelve uno al azar de los que quedan.
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

    // Devuelve el nombre del enemigo (o "El enemigo" como fallback) para usarlo en mensajes generados.
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

    // Marca como seleccionado un boton del menu y mueve el alma a su posicion de puntero.
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

    // Quita la marca de seleccionado a un boton concreto.
    void Deselecting(int deselectionInt)
    {
        if (buttons != null && buttons.Count > deselectionInt && buttons[deselectionInt] != null)
        {
            buttons[deselectionInt].DeselectButton();
        }
    }

    // Recorre los 4 botones, deja seleccionado el de selectionInt y deselecciona el resto.
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


    // Lanza la accion del boton actual segun el indice (0=Fight, 1=Act, 2=Item, 3=Mercy).
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

    // Hace el minijuego de la barra de ataque del player y, al acabar, lanza el ataque del enemigo.
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

    // Cierra el menu de ACT, devuelve el alma a la caja y arranca el ataque del enemigo en respuesta.
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

    // Cierra el menu de ITEMS, devuelve el alma a la caja y dispara el ataque del enemigo.
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

    // Callback que arranca el turno del enemigo cuando termina el dialogo del intento de Mercy.
    void FinishMercyDialogue()
    {
        StartCoroutine(MercySequence());
    }

    // Limpia el texto del Mercy fallido, devuelve el alma a la caja y lanza el ataque del enemigo.
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

    // Vacia el texto principal y el de acting, asi no se ven sobras del turno anterior cuando empieza el ataque.
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

    // Guarda una copia del sprite rojo original del alma para poder restaurarlo si fue cambiado a azul.
    void SaveOriginalSoulSprite()
    {
        if (soul != null && redSoulSprite == null)
        {
            redSoulSprite = soul.sprite;
        }
    }

    // Tras un FIGHT, oculta el alma, agranda la caja al tamaño de menu y bloquea inputs hasta el post-turno.
    void FinishAttackSequence()
    {
        HideSoul();
        ApplyPendingSoulBlue();
        StartCoroutine(ResizeBattleBox(new Vector2(11.5f, 3), FinishAttackResize));
        attackMgr.attackFinished = !attackMgr.attackFinished;
        isFighting = false;
        LockMenuInput();
    }

    // Despues de un Mercy fallido, devuelve la caja al tamaño de menu y muestra el texto post-turno.
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

    // Despues de un ACT, devuelve la caja al menu y deja todo listo para el siguiente turno del jugador.
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

    // Despues de usar un objeto, devuelve la caja al menu y resetea las flags del menu de items.
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

    // Reactiva el GameObject del texto principal del combate, que se apaga durante el FIGHT.
    void ShowActingText()
    {
        actingMgr.actingText.gameObject.SetActive(true);
    }

    // Callback al terminar el resize tras FIGHT, vuelve a mostrar el texto y arranca el dialogo post-turno.
    void FinishAttackResize()
    {
        ShowActingText();
        StartPostTurnText();
    }

    // Pide al DialogueManager el texto post-turno y, cuando termina, desbloquea el menu del jugador.
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

    // Lanza el dialogo post-turno, con callback distinto si hay texto especial pendiente (caso de Papyrus).
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

    // Tras el aviso especial (alma azul de Papyrus), lo aplica y enseña el dialogo normal del turno.
    void FinishSpecialPostTurnText()
    {
        ApplyPendingSoulBlue();
        ShowNormalPostTurnText();
    }

    // Escribe directamente el texto normal del post-turno en ambos cuadros, sin animacion de mecanografia.
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

    // Limpia el texto en pantalla justo antes de elegir nueva opcion, para que no se acumule el mensaje azul.
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

    // Mete el texto post-turno en el DialogueManager, callback usado tras secuencias de Acting.
    void SetPostTurnText()
    {
        DialogueManager.instance.dialogueTxt = GetPostTurnText();
    }

    // Permite a los ataques cambiar el tamaño de la caja al vuelo (lo usan PapyrusColumns, BoneStorm, etc).
    public void ChangeBattleBoxSize(Vector2 size)
    {
        StartCoroutine(ResizeBattleBox(size, null));
    }

    // Cambia el sprite del alma al azul de Papyrus y avisa a PlayerVars del cambio para que persista.
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

    // Localiza el contenedor "Colliders" hijo del battleBox y guarda los 4 BoxColliders que limitan la caja.
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

    // Aplica el nuevo tamaño al sprite blanco de la caja y reposiciona los colliders alrededor.
    void SetBattleBoxSize(Vector2 size)
    {
        if (battleBox != null)
        {
            battleBox.size = size;
            ResizeBattleColliders(size);
        }
    }

    // Coloca los 4 colliders (top, bottom, left, right) justo en los bordes de la caja del tamaño indicado.
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

    // Coloca un BoxCollider concreto en la posicion y tamaño que toca, dejando la rotacion en cero.
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

    // Anima el tamaño de la caja desde el actual hasta targetSize a velocidad constante y llama al callback al acabar.
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

    // Cierra el combate: cura al jugador, da puntos, marca esta pelea como completada, guarda y vuelve al mapa.
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


            SceneManager.LoadScene("PruebaEntradaEnCombate");
        }

    }

    // Al destruirse el manager (cambio de escena) restaura el sprite del Frisk del mapa y limpia el alma.
    void OnDestroy()
    {
        ShowWorldPlayer();

        if (playerVariables != null)
        {
            playerVariables.ClearSoulSprite();
        }
    }

}
