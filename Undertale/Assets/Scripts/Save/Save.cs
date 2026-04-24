using System.Diagnostics;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public void SaveGame(string json)
    {
        string jarPath;
        Process p;
        bool started;
        string output;
        string error;

        jarPath = Application.dataPath + "/../External/SaveSystem/SaveSystem.jar"; // Ruta relativa del archivo JAR

        if (File.Exists(jarPath) == false)
        {
            UnityEngine.Debug.LogError("Jar file not found: " + jarPath);
            return;
        }

        json = json.Replace("\"", "\\\""); // Ayuda a evitar problemas.

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

        /**
        UnityEngine.Debug.Log("Java output: " + output);
        UnityEngine.Debug.Log("Java error: " + error);
        UnityEngine.Debug.Log("Java exit code: " + p.ExitCode);
        */
    }
}