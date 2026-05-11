using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private string SaveFilePath
    {
        get
        {
            return Path.Combine(Application.dataPath, "../External/Save/save.json");
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveGame(string json)
    {
        string jarPath;
        Process p;
        bool started;
        string output;
        string error;

        jarPath = Application.dataPath + "/../External/SaveSystem/SaveSystem.jar";

        if (File.Exists(jarPath) == false)
        {
            UnityEngine.Debug.LogError("Jar file not found: " + jarPath);
            return;
        }

        json = json.Replace("\"", "\\\"");

        p = new Process();
        p.StartInfo.FileName = "java";
        p.StartInfo.Arguments = "-jar \"" + jarPath + "\" \"" + json + "\"";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.CreateNoWindow = true;

        started = p.Start();

        if (started == false)
        {
            UnityEngine.Debug.LogError("Java process could not start");
            return;
        }

        output = p.StandardOutput.ReadToEnd();
        error = p.StandardError.ReadToEnd();

        p.WaitForExit();

        UnityEngine.Debug.Log("Java output: " + output);
        UnityEngine.Debug.Log("Java error: " + error);
        UnityEngine.Debug.Log("Java exit code: " + p.ExitCode);
    }

    public bool LoadGame()
    {
        PlayerVars playerVars;
        PlayerVars.PlayerData loadedData = ReadSavedPlayerData();

        if (loadedData == null) return false;

        playerVars = PlayerVars.instance;

        if (playerVars == null)
        {
            UnityEngine.Debug.LogError("PlayerVars instance is null");
            return false;
        }

        playerVars.playerData = loadedData;
        return true;
    }

    public bool LoadGameAndScene(string sceneName)
    {
        if (LoadGame() == false)
        {
            return false;
        }

        SceneManager.LoadScene(sceneName);
        return true;
    }

    public PlayerVars.PlayerData ReadSavedPlayerData()
    {
        PlayerVars.PlayerData loadedData;
        string json;

        if (File.Exists(SaveFilePath) == false)
        {
            return null;
        }

        json = File.ReadAllText(SaveFilePath);
        loadedData = JsonUtility.FromJson<PlayerVars.PlayerData>(json);

        if (loadedData == null)
        {
            UnityEngine.Debug.LogError("Could not load player data from save");
            return null;
        }

        if (loadedData.completedFights == null)
        {
            loadedData.completedFights = new System.Collections.Generic.List<string>();
        }

        return loadedData;
    }
}
