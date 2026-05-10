using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hielo : MonoBehaviour
{
    // esto son variables para cuando pisa el hielo, el vector es para guaradar la dirección y el bool para saber si esta en hielo.
    private Vector2 direccionPlayerHielo;
    private bool estaEnHIelo;

    private float acceleration;

    // Esta funcion se ejecuta al empezar la escena y deja preparado el componente.
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

    // Esta funcion detecta cuando otro objeto entra en el trigger.
    public void OnTriggerEnter2D(Collider2D colision)
    {
        if (colision.CompareTag("Player"))
        {
            estaEnHIelo = true;

        }
    }
}
