using System;
using System.IO;
using System.Text;
using BIL_Interpreter.Features;

namespace BIL_Interpreter.Helpers;

public class Logger(ProgramEnvironment environment = null)
{
    /// <summary>
    /// Gets the global <see cref="Logger"/> instance
    /// </summary>
    public static Logger Instance { get; } = new();

    /// <summary>
    /// Gets or sets whether the debug should be enabled or not
    /// </summary>
    public static bool EnableDebug { get; set; } = false;
    
    private static readonly string GlobalFilePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), ".exec_logs.txt");

    private string FilePath { get; } = environment is null ? GlobalFilePath : environment.LogFile;

    private ProgramEnvironment Environment { get; } = environment;
    
    private FileStream LogWriter { get; set; } = null;

    public void Log(string level, string message)
    {
        if (FilePath is null)
            return;
        
        if (LogWriter is null)
        {
            File.WriteAllText(FilePath, "");
            LogWriter = File.Open(FilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        }

        string raw = $"[{DateTime.Now:g}][{level}] {message}\n";
        
        LogWriter.Write(Encoding.UTF8.GetBytes(raw), 0, raw.Length);
        LogWriter.Flush();
    }

    public void Error(string message)
    {
        Log("ERROR", message);
    }
    
    public void Warn(string message)
    {
        Log("WARN", message);
    }

    public void Info(string message)
    {
        Log("INFO", message);
    }

    public void Debug(string message)
    {
        if (Environment is not null && Environment.Debug)
            Log("DEBUG", message);
        
        if (Environment is null && EnableDebug)
            Log("DEBUG", message);
    }
}