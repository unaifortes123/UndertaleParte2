
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

    private float lastForwardValue = 0f;
    private float lastUpValue = 0f;

    // Esta funcion se ejecuta al empezar la escena y deja preparado el componente.
    void Start()
    {
        actions.Enable();
        forward_action = actions.FindActionMap("Movement").FindAction("Forward");
        up_action = actions.FindActionMap("Movement").FindAction("Up");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }


    // Esta funcion se ejecuta cada frame y revisa la entrada o el estado actual.
    void Update()
    {
        float forward;
        float up;
        Vector2 movementForce;
        int animationValueX;
        int animationValueY;


        forward = forward_action.ReadValue<float>();
        up = up_action.ReadValue<float>();

        CountPlayerMovement(forward, up);

        movementForce = new Vector2(forward * horizontalSpeed, up * verticalSpeed);
        rb.AddForce(movementForce);

        animationValueX = 0;

        if (forward != 0)
        {
            animationValueX = (int)forward;
        }

        animationValueY = 0;

        if (up != 0)
        {
            animationValueY = (int)up;
        }

        animator.SetInteger("SpeedX", animationValueX);
        animator.SetInteger("SpeedY", animationValueY);
        // Vector2 moveInput = new Vector2 (foward, up);
        // rb.MovePosition(rb.position + moveInput * speed * Time.deltaTime

    }

    // Esta funcion suma movimientos cuando el jugador empieza a moverse en horizontal o vertical.
    private void CountPlayerMovement(float currentForwardValue, float currentUpValue)
    {
        if (Mathf.Abs(currentForwardValue) > 0.01f && Mathf.Abs(lastForwardValue) <= 0.01f)
        {
            CountMovement.AddMovement();
        }

        if (Mathf.Abs(currentUpValue) > 0.01f && Mathf.Abs(lastUpValue) <= 0.01f)
        {
            CountMovement.AddMovement();
        }

        lastForwardValue = currentForwardValue;
        lastUpValue = currentUpValue;
    }

}
