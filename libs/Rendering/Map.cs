namespace libs;
using Newtonsoft.Json;

public class Map
{
    private char[,] RepresentationalLayer;
    private GameObject?[,] GameObjectLayer;

    private int _mapWidth;
    private int _mapHeight;

    public Map()
    {
        _mapWidth = 30;
        _mapHeight = 8;
        RepresentationalLayer = new char[_mapHeight, _mapWidth];
        GameObjectLayer = new GameObject[_mapHeight, _mapWidth];
    }

    public Map(int width, int height)
    {
        _mapWidth = width;
        _mapHeight = height;
        RepresentationalLayer = new char[_mapHeight, _mapWidth];
        GameObjectLayer = new GameObject[_mapHeight, _mapWidth];
    }

    public void Initialize()
    {
        RepresentationalLayer = new char[_mapHeight, _mapWidth];
        GameObjectLayer = new GameObject[_mapHeight, _mapWidth];

        // Initialize the map with some default values
        for (int i = 0; i < GameObjectLayer.GetLength(0); i++)
        {
            for (int j = 0; j < GameObjectLayer.GetLength(1); j++)
            {
                GameObjectLayer[i, j] = null; // Or new Floor() if you want a default object
                RepresentationalLayer[i, j] = ' '; // Default representation
            }
        }
    }

    public int MapWidth
    {
        get { return _mapWidth; } // Getter
        set { _mapWidth = value; Initialize(); } // Setter
    }

    public int MapHeight
    {
        get { return _mapHeight; } // Getter
        set { _mapHeight = value; Initialize(); } // Setter
    }

    public GameObject Get(int x, int y)
    {
        return GameObjectLayer[x, y];
    }

    public void Set(GameObject gameObject)
    {
        int posY = gameObject.PosY;
        int posX = gameObject.PosX;

        if (posX >= 0 && posX < _mapWidth && posY >= 0 && posY < _mapHeight)
        {
            GameObjectLayer[posY, posX] = gameObject;
            RepresentationalLayer[posY, posX] = gameObject.CharRepresentation;
            LogUtility.Log($"Set {gameObject.GetType().Name} at ({posX}, {posY})");
        }
        else
        {
            LogUtility.Log($"Failed to set {gameObject.GetType().Name} at ({posX}, {posY}) - out of bounds");
        }
    }
}
