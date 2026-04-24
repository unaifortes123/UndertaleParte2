using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CountMovement : MonoBehaviour
{
    public static int movementCount = 0;

    [SerializeField]
    InputActionAsset actions;

    PlayerController Movement;
    PlayerController forward;
    private float lastForwardValue = 0f;
    private float lastUpValue = 0f;
    private float currentForwardValue;
    private float currentUpValue;


    void Start()
    {
        movementCount = 0; // Inicializa el contador de movimientos a 0.
        Movement = GetComponent<PlayerController>(); // Obtiene el componente PlayerController.
        forward = GetComponent<PlayerController>(); // Obtiene el componente PlayerController.

    }

    void Update()
    {

        currentForwardValue = Movement.GetForward(); // Obtiene el valor del input de movimiento horizontal.
        currentUpValue = Movement.GetUp(); // Obtiene el valor del input de movimiento vertical.

        if ((currentForwardValue == 1 || currentForwardValue == -1) && lastForwardValue == 0) // Condicion de si el player se esta moviendo en su eje Forward
        {
            movementCount += 1;
            //Debug.Log("Se ha guardado el FORWARD");
        }

        if ((currentUpValue == 1 || currentUpValue == -1) && lastUpValue == 0) // Condicion de si el player se esta moviendo en su eje Up
        {
            movementCount += 1;
            //Debug.Log("Se ha guardado el UP");
        }

        lastForwardValue = currentForwardValue; // Actualiza el valor del frame anterior
        lastUpValue = currentUpValue; // Actualiza el valor del frame anterior

    }
}