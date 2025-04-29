using System;
using System.IO;
using UnityEngine;

namespace AutoBuildSystem
{
    public enum LogLevel
    {
        Log,
        Warning,
        Error
    }
    
    public class BuildLogger
    {
        private readonly string logFilePath;
        private readonly LogLevel minLevel;
        private readonly object lockObject = new object();
        
        public BuildLogger(string path, LogLevel minLevel = LogLevel.Log)
        {
            this.logFilePath = path;
            this.minLevel = minLevel;
            YooAsset.Editor.EditorTools.CreateFileDirectory(this.logFilePath);
        }
        
        public void Log(string message)
        {
            LogInternal(LogLevel.Log, message);
        }
        
        public void LogWarning(string message)
        {
            LogInternal(LogLevel.Warning, message);
        }
        
        public void LogError(string message)
        {
            LogInternal(LogLevel.Error, message);
        }
        
        private void LogInternal(LogLevel level, string message)
        {
            if (level < minLevel) return;
            
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            
            // 输出到Unity控制台
            switch (level)
            {
                case LogLevel.Log:
                    Debug.Log(logMessage);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(logMessage);
                    break;
                case LogLevel.Error:
                    Debug.LogError(logMessage);
                    break;
            }
            
            lock (lockObject)
            {
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
        }
    }
    
    
}