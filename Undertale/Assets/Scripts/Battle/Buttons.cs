using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    public Sprite buttonDeselected;
    public Sprite buttonSelected;
    public bool selected;
    public Transform soulPosition;

    private SpriteRenderer buttonSprite;

    // Esta funcion guarda el sprite del boton.
    void Awake()
    {
        buttonSprite = GetComponent<SpriteRenderer>();
    }

    // Esta funcion marca el boton como seleccionado.
    public void SelectButton()
    {
        selected = true;
        ChangeSprite(buttonSelected);
    }

    // Esta funcion marca el boton como no seleccionado.
    public void DeselectButton()
    {
        selected = false;
        ChangeSprite(buttonDeselected);
    }

    // Esta funcion cambia el dibujo del boton.
    void ChangeSprite(Sprite newSprite)
    {
        if (buttonSprite == null)
        {
            buttonSprite = GetComponent<SpriteRenderer>();
        }

        if (buttonSprite != null && newSprite != null)
        {
            buttonSprite.sprite = newSprite;
        }
    }
}
