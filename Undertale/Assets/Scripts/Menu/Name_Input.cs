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

    // Estado del menu de nombre.
    private List<Button> buttons = new List<Button>();
    private int currentIndex = 0;
    public static string playerName = "";

    // Esta funcion se ejecuta al empezar la escena y deja preparado el componente.
    void Start()
    {
        GenerateGrid();
        UpdateSelection();
        UpdateText();
    }

    // Esta funcion se ejecuta cada frame y revisa la entrada o el estado actual.
    void Update()
    {
        HandleInput();
        
    }

    // Esta funcion crea el teclado de letras.
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

    // Esta funcion lee el teclado para mover la seleccion y editar el nombre.
    void HandleInput()
    {
        int newIndex = currentIndex;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            newIndex += 1;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            newIndex -= 1;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            newIndex -= columns;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            newIndex += columns;
        }

        newIndex = Mathf.Clamp(newIndex, 0, buttons.Count - 1);
        currentIndex = newIndex;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            AddLetter(GetLetter(currentIndex));
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DeleteLetter();
        }

        UpdateSelection();
    }

    // Esta funcion devuelve la letra de una posicion del teclado.
    char GetLetter(int index)
    {
        return buttons[index].GetComponentInChildren<TMP_Text>().text[0];
    }

    // Esta funcion pinta de amarillo la letra seleccionada.
    void UpdateSelection()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            TMP_Text txt = buttons[i].GetComponentInChildren<TMP_Text>();

            txt.color = (i == currentIndex) ? Color.yellow : Color.white;
        }
    }

    // Esta funcion anade una letra al nombre.
    void AddLetter(char letter)
    {
        if (playerName.Length < maxLength)
        {
            playerName += letter;

            UpdateText();
        }
    }

    // Esta funcion elimina la ultima letra del nombre.
    public void DeleteLetter()
    {
        if (playerName.Length > 0)
        {
            playerName = playerName.Substring(0, playerName.Length - 1);

            UpdateText();
        }
    }

    // Esta funcion actualiza el texto visible del nombre.
    void UpdateText()
    {
        nameText.text = playerName;
    }
}
