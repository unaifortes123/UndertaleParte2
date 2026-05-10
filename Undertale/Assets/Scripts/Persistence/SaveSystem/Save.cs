using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private Process watchProcess;
    private string externalFolderPath;
    private string externalSaveFolderPath;
    private string saveSystemFolderPath;
    private string saveFilePath;
    private string loadFilePath;

    // Esta funcion prepara el sistema de guardado cuando aparece el objeto en la escena.
    void Awake()
    {
        bool canInitialize;

        canInitialize = true;

        if (instance != null && instance != this)
        {
            Destroy(this);
            canInitialize = false;
        }

        if (canInitialize == true)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            PreparePaths();
            EnsureJsonFiles();
            StartSaveWatcher();
        }
    }

    // Esta funcion calcula todas las rutas que usa el sistema de guardado.
    private void PreparePaths()
    {
        externalFolderPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../External"));
        externalSaveFolderPath = Path.Combine(externalFolderPath, "Save");
        saveSystemFolderPath = Path.Combine(externalFolderPath, "SaveSystem");
        saveFilePath = Path.Combine(externalSaveFolderPath, "save.json");
        loadFilePath = Path.Combine(externalSaveFolderPath, "load.json");
    }

    // Esta funcion guarda el JSON de Unity y lo sube a MySQL usando UploadDB.
    public void SaveGame(string json)
    {
        EnsureJsonFiles();
        File.WriteAllText(saveFilePath, json);
        RunUploadDb("save", true);
    }

    // Esta funcion carga la partida desde MySQL y la pone en PlayerVars.
    public bool LoadGame()
    {
        PlayerVars playerVars;
        PlayerVars.PlayerData loadedData;
        bool loaded;

        playerVars = null;
        loadedData = null;
        loaded = false;

        EnsureJsonFiles();
        RunUploadDb("load", true);
        loadedData = ReadSavedPlayerData();

        if (loadedData != null)
        {
            playerVars = PlayerVars.instance;

            if (playerVars == null)
            {
                UnityEngine.Debug.LogError("PlayerVars instance is null");
            }
            else
            {
                playerVars.playerData = loadedData;
                loaded = true;
            }
        }

        return loaded;
    }

    // Esta funcion carga la partida y cambia a la escena indicada si la carga ha ido bien.
    public bool LoadGameAndScene(string sceneName)
    {
        bool loaded;

        loaded = LoadGame();

        if (loaded == true)
        {
            SceneManager.LoadScene(sceneName);
        }

        return loaded;
    }

    // Esta funcion lee load.json y lo convierte a datos del jugador para Unity.
    public PlayerVars.PlayerData ReadSavedPlayerData()
    {
        PlayerVars.PlayerData loadedData;
        string json;
        string filePath;

        loadedData = null;
        json = "";
        filePath = "";

        EnsureJsonFiles();
        filePath = loadFilePath;

        if (File.Exists(filePath) == false)
        {
            filePath = saveFilePath;
        }

        if (File.Exists(filePath) == true)
        {
            json = File.ReadAllText(filePath);
            loadedData = JsonUtility.FromJson<PlayerVars.PlayerData>(json);

            if (loadedData == null)
            {
                UnityEngine.Debug.LogError("Could not load player data from save");
            }
            else
            {
                if (loadedData.completedFights == null)
                {
                    loadedData.completedFights = new System.Collections.Generic.List<string>();
                }
            }
        }

        return loadedData;
    }

    // Esta funcion crea la carpeta Save y los JSON si alguien los ha borrado (o por si pasa cualquier cosa con ellos).
    private void EnsureJsonFiles()
    {
        string defaultJson;
        // Esta es la estructura por defecto de los Json del sistema de guardado.
        defaultJson = "{\n"
            + "    \"health\": 20.0,\n"
            + "    \"score\": 0.0,\n"
            + "    \"playerName\": \"\",\n"
            + "    \"completedFights\": []\n"
            + "}\n";

        if (Directory.Exists(externalSaveFolderPath) == false)
        {
            Directory.CreateDirectory(externalSaveFolderPath);
        }

        if (File.Exists(saveFilePath) == false)
        {
            File.WriteAllText(saveFilePath, defaultJson);
        }

        if (File.Exists(loadFilePath) == false)
        {
            File.WriteAllText(loadFilePath, defaultJson);
        }
    }

    // Esta funcion arranca UploadDB en modo watch para vigilar save.json.
    private void StartSaveWatcher()
    {
        if (watchProcess == null)
        {
            watchProcess = RunUploadDb("watch", false);
        }
    }

    // Esta funcion ejecuta UploadDB con la opcion indicada.
    private Process RunUploadDb(string option, bool waitForExit)
    {
        Process process;
        string classPath;
        string output;
        string error;
        bool started;

        process = null;
        classPath = "." + Path.PathSeparator + "mysql-connector-j-9.6.0.jar";
        output = "";
        error = "";
        started = false;

        if (Directory.Exists(saveSystemFolderPath) == false)
        {
            UnityEngine.Debug.LogError("SaveSystem folder not found: " + saveSystemFolderPath);
        }
        else if (File.Exists(Path.Combine(saveSystemFolderPath, "UploadDB.class")) == false)
        {
            UnityEngine.Debug.LogError("UploadDB.class not found: " + saveSystemFolderPath);
        }
        else
        {
            process = new Process();
            process.StartInfo.FileName = "java";
            process.StartInfo.Arguments = "-cp \"" + classPath + "\" UploadDB " + option;
            process.StartInfo.WorkingDirectory = saveSystemFolderPath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = waitForExit;
            process.StartInfo.RedirectStandardError = waitForExit;
            process.StartInfo.CreateNoWindow = true;

            started = process.Start();

            if (started == false)
            {
                UnityEngine.Debug.LogError("Java process could not start");
                process = null;
            }
            else if (waitForExit == true)
            {
                output = process.StandardOutput.ReadToEnd();
                error = process.StandardError.ReadToEnd();
                process.WaitForExit();

            }
        }

        return process;
    }

    // Esta funcion cierra el proceso watch cuando se cierra el juego.
    // El proceso watch se encarga de "escuchar" los cambios del save.json para subirlo a la BD local.
    private void OnApplicationQuit()
    {
        if (watchProcess != null && watchProcess.HasExited == false)
        {
            watchProcess.Kill();
        }
    }
}
