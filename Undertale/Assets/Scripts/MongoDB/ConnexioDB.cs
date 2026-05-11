using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;

public class ConnexioDB : MonoBehaviour
{
    private MongoClient client;
    private IMongoDatabase database;
    public static IMongoCollection<BsonDocument> usersCollection;

    void Start()
    {
        /**
        * Esta cadena es la cadena de conexión a MongoDB Atlas, tiene mi usuario y contraseña, y el nombre del cluster. 
        * (Me dijo el david que usaramos un "superusuario" usamos el mio y ya*/

        string connectionString = "mongodb+srv://a25unaforcas_db_user:PbMYfbacegSK8bNP@cluster0.xbths3k.mongodb.net/";

        try
        {
            client = new MongoClient(connectionString); // Conexión a MongoDB.
            database = client.GetDatabase("UndertaleDB"); // Nombre de la base de datos.
            usersCollection = database.GetCollection<BsonDocument>("a25unaforcas_db_user"); // Datos recogidos del jugador.
        }
        catch (System.Exception e)
        {
            Debug.LogError("MongoDB Connection Error: " + e.Message); // Si da error nos lo muestra por consola
        }
    }
}