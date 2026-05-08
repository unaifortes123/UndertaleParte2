using UnityEngine;

public class PuzzleManagerFormas : MonoBehaviour
{
    //vector de formas que agregaremos
    public PuzzleFormas[] tiles;
    //llamamos al manager d elos pinchos
    public PinchosManager pinchosManager;
    // devuelve si está bien o no
    public bool Comprobar()
    {
        foreach (var tile in tiles)
        {
            //si no es círculo sera false
            if (!tile.EsCirculo())
                return false;
        }

        return true;
    }

    public void CompletarPuzzle()
    {
        //metodo para completar el puzzle en el que llamamos a la funcion completar
        foreach (var tile in tiles)
            tile.Completar();
        //entonces pondra el sprite donde estan bajados y desactivara el collider
        pinchosManager.BajarTodos();
    }

    public void ResetearPuzzle()
    {
        foreach (var tile in tiles)
            tile.Resetear();
    }
}