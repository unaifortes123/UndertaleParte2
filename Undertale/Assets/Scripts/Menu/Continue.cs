using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Continue : MonoBehaviour
{
    // Esta funcion cambia a la escena indicada.
    public void ChangeScene(string sceneName)
    {
       SceneManager.LoadScene(sceneName);
    }
}
