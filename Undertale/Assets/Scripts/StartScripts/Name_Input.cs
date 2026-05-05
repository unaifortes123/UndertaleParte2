using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class Name_Input : MonoBehaviour
{

    //Variable tipo transform para instanciar las letras
    public Transform gridParent;
    //Prefab del boton para cada letra
    public GameObject letterPrefab;
    //Texto donde se mostrará el name que escriba el player
    public TMP_Text nameText;

    //num d columnas, aunque despues podremos modificarlas
    public int columns = 7;
    //longitud max del nombre del player
    public int maxLength = 15;

    //Lista para guardar todos los botones d las letras q generemos
    private List<Button> buttons = new List<Button>();
    //Index de la letra q se selecciona actualmente
    private int currentIndex = 0;
    //String donde estará el nombre que escribe el jugador
    public static string playerName = "";
    internal List<GameObject> letters;

    // Start is called before the first frame update
    void Start()
    {
        //Genera las letras en pantalla
        GenerateGrid();
        //Actualiza la seleccion
        UpdateSelection();


    }

    // Update is called once per frame
    void Update()
    {
        //Detecta el input del teclado
        HandleInput();

    }
    void GenerateGrid()
    {
        //Limpiamos lista
        buttons.Clear();

        //String con todas las letras
        string letters =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        for (int i = 0; i < letters.Length; i++)
        {
            //Bucle para recorrer cada letra del string, crearemos un boton en el grid
            GameObject obj = Instantiate(letterPrefab, gridParent);
            //busca texto dentro del boton y asigna letra
            TMP_Text txt = obj.GetComponentInChildren<TMP_Text>();
            txt.text = letters[i].ToString();
            //Guarda la letra en una variable
            char letter = letters[i];
            //Obtiene el component button del prefab
            Button btn = obj.GetComponent<Button>();
            //cuando se haga click anadira la letra al nombre.

            //guardamos boton
            buttons.Add(btn);
        }
    }
    //Control del teclado
    void HandleInput()
    {
        int newIndex = currentIndex;

        // Movimiento horizontal
        if (Input.GetKeyDown(KeyCode.RightArrow))
            newIndex += 1;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            newIndex -= 1;

        // Movimiento vertical
        if (Input.GetKeyDown(KeyCode.UpArrow))
            newIndex -= columns;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            newIndex += columns;

        // Clamp seguro
        newIndex = Mathf.Clamp(newIndex, 0, buttons.Count - 1);

        currentIndex = newIndex;

        // Seleccionar letra
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            AddLetter(GetLetter(currentIndex));
        }
        char GetLetter(int index)
        {
            return buttons[index].GetComponentInChildren<TMP_Text>().text[0];
        }
        // Borrar
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DeleteLetter();
        }
        void HandleInput()
        {
            int newIndex = currentIndex;

            // Movimiento horizontal
            if (Input.GetKeyDown(KeyCode.RightArrow))
                newIndex += 1;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                newIndex -= 1;

            // Movimiento vertical
            if (Input.GetKeyDown(KeyCode.UpArrow))
                newIndex -= columns;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                newIndex += columns;

            // Clamp seguro
            newIndex = Mathf.Clamp(newIndex, 0, buttons.Count - 1);

            currentIndex = newIndex;

            // Seleccionar letra
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                AddLetter(GetLetter(currentIndex));
            }

            // Borrar
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                DeleteLetter();
            }

            UpdateSelection();
        }
        UpdateSelection();
    }
    //marca la letra q se selecciona
    void UpdateSelection()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            TMP_Text txt = buttons[i].GetComponentInChildren<TMP_Text>();

            if (i == currentIndex)
            {
                txt.color = Color.yellow;
            }
            else
            {
                txt.color = Color.white;
            }
        }
    }
    //añade una letra al nombre del player
    void AddLetter(char letter)
    {
        //si llega al maximo no se añaden mas
        if (playerName.Length == maxLength) return;
        //añade la letra al string
        playerName += letter;
        //Updatea el texto en pantalla
        nameText.text = playerName;
    }
    //delete d la ultima letra del nombre
    void DeleteLetter()
    {
        if (playerName.Length == 0) return;
        playerName = playerName.Substring(0, playerName.Length - 1);
        nameText.text = playerName;
    }
    
    
  
}
