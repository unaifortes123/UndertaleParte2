using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{

    public GameObject PauseOptions;
    public GameObject volumeMenu;

    // Esta funcion se ejecuta al empezar la escena y deja preparado el componente.
    void Start()
    {
        PauseOptions.SetActive(false);
        volumeMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    // Esta funcion se ejecuta cada frame y revisa la entrada o el estado actual.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Si el menu de volumen esta abierto, vuelve al menu de pausa.
            if (volumeMenu.activeSelf)
            {
                BackMain();
            }
            // Si el menu de pausa esta abierto, vuelve al juego.
            else if (PauseOptions.activeSelf)
            {
                Continue();
            }
            // Si el jugador esta jugando, abre la pausa.
            else
            {
                Pause();
            }
        }
    }

    // Esta funcion abre el menu de pausa.
    public void Pause()
    {
        PauseOptions.SetActive(true);
        volumeMenu.SetActive(false);
        Time.timeScale = 0f;
    }

    // Esta funcion cierra los menus y vuelve al juego.
    public void Continue()
    {
        PauseOptions.SetActive(false);
        volumeMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    // Esta funcion abre el menu de volumen.
    public void OpenVolume()
    {
        PauseOptions.SetActive(false);
        volumeMenu.SetActive(true);
    }

    // Esta funcion vuelve desde volumen al menu de pausa.
    public void BackMain()
    {
        PauseOptions.SetActive(true);
        volumeMenu.SetActive(false);
    }

    // Esta funcion cierra el juego o detiene el editor.
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
