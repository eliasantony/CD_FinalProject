namespace libs;

public class Save
{
    public State MapState;
    public int CurrentLevel;
    public int MapHeight;
    public int MapWidth;
    public DateTime SaveTimeStamp;
    
    public Save( State mapState, int currentLevel, int mapHeight, int mapWidth, DateTime saveTimeStamp)
    {
        MapState = mapState;
        CurrentLevel = currentLevel;
        MapHeight = mapHeight;
        MapWidth = mapWidth;
        SaveTimeStamp = saveTimeStamp;
    }
    
    public Save()
    {
    }
}