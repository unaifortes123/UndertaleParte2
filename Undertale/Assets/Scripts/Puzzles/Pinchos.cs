using UnityEngine;

public class Pinchos : MonoBehaviour
{
    public Sprite arriba;
    //para cuando esten subidos y bajados
    public Sprite abajo;
    public Collider2D col;
    //llamamos al collider
    private SpriteRenderer sr;
    //llamamos al sprite renderer

    void Awake()
    {
        //declaramos q col coja el componente de collider2d
        col = GetComponent<Collider2D>();
        //con sprite renderer too
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = arriba;
        //que capte un sprite para arriba 
        Subir();//se inicializa subido
    }

    public void Bajar()
    {//metodo para bajar los pinchos donde el sprite sera en el que estén bajados
        sr.sprite = abajo;
        col.enabled = false;
        //deshabilitamos el collider cuando esten bajados
    }

    public void Subir()
    {//metodo para subir, habilitaremos el collider y no podrá pasar en ese caso
        col.enabled = true;
        sr.sprite = arriba;
    }
}