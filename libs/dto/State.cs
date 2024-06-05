namespace libs;

public class State
{
    public int playerPosX;
    public int playerPosY;
    
    public List<object> gameObjects = new List<object>();
    
    public State(int playerX, int playerY, List<GameObject> gameObjectsSave)
    {
        playerPosX = playerX;
        playerPosY = playerY;
        foreach (GameObject gameObject in gameObjectsSave)
        {
            if(gameObject is not Player)
                gameObjects.Add(gameObject.Clone());
        }
    }
    
    public State()
    {
    }
}