using UnityEngine;

public class PinchosManager : MonoBehaviour
{
    //vector de pinchos para agregar todos los que tengamos
    public Pinchos[] pinchos;

    public void BajarTodos()
    {
        foreach (var p in pinchos)
            p.Bajar();
    }

    public void SubirTodos()
    {
        foreach (var p in pinchos)
            p.Subir();
    }
}