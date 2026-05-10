using UnityEngine;
using UnityEngine.UI;

public class UIBattle : MonoBehaviour
{
    public GameObject ButtonFight;
    public GameObject ButtonAct;
    public GameObject ButtonItem;
    public GameObject ButtonMercy;

    public BattleManager battle;

    private GameObject[] buttons;
    private int selectedIndex = 0;

    // Esta funcion se ejecuta al empezar la escena y deja preparado el componente.
    void Start()
    {
        bool canStart;

        canStart = true;

        if (!ButtonFight || !ButtonAct || !ButtonItem || !ButtonMercy)
        {
            Debug.LogError("Faltan botones asignados en el Inspector.");
            canStart = false;
        }

        if (!battle)
        {
            Debug.LogError("Falta BattleManager asignado.");
            canStart = false;
        }

        if (canStart == true)
        {
            buttons = new GameObject[]
            {
                ButtonFight,
                ButtonAct,
                ButtonItem,
                ButtonMercy
            };

            UpdateSelection();
        }
    }

    // Esta funcion se ejecuta cada frame y revisa la entrada o el estado actual.
    void Update()
    {
        HandleInput();
    }

    // Esta funcion lee las teclas del jugador.
    void HandleInput()
    {
        if (battle && buttons != null)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedIndex--;

                if (selectedIndex < 0)
                {
                    selectedIndex = buttons.Length - 1;
                }

                UpdateSelection();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedIndex++;

                if (selectedIndex >= buttons.Length)
                {
                    selectedIndex = 0;
                }

                UpdateSelection();
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Tab))
            {
                buttons[selectedIndex].GetComponent<Button>().onClick.Invoke();
            }
        }
    }

    // Esta funcion actualiza la seleccion visual actual.
    void UpdateSelection()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == selectedIndex)
            {
                buttons[i].transform.localScale = Vector3.one * 1.2f; // seleccionado
            }
            else
            {
                buttons[i].transform.localScale = Vector3.one; // normal
            }
        }
    }

   

    // Esta funcion activa o desactiva los botones del combate.
    public void EnableButtons(bool active)
    {
        ButtonFight.SetActive(active);
        ButtonAct.SetActive(active);
        ButtonItem.SetActive(active);
        ButtonMercy.SetActive(active);
    }

    // Esta funcion muestra un texto en pantalla.
    public void ShowText(string text)
    {
    }
    // Esta funcion ejecuta la opcion MERCY.
    public void OnMercy()
    {
        battle.Mercy();
    }
}
