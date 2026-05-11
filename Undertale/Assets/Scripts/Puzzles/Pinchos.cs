using UnityEngine;

public class Pinchos : MonoBehaviour
{
    public Sprite arriba;
    public Sprite abajo;
    public Collider2D col;
    private SpriteRenderer sr;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = arriba;
        Subir();
    }

    public void Bajar()
    {
        sr.sprite = abajo;
        col.enabled = false;
    }

    public void Subir()
    {
        col.enabled = true;
        sr.sprite = arriba;
    }
}
