using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public GameObject botonFinal;

    public string puzzleTag; // 👈 ID del puzzle

    private TileXOXO[] tiles;

    void Start()
    {
        // cogemos solo las tiles del mismo tag
        tiles = FindObjectsOfType<TileXOXO>();

        if (botonFinal != null)
            botonFinal.SetActive(false);
    }

    public void ComprobarPuzzle()
    {
        foreach (var tile in tiles)
        {
            if (!tile.CompareTag(puzzleTag))
                continue;

            if (!tile.EstaRojo())
                return;
        }

        if (botonFinal != null)
            botonFinal.SetActive(true);
    }

    public void PulsarBoton()
    {
        foreach (var tile in tiles)
        {
            if (tile.CompareTag(puzzleTag))
                tile.PonerVerde();
        }
    }
}