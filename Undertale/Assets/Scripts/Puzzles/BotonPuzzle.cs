using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotonPuzzle : MonoBehaviour
{
    public PuzzleManager PuzzleManager;
    public Sprite botonNormal;
    public Sprite botonBajado;

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
        Debug.Log(PuzzleManager.name);
        bool correcto = PuzzleManager.ComprobarPuzzle();

        if (correcto)
        {
            completado = true;
            sr.sprite = botonBajado;

            PuzzleManager.PulsarBoton();
        }
    }
}