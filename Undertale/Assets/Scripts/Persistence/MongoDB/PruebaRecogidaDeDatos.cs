using UnityEngine;
using MongoDB.Bson;
using System;

public class PruebaRecogidaDeDatos : MonoBehaviour
{
    private static PruebaRecogidaDeDatos instance;
    private static bool sessionSaved = false;
    private int totalSeconds;
    private int hours;
    private int minutes;
    private int seconds;
    private string formattedTime;

    // Esta funcion se ejecuta antes de Start y evita que haya mas de un recogedor de datos activo.
    void Awake()
    {
        bool canInitialize;

        canInitialize = true;

        if (instance != null && instance != this)
        {
            canInitialize = false;
            Destroy(this);
        }

        if (canInitialize == true)
        {
            instance = this;
            sessionSaved = false;
            CountMovement.ResetMovementCount();
        }
    }

    // Esta funcion se ejecuta al cerrar el juego y guarda los datos de la sesion una sola vez.
    void OnApplicationQuit()
    {
        if (sessionSaved == false)
        {
            SaveMovementCount();
        }
    }


    // Esta funcion crea el documento con los datos de la sesion y lo sube a MongoDB Atlas.
    public void SaveMovementCount()
    {
        BsonDocument document;
        bool canSave;

        canSave = true;

        if (ConnexioDB.usersCollection == null)
        {
            canSave = false;
            Debug.LogError("No hay conexion a MongoDB");
        }

        if (sessionSaved == true)
        {
            canSave = false;
        }

        if (canSave == true)
        {
            try
            {
                document = new BsonDocument();
                document.Add("movementCount", CountMovement.movementCount);
                document.Add("date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                // Calcular el tiempo de juego en horas, minutos y segundos
                totalSeconds = Mathf.RoundToInt(Time.time);
                hours = totalSeconds / 3600;
                minutes = (totalSeconds % 3600) / 60;
                seconds = totalSeconds % 60;

                formattedTime = hours + "h " + minutes + "min " + seconds + "s"; // Hace una string en formato de h,min y s.

                document.Add("timePlay", formattedTime); // Añade el tiempo de juego al documento.

                ConnexioDB.usersCollection.InsertOne(document);
                sessionSaved = true;

            }
            catch (Exception e)
            {
                Debug.LogError("Error al guardar los datos en MongoDB: " + e.Message);
            }
        }
    }
}
