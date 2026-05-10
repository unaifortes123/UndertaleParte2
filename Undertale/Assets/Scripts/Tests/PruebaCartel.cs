using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaCartel : MonoBehaviour
{
    public string text = "";
    GameManager GameManager;
    // Esta funcion se ejecuta al empezar la escena y deja preparado el componente.
    void Start()
    {
        GameManager = FindObjectOfType<GameManager>();
    }

     // Esta funcion detecta cuando otro objeto entra en el trigger.
     void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.ShowText(text);
        }
    }
     // Esta funcion detecta cuando otro objeto sale del trigger.
     void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            GameManager.HideText();
        }
    }
}
