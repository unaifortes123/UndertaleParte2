using UnityEngine;

public class PuzzleFormas : MonoBehaviour
{
    //4 publicos para pedir cada sprite
    public Sprite cruz;
    public Sprite circulo;
    public Sprite triangulo;
    public Sprite trianguloVerde;

    //llamamos al sprite renderer
    private SpriteRenderer sr;
    private int estado = 0;
    //inicializamos el estado en 0
    private bool completado = false;
    //completado se inicializa en false

    void Start()
    {
        //pillamos el componente de sprite renderer
        sr = GetComponent<SpriteRenderer>();
        //empezará siendo cruz
        sr.sprite = cruz;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //si no colisiona con player devuelve
        if (!collision.CompareTag("Player")) return;
        if (completado) return;
        //si esta completado return
        //cambiamos de estado, llamamos al metodo
        CambiarEstado();
    }

    void CambiarEstado()
    {
        //sumamos a estado que estaba inicializado en 0
        estado++;

        //si estado es mayor a 2, vuelve a 0
        if (estado > 2)
            estado = 0;
        
        //llamamos al metodo actualizar sprite
        ActualizarSprite();
    }

    void ActualizarSprite()
    {
        //un switch para cada estado, cada tipo 
        switch (estado)
        {
            case 0: 
                sr.sprite = cruz; 
                break;
            case 1:
                sr.sprite = circulo;
                break;
            case 2: 
                sr.sprite = triangulo; 
                break;
        }
    }

    public bool EsCirculo()
    {//metodo para el circulo, será el estado 1
        return estado == 1;
    }

    public void Resetear()
    {//metodo para resetear si lo necesitasemos
        completado = false;
        estado = 0;
        sr.sprite = cruz;
    }

    public void Completar()
    {//completar, en caso de que sea así se convierte en verde
        completado = true;
        sr.sprite = trianguloVerde;
    }
}