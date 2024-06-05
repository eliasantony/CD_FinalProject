using System;

namespace libs;

public sealed class InputHandler{

    private static InputHandler? _instance;
    private GameEngine engine;

    public static InputHandler Instance {
        get{
            if(_instance == null)
            {
                _instance = new InputHandler();
            }
            return _instance;
        }
    }

    private InputHandler() {
        //INIT PROPS HERE IF NEEDED
        engine = GameEngine.Instance;
    }

    public void Handle(ConsoleKeyInfo keyInfo)
    {
        GameObject focusedObject = engine.GetFocusedObject();

        if (focusedObject != null) {
            // Handle keyboard input to move the player
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow: 
                case ConsoleKey.W:
                    focusedObject.Move(0, -1);
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    focusedObject.Move(0, 1);
                    break;
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    focusedObject.Move(-1, 0);
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    focusedObject.Move(1, 0);
                    break;
                case ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;
                case ConsoleKey.Enter:
                    GameEngine.Instance.IncreaseLevel();
                    break;
                case ConsoleKey.R:
                    GameEngine.Instance.UndoMove();
                    break;
                case ConsoleKey.D1:
                    if (keyInfo.Modifiers == ConsoleModifiers.Shift)
                    {
                        GameEngine.Instance.LoadGame(1);
                    }
                    else
                    {
                        GameEngine.Instance.SaveGame(1);
                    }
                    break;
                case ConsoleKey.D2:
                    if (keyInfo.Modifiers == ConsoleModifiers.Shift)
                    {
                        GameEngine.Instance.LoadGame(2);
                    }
                    else
                    {
                        GameEngine.Instance.SaveGame(2);
                    }
                    break;
                case ConsoleKey.D3:
                    if (keyInfo.Modifiers == ConsoleModifiers.Shift)
                    {
                        GameEngine.Instance.LoadGame(3);
                    }
                    else
                    {
                        GameEngine.Instance.SaveGame(3);
                    }
                    break;
                case ConsoleKey.D4:
                    if (keyInfo.Modifiers == ConsoleModifiers.Shift)
                    {
                        GameEngine.Instance.LoadGame(4);
                    }
                    else
                    {
                        GameEngine.Instance.SaveGame(4);
                    }
                    break;
                case ConsoleKey.D5:
                    if (keyInfo.Modifiers == ConsoleModifiers.Shift)
                    {
                        GameEngine.Instance.LoadGame(5);
                    }
                    else
                    {
                        GameEngine.Instance.SaveGame(5);
                    }
                    break;
                case ConsoleKey.E:
                    GameObject objectInFront = GameEngine.Instance.GetObjectInFrontOfPlayer();
                    if (objectInFront is Box)
                    {
                        objectInFront.Remove();
                        GameEngine.Instance.RemoveFromBoxesInCurrentLevel();
                        //GameEngine.Instance.CheckWinCondition();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}