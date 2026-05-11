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

    void Awake()
    {
        buttonSprite = GetComponent<SpriteRenderer>();
    }

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

    // cambia el sprite, y por si acaso vuelve a pillar el SpriteRenderer si se perdio
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
