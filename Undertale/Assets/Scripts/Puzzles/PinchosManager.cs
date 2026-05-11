using UnityEngine;

public class PinchosManager : MonoBehaviour
{
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
