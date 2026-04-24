using UnityEngine;

public class SaveController : MonoBehaviour
{
    public PlayerController playerStats;
    public SaveManager saveManager;

    private bool isPlayerInTrigger = false;

    void Awake() // Se inicia anets que el Start, para que esten bien las referencias.
    {
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerController>(); // Coge la referencia del PlayerController de la escena.
        }

        if (saveManager == null)
        {
            saveManager = FindObjectOfType<SaveManager>(); // Coge la referencia del SaveManager de la escena.
        }
        playerStats.GetComponent<PlayerController>();

    }

    void Update()
    {
        if (isPlayerInTrigger == true) // Condicion de si el jugador esta en el collider del punto de guardado.
        { 
            if (Input.GetKeyDown(KeyCode.G)) // Condicion de si se pulsa la tecla G para guardar.
            {
                Debug.Log("GUARDANDO...");

                // Condiciones que se usa para avisarnos si no estan bien las referencias. 

                if (playerStats == null)
                {
                    Debug.LogError("playerStats is null");
                }

                if (saveManager == null)
                {
                    Debug.LogError("saveManager is null");
                }     

                playerStats.playerData.getMaxhealth(); // Llama a la funcion que le da la vida maxima al player.
                string json = JsonUtility.ToJson(playerStats.playerData, true); // Coge los datos del jugador y los convierte a formato JSON.
                saveManager.SaveGame(json); // Llama a la funcion SaveGame del SaveManager, pasandole el JSON con los datos del jugador.

            }
        }

    }

    void OnTriggerEnter2D(Collider2D other) // Funcion que se utiliza para detectar si el jugador entra en el collider del punto de guardado.
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            Debug.Log("PLAYER DENTRO DEL PUNTO DE GUARDADO");
        }
    }

    void OnTriggerExit2D(Collider2D other) // Funcion que se utiliza para detectar si el jugador sale del collider del punto de guardado.
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            Debug.Log("PLAYER Fuera del punto de guardado");
        }
    }
}