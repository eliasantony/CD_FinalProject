using System.Reflection.Metadata.Ecma335;

namespace libs;

using Newtonsoft.Json;

public static class FileHandler
{
    private static string filePath;
    private readonly static string envVar = "GAME_SETUP_PATH";

    static FileHandler()
    {
        Initialize();
    }

    private static void Initialize()
    {
        if(Environment.GetEnvironmentVariable(envVar) != null){
            filePath = Environment.GetEnvironmentVariable(envVar);
        };
    }

    public static dynamic ReadJson(string assetsPathToFile)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new InvalidOperationException("JSON file path not provided in environment variable");
        }

        try
        {
            string jsonContent = File.ReadAllText(filePath + assetsPathToFile);
            dynamic jsonData = JsonConvert.DeserializeObject(jsonContent);
            return jsonData;
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException($"JSON file not found at path: {filePath}");
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
            string jsonContent = JsonConvert.SerializeObject(objectToWrite, Formatting.Indented);
            File.WriteAllText(filePath + assetsPathToFile, jsonContent);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error writing JSON file: {ex.Message}");
        }
    }
    
    public static int CountLevelFiles()
    {
        string[] jsonLevelFiles = Directory.GetFiles(filePath + "/levels/", "*.json", SearchOption.TopDirectoryOnly);

        return jsonLevelFiles.Length;
    }
    
    public static bool SaveExists(int saveSlot)
    {
        return File.Exists(filePath + "/saves/save" + saveSlot + ".json");
    }
    
    public static void SaveGame(Save save, int saveSlot)
    {
        WriteJson("/saves/save" + saveSlot + ".json", save);
    }
    
    public static Save GetSave(int saveSlot)
    {
        dynamic saveData = ReadJson("/saves/save" + saveSlot + ".json");
        Save save = new Save();
        save.MapState = JsonConvert.DeserializeObject<State>(saveData.MapState.ToString());
        save.CurrentLevel = saveData.CurrentLevel;
        save.MapHeight = saveData.MapHeight;
        save.MapWidth = saveData.MapWidth;
        save.SaveTimeStamp = saveData.SaveTimeStamp;
        return save;
    }

}
