using UnityEngine;
using TMPro;

public class ChangeName : MonoBehaviour
{

    public TextMeshPro miTextoTMP;

    // Esta funcion pone el nombre del player en pantalla.
    void Start()
    {
        miTextoTMP.text = Name_Input.playerName;
    }
}
