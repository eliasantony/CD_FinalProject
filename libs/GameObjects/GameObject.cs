namespace libs;

public class GameObject : IGameObject, IMovement
{
    private char _charRepresentation = '#';
    private ConsoleColor _color;

    private int _posX;
    private int _posY;
    
    private int _prevPosX;
    private int _prevPosY;

    public GameObjectType Type;

    public GameObject() {
        this._posX = 5;
        this._posY = 5;
        this._color = ConsoleColor.Gray;
    }

    public GameObject(int posX, int posY){
        this._posX = posX;
        this._posY = posY;
    }

    public GameObject(int posX, int posY, ConsoleColor color){
        this._posX = posX;
        this._posY = posY;
        this._color = color;
    }

    public char CharRepresentation
    {
        get { return _charRepresentation ; }
        set { _charRepresentation = value; }
    }

    public ConsoleColor Color
    {
        get { return _color; }
        set { _color = value; }
    }

    public int PosX
    {
        get { return _posX; }
        set { _posX = value; }
    }

    public int PosY
    {
        get { return _posY; }
        set { _posY = value; }
    }

    public int GetPrevPosY() {
        return _prevPosY;
    }
    
    public int GetPrevPosX() {
        return _prevPosX;
    }

    public int Move(int dx, int dy)
    {
        State savedState = GameEngine.Instance.GetCurrentState();
        GameObject fieldToMoveOn = GameEngine.Instance.GetMap().Get(_posY + dy, _posX + dx);
        if( fieldToMoveOn is Obstacle)
        {
            return 0;
        }
        /*
        if (fieldToMoveOn is Box && this is Player)
        {
            Box box = (Box)fieldToMoveOn;
            if(box.Move(dx, dy) == 0)
            {
                return 0;
            }
        }
        if(fieldToMoveOn is BoxGoal && this is Box)
        {
            BoxOnGoal boxOnGoal = new BoxOnGoal();
            boxOnGoal.PosX = fieldToMoveOn.PosX;
            boxOnGoal.PosY = fieldToMoveOn.PosY;
            GameEngine.Instance.AddGameObject(boxOnGoal);
            
            BoxGoal boxGoal = (BoxGoal)fieldToMoveOn;
            boxGoal.Remove();
            _prevPosX = _posX;
            _prevPosY = _posY;
            Remove();
            GameEngine.Instance.RemoveFromBoxesInCurrentLevel();
            return 1;
        }
        */
        _prevPosX = _posX;
        _prevPosY = _posY;
        _posX += dx;
        _posY += dy;
        if(this is Player)
        {
            if (dx == 1)
                Player.Instance.Direction = Direction.Right;
            else if (dx == -1)
                Player.Instance.Direction = Direction.Left;
            else if (dy == 1)
                Player.Instance.Direction = Direction.Down;
            else if (dy == -1)
                Player.Instance.Direction = Direction.Up;
            
            GameEngine.Instance.SaveState(savedState);
        }
        return 1;
    }

    public void Remove()
    {
        _posX = -1;
        _posY = -1;
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
