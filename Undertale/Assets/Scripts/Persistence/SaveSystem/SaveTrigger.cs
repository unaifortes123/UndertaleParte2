using UnityEngine;

public class SaveTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger activado");

        if (PlayerVars.instance == null)
        {
            Debug.LogError("PlayerVars.instance ES NULL");
            return;
        }

        if (SaveManager.instance == null)
        {
            Debug.LogError("SaveManager.instance ES NULL");
            return;
        }

        if (PlayerVars.instance.playerData == null)
        {
            Debug.LogError("playerData ES NULL");
            return;
        }

        string json = JsonUtility.ToJson(PlayerVars.instance.playerData);
        SaveManager.instance.SaveGame(json);
    }
}
