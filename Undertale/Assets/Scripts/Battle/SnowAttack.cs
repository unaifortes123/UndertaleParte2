using UnityEngine;

public class SnowAttack : MonoBehaviour
{
    public GameObject iceShardPrefab;   // ❄️ el ataque
    public Transform spawnPoint;

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

        Debug.Log("Snowdrake lanza hielo ❄️");
    }
}