using UnityEngine;

public class TileXOXO : MonoBehaviour
{
    //llamamos al script que hemos creado anteriormente como Manager del puzzle
    public PuzzleManager manager;

    //pedimos los sprites que querremos en el editor
    public Sprite cruz;
    public Sprite circuloRojo;
    public Sprite circuloVerde;

    //declaramos un spriteRenderer
    private SpriteRenderer sr;
    //booleanas para detectar si sigue en rojo o se ha completado
    private bool estaRojo = false;
    private bool completado = false;

    void Start()
    {
        //inicializamos el sprite renderer cogiendolo con getComponent
        sr = GetComponent<SpriteRenderer>();
        //pillamos la cruz
        sr.sprite = cruz;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //método para cuando detecte el trigger habilitado del personaje
        if (!collision.CompareTag("Player")) return;
        if (completado) return;

        // toggle cruz <-> rojo, para que cambie el estado
        estaRojo = !estaRojo;

        if (estaRojo)
        {//para el circulo
            sr.sprite = circuloRojo;
        }
        else
        {//cruz, todo esto hara que si volvemos a ponernos encima cambie otra vez
            sr.sprite = cruz;
        }

        manager.ComprobarPuzzle();
        //llamamos a la funcion comprobar puzzle
    }

    //comprobamos si esta rojo
    public bool EstaRojo()
    {
        return estaRojo;
    }

    //comprobamos si esta verde
    public void PonerVerde()
    {
        completado = true;
        sr.sprite = circuloVerde;
    }
}