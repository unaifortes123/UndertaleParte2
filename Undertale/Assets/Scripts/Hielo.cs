using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hielo : MonoBehaviour
{
    // esto son variables para cuando pisa el hielo, el vector es para guaradar la dirección y el bool para saber si esta en hielo.
    private Vector2 direccionPlayerHielo; //direccion del movimiento
    private bool estaEnHIelo; //bool que indica si esta en el hielo
    private Rigidbody2D rbPlayer; // Esto guarda el rb de lo que entra, en este caso es el player.

    private float velocidadHielo = 5f; //velocidad del hielo
    PlayerController player;  // variable player

    void Start()
    {
        
    }

    void Update()
    {
        if (estaEnHIelo) // if de que si esta en el hielo, la velocidad del player es la dirección guardada por la velocidad.
        {
            rbPlayer.velocity = direccionPlayerHielo * velocidadHielo;
        }
    }

    public void OnTriggerEnter2D(Collider2D colision) //si el usuario esta colisionando con el collider del hielo
    {
        // Debug.Log(colision.gameObject.name);
      

        if (colision.CompareTag("Player")) // si tiene la etiqueta player (literalmente el player)
        {
            
            rbPlayer = colision.GetComponent<Rigidbody2D>(); //creamos la variable rbPlayer que guarda el rigidbody2D del player
            player = colision.GetComponent<PlayerController>(); //creamos la variable player que guarda el player controller
            if (player != null) // if de si existe player, setea la variable canMove (en el player controller) a false, haciendo de que no se pueda mover
            {
                player.SetCanMove(false);
            }

            estaEnHIelo = true; //indica esta en el hielo es true
            direccionPlayerHielo = rbPlayer.velocity.normalized; // guarda la dirección del player en la variable 
            Debug.Log("Estoy en el hielo");

        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // si tiene la etiqueta player (literalmente el player)
        {
            player = collision.GetComponent<PlayerController>(); //creamos la variable player que guarda el player controller
            if (player != null)  // if de si existe player, setea la variable canMove (en el player controller) a true, haciendo de que se pueda mover
            {
                player.SetCanMove(true);
            }
            estaEnHIelo =false; //indica esta en el hielo es false
            rbPlayer = null; //reinicia el rigidbody
            Debug.Log("No estoy en el hielo");
        }
    }
}
