using System;

namespace libs
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class Player : GameObject
    {
        private static Player _instance;

        public static Player Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Player();
                }
                return _instance;
            }
        }

        private Direction _direction;
        
        private Player() : base()
        {
            Type = GameObjectType.Player;
            // CharRepresentation = '☻';
            Direction = Direction.Down;
            Color = ConsoleColor.DarkYellow;
        }

        public Direction Direction
        {
            get { return _direction; }
            set { _direction = value; UpdateCharRepresentation(); }
        }

        private void UpdateCharRepresentation()
        {
            switch (_direction)
            {
                case Direction.Up:
                    CharRepresentation = '▲';
                    break;
                case Direction.Down:
                    CharRepresentation = '▼';
                    break;
                case Direction.Left:
                    CharRepresentation = '◄';
                    break;
                case Direction.Right:
                    CharRepresentation = '►';
                    break;
            }
        }

        public static void ResetPlayer()
        {
            _instance = null;
        }
    }
}