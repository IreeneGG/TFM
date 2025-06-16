using System.IO;
using UnityEngine;

public static class LogToFile
{
    private static readonly string logPath = Application.dataPath + "/../logs/entrenamiento.log";

    static LogToFile()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(logPath));
    }

    public static void Write(string message)
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        File.AppendAllText(logPath, $"[{timestamp}] {message}\n");
    }
}
