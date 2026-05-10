using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;

public class ConnexioDB : MonoBehaviour
{
    private static ConnexioDB instance;
    private MongoClient client;
    private IMongoDatabase database;
    public static IMongoCollection<BsonDocument> usersCollection;

    // Esta funcion se ejecuta antes de Start y evita que haya mas de un MongoDBManager vivo.
    void Awake()
    {
        bool canInitialize;

        canInitialize = true;

        if (instance != null && instance != this)
        {
            canInitialize = false;
            Destroy(gameObject);
        }

        if (canInitialize == true)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Esta funcion prepara la conexion con MongoDB Atlas una sola vez.
    void Start()
    {
        string connectionString;
        bool canConnect;

        connectionString = "mongodb+srv://a25unaforcas_db_user:PbMYfbacegSK8bNP@cluster0.xbths3k.mongodb.net/";
        canConnect = instance == this;

        if (canConnect == true)
        {
            try
            {
                client = new MongoClient(connectionString);
                database = client.GetDatabase("UndertaleDB");
                usersCollection = database.GetCollection<BsonDocument>("a25unaforcas_db_user");
            }
            catch (System.Exception e)
            {
                Debug.LogError("MongoDB Connection Error: " + e.Message);
            }
        }
    }
}
