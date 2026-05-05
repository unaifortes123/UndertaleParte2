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

            Button btn = obj.GetComponent<Button>();

            char letter = letters[i];
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
            newIndex -= columns;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            newIndex += columns;

        newIndex = Mathf.Clamp(newIndex, 0, buttons.Count - 1);
        currentIndex = newIndex;

        // seleccionar letra
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            AddLetter(GetLetter(currentIndex));
        }

        // borrar
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DeleteLetter();
        }

        UpdateSelection();
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
    //donde añadiremos las letras
    void AddLetter(char letter)
    {
        if (playerName.Length >= maxLength) return;

        playerName += letter;
        UpdateText();
    }

    //Para eliminar letras
    public void DeleteLetter()
    {
        if (playerName.Length == 0) return;

        playerName = playerName.Substring(0, playerName.Length - 1);
        UpdateText();
    }

    //actualizamos el texto para q se aplique
    void UpdateText()
    {
        nameText.text = playerName;
    }
}
