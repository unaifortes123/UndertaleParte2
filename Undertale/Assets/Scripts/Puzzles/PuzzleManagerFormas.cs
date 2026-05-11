using UnityEngine;

public class PuzzleManagerFormas : MonoBehaviour
{
    public PuzzleFormas[] tiles;
    public PinchosManager pinchosManager;

    // Comprueba si el puzzle está bien
    public bool Comprobar()
    {
        foreach (var tile in tiles)
        {
            if (!tile.EsCirculo())
                return false;
        }

        return true;
    }

    // Resolver puzzle (acciones finales)
    public void CompletarPuzzle()
    {
        foreach (var tile in tiles)
        {
            tile.Completar();
        }

        if (pinchosManager != null)
            pinchosManager.BajarTodos();
    }

    // Reset puzzle
    public void ResetearPuzzle()
    {
        foreach (var tile in tiles)
            tile.Resetear();

        if (pinchosManager != null)
            pinchosManager.SubirTodos();
    }
}