namespace libs;

public interface IGameObjectFactory
{
    public GameObject CreateGameObject(dynamic obj);
    public GameObject LoadGameObject(dynamic obj);
    public void SetCurrentLevel(int level);
}