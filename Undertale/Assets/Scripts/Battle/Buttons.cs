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

     // Se ejecuta antes del Start(), y coge la referencia del SpriteRendere para el buttonSprite

    void Awake()
    {
        buttonSprite = GetComponent<SpriteRenderer>();
    }


    // Funcio 
    public void SelectButton()
    {
        selected = true;
        ChangeSprite(buttonSelected);
    }

    public void DeselectButton()
    {
        selected = false;
        ChangeSprite(buttonDeselected);
    }

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