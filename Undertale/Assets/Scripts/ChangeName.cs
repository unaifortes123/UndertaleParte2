using UnityEngine;
using TMPro;

public class ChangeName : MonoBehaviour
{

    public TextMeshPro miTextoTMP;

    void Start()
    {
        // Le pone el texto del nombre del player.
        miTextoTMP.text = Name_Input.playerName;
    }
}
