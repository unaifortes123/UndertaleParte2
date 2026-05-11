using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// movimiento del corazon dentro de la caja de combate (flechas WASD)
public class PlayerMovement : MonoBehaviour
{
    public float speed; // velocidad del alma esquivando
    float xMovement;
    float yMovement;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        xMovement = Input.GetAxisRaw("Horizontal");
        yMovement = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector3(xMovement * speed, yMovement * speed);

    }

    // al desactivar el script para la velocidad para que el alma no siga moviendose sola
    void OnDisable()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }
}
