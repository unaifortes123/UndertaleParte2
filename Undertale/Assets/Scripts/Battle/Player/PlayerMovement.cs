using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    float xMovement;
    float yMovement;
    Rigidbody2D rb;

    // Esta funcion guarda el Rigidbody del corazon.
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Esta funcion mueve el corazon con las flechas.
    void Update()
    {
        xMovement = Input.GetAxisRaw("Horizontal");
        yMovement = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector3(xMovement * speed, yMovement * speed);

    }

    // Esta funcion para el corazon cuando se desactiva el movimiento.
    void OnDisable()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }
}
