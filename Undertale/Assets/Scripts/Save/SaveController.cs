using UnityEngine;

public class SaveController : MonoBehaviour
{
    private PlayerVars playerStats;
    private SaveManager saveManager;

    private bool isPlayerInTrigger = false;
    private string json;

    void Start()
    {
        playerStats = PlayerVars.instance;
        saveManager = SaveManager.instance;
    }

    void Update()
    {
        if (isPlayerInTrigger == true)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (playerStats == null)
                {
                    playerStats = PlayerVars.instance;
                }

                if (saveManager == null)
                {
                    saveManager = SaveManager.instance;
                }

                if (playerStats == null)
                {
                    Debug.LogError("playerStats is null");
                    return;
                }

                if (saveManager == null)
                {
                    Debug.LogError("saveManager is null");
                    return;
                }

                Debug.Log("GUARDANDO...");

                playerStats.playerData.getMaxhealth();

                Debug.Log("COMPLETED FIGHTS COUNT: " + playerStats.playerData.completedFights.Count);

                json = JsonUtility.ToJson(playerStats.playerData, true);

                Debug.Log(json);

                saveManager.SaveGame(json);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            Debug.Log("PLAYER DENTRO DEL PUNTO DE GUARDADO");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            Debug.Log("PLAYER FUERA DEL PUNTO DE GUARDADO");
        }
    }
}