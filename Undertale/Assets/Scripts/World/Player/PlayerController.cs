using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float speed; //Velocidad del personaje



    // este vector es el de movimiento, tenemos foward (derecha izquierda) y up (arriba y abajo)
    float foward;
    float up;
    Vector2 movement; //vector del movimiento


    //[SerializeField]
    //float speed; // segunda manerade hacerlo
    [SerializeField]
    InputActionAsset actions;

    InputAction up_action;
    InputAction forward_action;

    Rigidbody2D rb; //pillamos el rigidbody del personaje

    Animator animator;

    private bool canMove = true;

    void Start()
    {
        actions.Enable();
        forward_action = actions.FindActionMap("Movement").FindAction("Forward");
        up_action = actions.FindActionMap("Movement").FindAction("Up");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        //Debug.Log("forwardddd " + forward_action.ReadValue<float>());
        //Debug.Log("forwardddd " + up_action.ReadValue<float>());

        //Movimiento
        foward = forward_action.ReadValue<float>();
        up = up_action.ReadValue<float>();
        movement = new Vector2(foward, up);
        movement = movement.normalized; //esto es para que cuando vas en diagonal el personaje no corra mas


        if (canMove == true)
        {
            rb.velocity = movement * speed;

        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        // Animación
        int animationValueX = 0;

        if (foward != 0)
        {
            animationValueX = (int)foward;
        }

        int animationValueY = 0;
        if (up != 0)
        {
            animationValueY = (int)up;
        }

        animator.SetInteger("SpeedX", animationValueX);
        animator.SetInteger("SpeedY", animationValueY);
        // Vector2 moveInput = new Vector2 (foward, up);
        // rb.MovePosition(rb.position + moveInput * speed * Time.deltaTime

    }

    public float GetUp() // Funcion que devuelve el valor que coge del input del movimiento vertical.
    {
        Debug.Log("Valor UP :" + up_action.ReadValue<float>());
        return up_action.ReadValue<float>();
    }

    public float GetForward() // Funcion que devuelve el valor que coge del input del movimiento horizontal.
    {
        Debug.Log("Valor FORWARD :" + forward_action.ReadValue<float>());
        return forward_action.ReadValue<float>();
    }
    public void SetCanMove(bool newCanMove)
    {
        canMove = newCanMove;
    }



}