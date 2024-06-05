namespace libs;

public class GameObjectFactory : IGameObjectFactory
{
    public GameObject CreateGameObject(dynamic obj) {

        GameObject newObj = new GameObject();
        int type = (int) obj.Type;

        switch (type)
        {
            case (int) GameObjectType.Player:
                Player.Instance.PosX = obj.PosX;
                Player.Instance.PosY = obj.PosY;
                newObj = Player.Instance;
                break;
            case (int) GameObjectType.Obstacle:
                newObj = obj.ToObject<Obstacle>();
                break;
            case (int) GameObjectType.Box:
                newObj = obj.ToObject<Box>();
                break;
        }

        return newObj;
    }
    
    public GameObject LoadGameObject(dynamic obj)
    {
        int type = (int) obj.Type;

        switch (type)
        {
            case (int) GameObjectType.Player:
                Player.Instance.PosX = obj.PosX;
                Player.Instance.PosY = obj.PosY;
                return Player.Instance;
            case (int) GameObjectType.Obstacle:
                return (Obstacle) obj;
            case (int) GameObjectType.Box:
                return (Box) obj;
        }

        return null;
    }
}