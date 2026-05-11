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

    void Start()
    {
        bool canStart;

        canStart = true;

        // si se olvida de asignar algun boton en el inspector, lo canta en consola
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

    void Update()
    {
        HandleInput();
    }

    // flechas para moverse entre botones, enter o tab para confirmar
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

    // hace un poco mas grande el boton seleccionado para que se vea cual esta marcado
    void UpdateSelection()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == selectedIndex)
            {
                buttons[i].transform.localScale = Vector3.one * 1.2f;
            }
            else
            {
                buttons[i].transform.localScale = Vector3.one;
            }
        }
    }

    public void EnableButtons(bool active)
    {
        ButtonFight.SetActive(active);
        ButtonAct.SetActive(active);
        ButtonItem.SetActive(active);
        ButtonMercy.SetActive(active);
    }

    // placeholder, lo deja vacio porque el texto lo gestiona el DialogueManager
    public void ShowText(string text)
    {
    }

    public void OnMercy()
    {
        battle.Mercy();
    }
}
