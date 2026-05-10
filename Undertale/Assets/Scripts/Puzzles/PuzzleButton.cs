using UnityEngine;

public class PuzzleButton : MonoBehaviour
{
    //Llamamos al código de puzzleManager 
    public PuzzleManager manager;
    //llamaremos al animator
    public Animator animator;
    //booleana para saber si ha sido utilizado
    private bool usado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {//si toca el trigger habilitado comprobamos si es el player
        if (!collision.CompareTag("Player")) return;
        if (usado) return;
        //estará usado
        usado = true;
        //animator se volvera true si ha sido presionado
        animator.SetBool("Pressed", true);

        manager.PulsarBoton();
    }
}