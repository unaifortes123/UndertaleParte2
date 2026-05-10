using UnityEngine;

public class SnowAttack : MonoBehaviour
{
    public GameObject iceShardPrefab;
    public Transform spawnPoint;

    // Esta funcion crea los proyectiles de hielo.
    public void ShootSnow()
    {
        for (int i = 0; i < 5; i++)
        {
            Instantiate(
                iceShardPrefab,
                spawnPoint.position,
                Quaternion.identity
            );
        }

    }
}
