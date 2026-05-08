using UnityEngine;

public class BotonFormas : MonoBehaviour
{
    public PuzzleManagerFormas manager;

    public Sprite botonNormal;
    public Sprite botonCorrecto;

    private SpriteRenderer sr;

    private bool completado = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = botonNormal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (completado) return;

        bool correcto = manager.Comprobar();

        if (correcto)
        {
            completado = true;
            sr.sprite = botonCorrecto; // ← cambia sprite
            manager.CompletarPuzzle();
        }
        else
        {
            manager.ResetearPuzzle();
        }
    }
}