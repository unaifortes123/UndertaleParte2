using UnityEngine;

public class SaveController : MonoBehaviour
{
    private PlayerVars playerStats;
    private SaveManager saveManager;

    private bool isPlayerInTrigger = false;
    private string json;

    // Esta funcion se ejecuta al empezar la escena y deja preparado el componente.
    void Start()
    {
        playerStats = PlayerVars.instance;
        saveManager = SaveManager.instance;
    }

    // Esta funcion se ejecuta cada frame y revisa la entrada o el estado actual.
    void Update()
    {
        if (isPlayerInTrigger == true)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                bool canSave;

                canSave = true;

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
                    Debug.LogError("playerStats esta null");
                    canSave = false;
                }

                if (saveManager == null)
                {
                    Debug.LogError("saveManager esta null");
                    canSave = false;
                }

                if (canSave == true)
                {

                    playerStats.playerData.getMaxhealth();


                    json = JsonUtility.ToJson(playerStats.playerData, true);


                    saveManager.SaveGame(json);
                }
            }
        }
    }

    // Esta funcion detecta cuando otro objeto entra en el trigger.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    // Esta funcion detecta cuando otro objeto sale del trigger.
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
        }
    }
}
