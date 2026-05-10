using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class CopyExternalOnBuild : IPostprocessBuildWithReport
{
    public int callbackOrder
    {
        get
        {
            return 0;
        }
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        string projectFolder;
        string sourceFolder;
        string buildFolder;
        string targetFolder;

        projectFolder = Directory.GetParent(Application.dataPath).FullName;
        sourceFolder = Path.Combine(projectFolder, "External");
        buildFolder = Path.GetDirectoryName(report.summary.outputPath);
        targetFolder = Path.Combine(buildFolder, "External");

        if (Directory.Exists(sourceFolder))
        {
            if (Directory.Exists(targetFolder))
            {
                Directory.Delete(targetFolder, true);
            }

            CopyFolder(sourceFolder, targetFolder);
            Debug.Log("External folder copied to build: " + targetFolder);
        }
        else
        {
            Debug.LogWarning("External folder not found: " + sourceFolder);
        }
    }

    private void CopyFolder(string sourceFolder, string targetFolder)
    {
        string[] folders;
        string[] files;
        string folderName;
        string fileName;
        string targetPath;
        int i;

        folders = Directory.GetDirectories(sourceFolder);
        files = Directory.GetFiles(sourceFolder);
        folderName = "";
        fileName = "";
        targetPath = "";
        i = 0;

        if (Directory.Exists(targetFolder) == false)
        {
            Directory.CreateDirectory(targetFolder);
        }

        for (i = 0; i < files.Length; i++)
        {
            fileName = Path.GetFileName(files[i]);
            targetPath = Path.Combine(targetFolder, fileName);
            File.Copy(files[i], targetPath, true);
        }

        for (i = 0; i < folders.Length; i++)
        {
            folderName = Path.GetFileName(folders[i]);
            targetPath = Path.Combine(targetFolder, folderName);
            CopyFolder(folders[i], targetPath);
        }
    }
}
