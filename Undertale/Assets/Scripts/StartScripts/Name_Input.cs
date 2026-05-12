using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Name_Input : MonoBehaviour
{
    // UI
    public Transform gridParent;
    public GameObject letterPrefab;
    public TMP_Text nameText;

    // Config
    public int columns = 7;
    public int maxLength = 15;

    // Estado
    private List<Button> buttons = new List<Button>();
    private int currentIndex = 0;
    public static string playerName = "";

    void Start()
    {
        GenerateGrid();
        UpdateSelection();
        UpdateText();
    }

    void Update()
    {
        HandleInput();
    }

    // ---------------- GRID ----------------
    void GenerateGrid()
    {
        buttons.Clear();

        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        for (int i = 0; i < letters.Length; i++)
        {
            GameObject obj = Instantiate(letterPrefab, gridParent);

            TMP_Text txt = obj.GetComponentInChildren<TMP_Text>();
            txt.text = letters[i].ToString();

            char letter = letters[i];

            Button btn = obj.GetComponent<Button>();
            btn.onClick.AddListener(() => AddLetter(letter));

            buttons.Add(btn);
        }
    }

    // ---------------- INPUT ----------------
    void HandleInput()
    {
        int newIndex = currentIndex;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            newIndex += 1;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            newIndex -= 1;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            newIndex -= 7;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            newIndex += 7;

        currentIndex = Mathf.Clamp(newIndex, 0, buttons.Count - 1);

        // seleccionar letra
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            AddLetter(GetLetter(currentIndex));
        }

        //  BACKSPACE SEGURO 
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            HandleDelete();
        }

        UpdateSelection();
    }

    // BLOQUEO EXTRA PARA EVITAR CONFLICTOS CON UI "Cancel"
    void HandleDelete()
    {
        // Evita que EventSystem/UI lo capture
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null)
        {
            // deselecciona UI para evitar "Back" accidental
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }

        DeleteLetter();
    }

    char GetLetter(int index)
    {
        return buttons[index].GetComponentInChildren<TMP_Text>().text[0];
    }

    // ---------------- SELECCIÓN ----------------
    void UpdateSelection()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            TMP_Text txt = buttons[i].GetComponentInChildren<TMP_Text>();
            txt.color = (i == currentIndex) ? Color.yellow : Color.white;
        }
    }

    // ---------------- INPUT LOGIC ----------------
    void AddLetter(char letter)
    {
        if (playerName.Length >= maxLength) return;

        playerName += letter;
        UpdateText();
    }

    public void DeleteLetter()
    {
        if (playerName.Length == 0) return;

        playerName = playerName.Substring(0, playerName.Length - 1);
        UpdateText();
    }

    void UpdateText()
    {
        nameText.text = playerName;
    }
}