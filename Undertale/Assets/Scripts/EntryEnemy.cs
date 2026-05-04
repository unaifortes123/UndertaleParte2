using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntryEnemy : MonoBehaviour
{
    public GameObject Player;
    private Boolean isPlayerInTrigger = false;
    private PlayerVars playerStats;
    private SaveManager saveManager;

    // Esta funcion carga las referencias basicas.
    void Start()
    {
        playerStats = PlayerVars.instance;
        saveManager = SaveManager.instance;

        RefreshPlayerDataFromSave();

        if (playerStats == null)
        {
            Debug.LogError("No se encontro PlayerVars en la escena.");
        }
    }

    // Esta funcion comprueba si toca entrar en combate.
    void Update()
    {
        if (playerStats == null)
        {
            playerStats = PlayerVars.instance;
        }

        if (saveManager == null)
        {
            saveManager = SaveManager.instance;
        }

        if (isPlayerInTrigger == true && playerStats != null)
        {
            if (gameObject.name == "SpawnHatsune")
            {
                if (!playerStats.playerData.completedFights.Contains("HatsuneMiku2"))
                {
                    SceneManager.LoadScene("HatsuneMiku2");
                }
                else
                {
                    Debug.Log("COMBATE YA COMPLETADO");
                }

            }

            if (gameObject.name == "SpawnPeter")
            {
                if (!playerStats.playerData.completedFights.Contains("Combat2"))
                {
                    Debug.Log("ENTRANDO EN COMBATE...");
                    SceneManager.LoadScene("Combat2");
                }
                else
                {
                    Debug.Log("COMBATE YA COMPLETADO");
                }

            }

        }
    }

    // Esta funcion detecta cuando el player entra en el trigger del enemigo.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RefreshPlayerDataFromSave();
            isPlayerInTrigger = true;
            Debug.Log("Is Entey");
        }
    }

    // Esta funcion detecta cuando el player sale del trigger del enemigo.
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            Debug.Log("Out of the Entry");
        }
    }

    // Esta funcion recarga los datos del player desde el json.
    void RefreshPlayerDataFromSave()
    {
        PlayerVars.PlayerData savedData;

        if (saveManager == null || playerStats == null)
        {
            return;
        }

        savedData = saveManager.ReadSavedPlayerData();

        if (savedData == null)
        {
            return;
        }

        playerStats.playerData = savedData;
    }
}
