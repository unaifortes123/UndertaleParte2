using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaCartel : MonoBehaviour
{
    public string text = "";
    GameManager GameManager;
    void Start()
    {
        GameManager = FindObjectOfType<GameManager>();
    }

     void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("aaaa");
            GameManager.ShowText(text);
        }
    }
     void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("eeeee");

            GameManager.HideText();
        }
    }
}
