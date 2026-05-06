using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hielo : MonoBehaviour
{
    // esto son variables para cuando pisa el hielo, el vector es para guaradar la dirección y el bool para saber si esta en hielo.
    private Vector2 direccionPlayerHielo;
    private bool estaEnHIelo;

    private float acceleration;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (estaEnHIelo) { 
           // acceleration =       
        }
    }

    public void OnTriggerEnter2D(Collider2D colision)
    {
        Debug.Log(colision.gameObject.name);
        if (colision.CompareTag("Player"))
        {
            estaEnHIelo = true;
            Debug.Log("Estoy en el hielo");

        }
    }
}
