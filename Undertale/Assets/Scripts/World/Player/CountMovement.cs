using UnityEngine;

public class CountMovement : MonoBehaviour
{
    public static int movementCount = 0;

    // Esta funcion reinicia el contador de movimientos al empezar una nueva sesion.
    public static void ResetMovementCount()
    {
        movementCount = 0;
    }

    // Esta funcion suma un movimiento cuando el jugador empieza a moverse en un eje.
    public static void AddMovement()
    {
        movementCount += 1;
    }
}
