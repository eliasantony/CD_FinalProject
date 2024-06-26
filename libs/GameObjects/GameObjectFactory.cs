namespace libs;

public class GameObjectFactory : IGameObjectFactory
{
    private int currentLevel;
    private int boxCounter = 0; // Counter for box numbers

    public GameObjectFactory(int level)
    {
        currentLevel = level;
    }

    public void SetCurrentLevel(int level)
    {
        currentLevel = level;
        boxCounter = 0; // Reset the box counter for the new level
    }

    public GameObject CreateGameObject(dynamic obj)
    {
        GameObject newObj = null;
        int type = (int)obj.Type;

        switch (type)
        {
            case (int)GameObjectType.Player:
                Player.Instance.PosX = obj.PosX;
                Player.Instance.PosY = obj.PosY;
                newObj = Player.Instance;
                LogUtility.Log($"Created Player at ({obj.PosX}, {obj.PosY})");
                break;
            case (int)GameObjectType.Obstacle:
                newObj = new Obstacle
                {
                    PosX = obj.PosX,
                    PosY = obj.PosY
                };
                // LogUtility.Log($"Created Obstacle at ({obj.PosX}, {obj.PosY})");
                break;
            case (int)GameObjectType.Box:
                boxCounter++;
                string dialogPath = $"/dialogs/level{currentLevel}_box{boxCounter}.json";
                try
                {
                    Dialog dialog = Dialog.LoadFromJson(dialogPath);
                    newObj = new Box(dialog)
                    {
                        PosX = obj.PosX,
                        PosY = obj.PosY
                    };
                    LogUtility.Log($"Created Box at ({obj.PosX}, {obj.PosY}) with dialog {dialogPath}");
                }
                catch (Exception ex)
                {
                    LogUtility.Log($"Failed to load dialog: {ex.Message}");
                }
                break;
        }

        return newObj;
    }

    public GameObject LoadGameObject(dynamic obj)
    {
        int type = (int)obj.Type;

        switch (type)
        {
            case (int)GameObjectType.Player:
                Player.Instance.PosX = obj.PosX;
                Player.Instance.PosY = obj.PosY;
                LogUtility.Log($"Loaded Player at ({obj.PosX}, {obj.PosY})");
                return Player.Instance;
            case (int)GameObjectType.Obstacle:
                LogUtility.Log($"Loaded Obstacle at ({obj.PosX}, {obj.PosY})");
                return new Obstacle
                {
                    PosX = obj.PosX,
                    PosY = obj.PosY
                };
            case (int)GameObjectType.Box:
                boxCounter++;
                string dialogPath = $"/dialogs/level{currentLevel}_box{boxCounter}.json";
                try
                {
                    Dialog dialog = Dialog.LoadFromJson(dialogPath);
                    Box box = new Box(dialog)
                    {
                        PosX = obj.PosX,
                        PosY = obj.PosY
                    };
                    LogUtility.Log($"Loaded Box at ({obj.PosX}, {obj.PosY}) with dialog {dialogPath}");
                    return box;
                }
                catch (Exception ex)
                {
                    LogUtility.Log($"Failed to load dialog: {ex.Message}");
                }
                break;
        }

        return null;
    }
}