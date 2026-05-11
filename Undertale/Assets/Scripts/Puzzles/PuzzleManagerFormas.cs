using UnityEngine;

public class PuzzleManagerFormas : MonoBehaviour
{
    public PuzzleFormas[] tiles;
    public PinchosManager pinchosManager;

    // devuelve si está bien o no
    public bool Comprobar()
    {
        foreach (var tile in tiles)
        {
            if (!tile.EsCirculo())
                return false;
        }

        return true;
    }

    public void CompletarPuzzle()
    {
        foreach (var tile in tiles)
            tile.Completar();
        pinchosManager.BajarTodos();
    }

    public void ResetearPuzzle()
    {
        foreach (var tile in tiles)
            tile.Resetear();
    }
}