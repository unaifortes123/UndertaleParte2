using UnityEngine;

public class PuzzleButton : MonoBehaviour
{
    public PuzzleManager manager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            manager.PulsarBoton();
        }
    }
}