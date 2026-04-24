using UnityEngine;
using MongoDB.Bson;
using System.Threading.Tasks;
using System;

public class MongoManager : MonoBehaviour
{
    void OnApplicationQuit() // Funcion que cuando se cierra el juego, guarda un numero de movimientos a la DB.
    {
        SaveMovementCount(); 
    }

    public void SaveMovementCount() 
    {
        BsonDocument document;

        if (ConnexioDB.usersCollection == null)
        {
            Debug.LogError("No hay conexión a MongoDB");
        } 
        else
        {
            document = new BsonDocument(); 

            document.Add("movementCount", CountMovement.movementCount); // Se añade el numero de movimientos.
            document.Add("date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")); // Se añade la fecha y hora actual.
            document.Add("timePlay", Mathf.RoundToInt(Time.time)); // Se añade el tiempo que se ha jugado en una sesion.
            ConnexioDB.usersCollection.InsertOne(document); // Inserta el document al MongoDB.

            Debug.Log("Datos guardados en MongoDB");


        }


    }
}