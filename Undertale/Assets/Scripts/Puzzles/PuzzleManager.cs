using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public GameObject botonFinal;

    private TileXOXO[] tiles;

    void Start()
    {
        tiles = FindObjectsOfType<TileXOXO>();

        if (botonFinal != null)
            botonFinal.SetActive(false);
    }

    public void ComprobarPuzzle()
    {
        foreach (var tile in tiles)
        {
            if (!tile.EstaRojo())
                return; // aún no todas en rojo
        }

        // todas están en rojo
        if (botonFinal != null)
            botonFinal.SetActive(true);
    }

    public void PulsarBoton()
    {
        foreach (var tile in tiles)
        {
            // poner todas en verde o estado final
            tile.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }
}