using UnityEngine;
using UnityEngine.UI;

public class UIBattle : MonoBehaviour
{
    public GameObject ButtonFight;
    public GameObject ButtonAct;
    public GameObject ButtonItem;
    public GameObject ButtonMercy;

    public BattleManager2 battle;

    private GameObject[] buttons;
    private int selectedIndex = 0;

    void Start()
    {
        if (!ButtonFight || !ButtonAct || !ButtonItem || !ButtonMercy)
        {
            Debug.LogError("❌ Faltan botones asignados en el Inspector");
            return;
        }

        if (!battle)
        {
            Debug.LogError("❌ Falta BattleManager asignado");
            return;
        }

        buttons = new GameObject[]
        {
            ButtonFight,
            ButtonAct,
            ButtonItem,
            ButtonMercy
        };

        UpdateSelection();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (!battle || buttons == null) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = buttons.Length - 1;
            UpdateSelection();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedIndex++;
            if (selectedIndex >= buttons.Length) selectedIndex = 0;
            UpdateSelection();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Tab))
        {
            buttons[selectedIndex].GetComponent<Button>().onClick.Invoke();
        }
    }

    void UpdateSelection()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == selectedIndex)
                buttons[i].transform.localScale = Vector3.one * 1.2f; // seleccionado
            else
                buttons[i].transform.localScale = Vector3.one; // normal
        }
    }

   

    public void EnableButtons(bool active)
    {
        ButtonFight.SetActive(active);
        ButtonAct.SetActive(active);
        ButtonItem.SetActive(active);
        ButtonMercy.SetActive(active);
    }

    public void ShowText(string text)
    {
        Debug.Log(text);
    }
    public void OnMercy()
    {
        battle.PlayerAction("MERCY");
    }
}