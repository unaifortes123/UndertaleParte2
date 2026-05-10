using UnityEngine;

public class PuzzleFormas : MonoBehaviour
{
    public Sprite cruz;
    public Sprite circulo;
    public Sprite triangulo;
    public Sprite circuloVerde;

    private SpriteRenderer sr;
    private int estado = 0;
    private bool completado = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = cruz;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (completado) return;

        CambiarEstado();
    }

    void CambiarEstado()
    {
        estado++;

        if (estado > 2)
            estado = 0;

        ActualizarSprite();
    }

    void ActualizarSprite()
    {
        switch (estado)
        {
            case 0: sr.sprite = cruz; break;
            case 1: sr.sprite = circulo; break;
            case 2: sr.sprite = triangulo; break;
        }
    }

    public bool EsCirculo()
    {
        return estado == 1;
    }

    public void Resetear()
    {
        completado = false;
        estado = 0;
        sr.sprite = cruz;
    }

    public void Completar()
    {
        completado = true;
        sr.sprite = circuloVerde;
    }
}