using System.Reflection.Metadata.Ecma335;
using Newtonsoft.Json;

namespace libs;

public static class FileHandler
{
    private static string filePath = AppDomain.CurrentDomain.BaseDirectory;
    private readonly static string envVar = "GAME_SETUP_PATH";

    static FileHandler()
    {
        Initialize();
    }

    private static void Initialize()
    {
        if (Environment.GetEnvironmentVariable(envVar) != null)
        {
            filePath = Environment.GetEnvironmentVariable(envVar);
        }
    }

    public static dynamic ReadJson(string assetsPathToFile)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new InvalidOperationException("JSON file path not provided in environment variable");
        }

        try
        {
            string fullPath = Path.Combine(filePath, assetsPathToFile.TrimStart('/'));
            string jsonContent = File.ReadAllText(fullPath);
            dynamic jsonData = JsonConvert.DeserializeObject(jsonContent);
            return jsonData;
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException($"JSON file not found at path: {filePath + assetsPathToFile}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error reading JSON file: {ex.Message}");
        }
    }

    public static void WriteJson(string assetsPathToFile, object objectToWrite)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new InvalidOperationException("JSON file path not provided in environment variable");
        }

        try
        {
            string fullPath = Path.Combine(filePath, assetsPathToFile.TrimStart('/'));
            string directoryPath = Path.GetDirectoryName(fullPath);

            // Check if the directory exists
            if (!Directory.Exists(directoryPath))
            {
                // Create the directory
                Directory.CreateDirectory(directoryPath);
            }

            string jsonContent = JsonConvert.SerializeObject(objectToWrite, Formatting.Indented);
            File.WriteAllText(fullPath, jsonContent);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error writing JSON file: {ex.Message}");
        }
    }

    public static int CountLevelFiles()
    {
        string[] jsonLevelFiles = Directory.GetFiles(Path.Combine(filePath, "levels"), "*.json", SearchOption.TopDirectoryOnly);
        return jsonLevelFiles.Length;
    }

    public static bool SaveExists(int saveSlot)
    {
        return File.Exists(Path.Combine(filePath, "saves", $"save{saveSlot}.json"));
    }

    public static void SaveGame(Save save, int saveSlot)
    {
        WriteJson(Path.Combine("saves", $"save{saveSlot}.json"), save);
    }

    public static Save GetSave(int saveSlot)
    {
        dynamic saveData = ReadJson(Path.Combine("saves", $"save{saveSlot}.json"));
        Save save = new Save();
        save.MapState = JsonConvert.DeserializeObject<State>(saveData.MapState.ToString());
        save.CurrentLevel = saveData.CurrentLevel;
        save.MapHeight = saveData.MapHeight;
        save.MapWidth = saveData.MapWidth;
        save.SaveTimeStamp = saveData.SaveTimeStamp;
        return save;
    }
}
