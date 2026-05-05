using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{

    public GameObject PauseOptions;
    public GameObject volumeMenu;

    void Start()
    {
        PauseOptions.SetActive(false);
        volumeMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Si estás en volumen → volver al menú principal
            if (volumeMenu.activeSelf)
            {
                BackMain();
            }
            // Si estás en pausa → volver al juego
            else if (PauseOptions.activeSelf)
            {
                Continue();
            }
            // Si estás jugando → abrir pausa
            else
            {
                Pause();
            }
        }
    }

    // 🔹 Abrir pausa
    public void Pause()
    {
        PauseOptions.SetActive(true);
        volumeMenu.SetActive(false);
        Time.timeScale = 0f;
    }

    // 🔹 Volver al juego
    public void Continue()
    {
        PauseOptions.SetActive(false);
        volumeMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    // 🔹 Abrir volumen
    public void OpenVolume()
    {
        PauseOptions.SetActive(false);
        volumeMenu.SetActive(true);
    }

    // 🔹 Volver al menú principal
    public void BackMain()
    {
        PauseOptions.SetActive(true);
        volumeMenu.SetActive(false);
    }

    // 🔹 Salir del juego
    public void Quit()
    {
        Debug.Log("QUIT PRESSED");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
