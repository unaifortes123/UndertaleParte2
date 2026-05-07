using UnityEngine;

public class TileXOXO : MonoBehaviour
{
    public PuzzleManager manager;

    public Sprite cruz;
    public Sprite circuloRojo;

    private SpriteRenderer sr;
    private bool estaRojo = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = cruz;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // TOGGLE
        estaRojo = !estaRojo;

        if (estaRojo)
        {
            sr.sprite = circuloRojo;
        }
        else
        {
            sr.sprite = cruz;
        }

        manager.ComprobarPuzzle();
    }

    public bool EstaRojo()
    {
        return estaRojo;
    }
}