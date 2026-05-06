
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float horizontalSpeed;
    [SerializeField]
    float verticalSpeed;
    //[SerializeField]
    //float speed; // segunda manerade hacerlo
    [SerializeField]
    InputActionAsset actions;

    InputAction up_action;
    InputAction forward_action;

    Rigidbody2D rb;

    Animator animator;


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

        float foward = forward_action.ReadValue<float>();
        float up = up_action.ReadValue<float>();

        Vector2 movement_force = new Vector2(foward * horizontalSpeed, up * verticalSpeed);
        rb.AddForce(movement_force);

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



}
