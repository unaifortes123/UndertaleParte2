using UnityEngine;

public class PuzzleButton : MonoBehaviour
{
    public PuzzleManager manager;

    public Sprite botonNormal;
    public Sprite botonCorrecto;

    private SpriteRenderer sr;

    private bool usado = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = botonNormal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (usado) return;

        bool correcto = manager.ComprobarPuzzle();

        if (correcto)
        {
            usado = true;
            sr.sprite = botonCorrecto;

            manager.PulsarBoton(); // baja pinchos aquí
        }
    }
}