using System;
using System.IO;

namespace libs
{
    public static class LogUtility
    {
        private static readonly string logFilePath = "game_log.txt";

        public static void Log(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging failed: {ex.Message}");
            }
        }

        public static void LogCurrentDirectory()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            Log($"Current Directory: {currentDirectory}");
        }
    }
}