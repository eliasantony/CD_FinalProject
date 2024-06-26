﻿namespace libs;

using System.Linq;

public sealed class GameEngine
{
    private static GameEngine? _instance;
    private IGameObjectFactory gameObjectFactory;
    private string currentLevelName = "Level 0 - Intro";
    private string currentSubtitle = "";
    private int currentLevel = 0;
    private int totalAmountOfLevels = FileHandler.CountLevelFiles();
    private int amountOfBoxesInCurrentLevel = 0;
    private int timeRemainingInSeconds = 20;
    private bool isTimerRunning = false;

    public HashSet<State> states = new HashSet<State>();

    public static GameEngine Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameEngine();
            }
            return _instance;
        }
    }

    private GameEngine()
    {
        // INIT PROPS HERE IF NEEDED
        gameObjectFactory = new GameObjectFactory(currentLevel);
    }

    private GameObject? _focusedObject;

    private Map map = new Map();

    private List<GameObject> gameObjects = new List<GameObject>();

    public Map GetMap()
    {
        return map;
    }

    public GameObject GetFocusedObject()
    {
        return _focusedObject;
    }

    public GameObject GetObjectInFrontOfPlayer()
    {
        int playerPosX = Player.Instance.PosX;
        int playerPosY = Player.Instance.PosY;
        Direction playerDirection = Player.Instance.Direction;

        switch (playerDirection)
        {
            case Direction.Up:
                return map.Get(playerPosY - 1, playerPosX);
            case Direction.Down:
                return map.Get(playerPosY + 1, playerPosX);
            case Direction.Left:
                return map.Get(playerPosY, playerPosX - 1);
            case Direction.Right:
                return map.Get(playerPosY, playerPosX + 1);
            default:
                return null;
        }
    }

    public void CheckWinCondition()
    {
        if (IsLevelCompleted())
        {
            // Trigger win condition
            Console.WriteLine("You win!");
            // For now just quit before proceeding to next level
            Environment.Exit(0);
        }
    }

    public void RemoveGameObject(GameObject gameObject)
    {
        gameObjects.Remove(gameObject);
    }

    public void Setup()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        dynamic gameData = FileHandler.ReadJson("/levels/level" + currentLevel + ".json");

        if (gameData == null)
        {
            Console.WriteLine($"Error: Level {currentLevel} data could not be loaded.");
            return;
        }

        map.MapWidth = gameData.map.width;
        map.MapHeight = gameData.map.height;

        var gameObjectsData = ((IEnumerable<dynamic>)gameData.gameObjects).ToList();
        var boxes = gameObjectsData.Where(g => (int)g.Type == (int)GameObjectType.Box).ToList();
        int boxNumber = 1;

        foreach (var gameObject in gameObjectsData)
        {
            if ((int)gameObject.Type == (int)GameObjectType.Box)
            {
                gameObject.BoxNumber = boxNumber++;
            }
            GameObject newGameObject = CreateGameObject(gameObject);
            if (newGameObject != null)
            {
                if (newGameObject is Box)
                {
                    amountOfBoxesInCurrentLevel++;
                    LogUtility.Log($"Box added to level: {newGameObject.PosX}, {newGameObject.PosY}");
                }
                AddGameObject(newGameObject);
            }
        }

        _focusedObject = gameObjects.OfType<Player>().FirstOrDefault();

        StartTimer();
    }

    private void StartTimer()
    {
        isTimerRunning = true;
        Thread timerThread = new Thread(() =>
        {
            while (timeRemainingInSeconds > 0 && isTimerRunning)
            {
                Thread.Sleep(1000);
                timeRemainingInSeconds--;
            }

            if (timeRemainingInSeconds <= 0)
            {
                GameOver();
            }
        });

        timerThread.Start();
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    private void GameOver()
    {
        Console.WriteLine("Game Over");
        Environment.Exit(0);
    }

    public void Render()
    {
        Console.Clear();

        map.Initialize();

        PlaceGameObjects();

        for (int i = 0; i < map.MapHeight; i++)
        {
            for (int j = 0; j < map.MapWidth; j++)
            {
                DrawObject(map.Get(i, j));
            }
            int middleLine = map.MapHeight / 2;
            WriteLevelInformation(i, middleLine);
            WriteKeyBinds(i, middleLine);
            WriteSaveSlots(i, middleLine);
            Console.WriteLine();
        }
        DrawHintWindow("HINT: Maybe it helps if you interact with a box...?");
    }

    private void WriteSaveSlots(int currentLine, int middleLine)
    {
        int basePadding = 10;

        string output = currentLine switch
        {
            var x when x == middleLine - 2 => GetInformationStringAboutSaveSlot(1),
            var x when x == middleLine - 1 => GetInformationStringAboutSaveSlot(2),
            var x when x == middleLine => GetInformationStringAboutSaveSlot(3),
            var x when x == middleLine + 1 => GetInformationStringAboutSaveSlot(4),
            var x when x == middleLine + 2 => GetInformationStringAboutSaveSlot(5),
            _ => ""
        };
        Console.ForegroundColor = ConsoleColor.Yellow;
        string padding = new string(' ', basePadding);
        output = padding + output;
        Console.Write(output);
    }

    private string GetInformationStringAboutSaveSlot(int slotNumber)
    {
        if (FileHandler.SaveExists(slotNumber))
        {
            Save save = FileHandler.GetSave(slotNumber);
            return "Save slot " + slotNumber + " - Level " + save.CurrentLevel + " - " + save.SaveTimeStamp;
        }
        return "Save slot " + slotNumber + " - Empty";
    }

    private void WriteKeyBinds(int currentLine, int middleLine)
    {
        int basePadding = 80;

        string output = currentLine switch
        {
            var x when x == middleLine - 2 => "[ESC] to quit".PadLeft(basePadding),
            var x when x == middleLine - 1 => "[WASD]/[↑←↓→] to move".PadLeft(basePadding),
            var x when x == middleLine => "[R] to undo".PadLeft(basePadding - currentLevelName.Length - 6),
            var x when x == middleLine + 1 => "[NUMBER] to save to slot".PadLeft(basePadding - currentSubtitle.Length - 6),
            var x when x == middleLine + 2 => "[SHIFT + NUMBER] to load from slot".PadLeft(basePadding - 15),
            var x when x == middleLine + 3 => "[E] to interact with Object".PadLeft(basePadding),
            _ => ""
        };
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write(output);
    }

    private void WriteLevelInformation(int currentLine, int levelLine)
    {
        if (currentLine == levelLine)
        {
            if (IsLevelCompleted())
            {
                currentLevelName = "Level " + currentLevel + " - ✅ CLEAR";
                StopTimer(); // Stop timer when level is finished
            }
            DrawLevelName();
        }
        else if (currentLine == levelLine + 1)
        {
            if (GameEngine.Instance.IsLevelCompleted())
            {
                if (IsGameOver())
                {
                    currentSubtitle = "GAME OVER";
                }
                else
                {
                    currentSubtitle = "Press [ENTER] to continue";
                }
            }
            else
            {
                currentSubtitle = "Boxes left: " + amountOfBoxesInCurrentLevel;
            }
            DrawSubtitleLine();

        }
        else if (currentLine == levelLine + 2)
        {
            currentSubtitle = "Time: " + timeRemainingInSeconds + "s"; // shows the timer in line 3
            DrawSubtitleLine();
        }
    }

    public void DrawHintWindow(string hint)
    {
        int windowTop = map.MapHeight + 2;
        int windowLeft = 0;
        int windowWidth = hint.Length;
        int windowHeight = 3;

        // Draw the top border of the window
        Console.SetCursorPosition(windowLeft, windowTop);
        Console.Write(new string('-', windowWidth));

        // Draw the empty space inside the window
        for (int i = 1; i < windowHeight - 1; i++)
        {
            Console.SetCursorPosition(windowLeft, windowTop + i);
            Console.Write("|");
            Console.SetCursorPosition(windowLeft + windowWidth - 1, windowTop + i);
            Console.Write("|");
        }

        // Draw the bottom border of the window
        Console.SetCursorPosition(windowLeft, windowTop + windowHeight - 1);
        Console.Write(new string('-', windowWidth));

        // Display the hint text inside the window
        Console.SetCursorPosition(windowLeft + 1, windowTop + 1);
        Console.Write(hint);

        Console.WriteLine();
        Console.WriteLine();
    }

    public GameObject CreateGameObject(dynamic obj)
    {
        return gameObjectFactory.CreateGameObject(obj);
    }

    public void AddGameObject(GameObject gameObject)
    {
        if (gameObject != null)
        {
            gameObjects.Add(gameObject);
        }
    }

    private void PlaceGameObjects()
    {
        gameObjects.ForEach(delegate (GameObject obj)
        {
            if (obj is not Player)
            {
                map.Set(obj);
                // LogUtility.Log($"Placed {obj.GetType().Name} at ({obj.PosX}, {obj.PosY})");
            }
        });

        if (Player.Instance != null)
        {
            map.Set(Player.Instance);
            // LogUtility.Log($"Placed Player at ({Player.Instance.PosX}, {Player.Instance.PosY})");
        }
    }

    private void DrawObject(GameObject gameObject)
    {
        Console.ResetColor();

        if (gameObject != null)
        {
            Console.ForegroundColor = gameObject.Color;
            Console.Write(gameObject.CharRepresentation);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(' ');
        }
    }

    private void DrawLevelName()
    {
        Console.ForegroundColor = ConsoleColor.White;
        string padding = new string(' ', 6);
        string output = padding + currentLevelName;
        Console.Write(output);
    }

    private void DrawSubtitleLine()
    {
        Console.ForegroundColor = ConsoleColor.White;
        string padding = new string(' ', 6);
        string output = padding + currentSubtitle;
        Console.Write(output);
    }

    public void RemoveFromBoxesInCurrentLevel()
    {
        amountOfBoxesInCurrentLevel--;
    }

    public void IncreaseLevel()
    {
        if (IsLevelCompleted() && !IsGameOver())
        {
            currentLevel++;
            if (currentLevel >= totalAmountOfLevels)
            {
                Console.WriteLine("Congratulations! You have completed all levels.");
                Environment.Exit(0);
            }
            else
            {
                currentLevelName = "Level " + currentLevel;
                states.Clear();
                gameObjects.Clear();
                amountOfBoxesInCurrentLevel = 0;

                // Set time based on level
                switch (currentLevel)
                {
                    case 1:
                        timeRemainingInSeconds = 60;
                        break;
                    case 2:
                        timeRemainingInSeconds = 120;
                        break;
                    case 3:
                        timeRemainingInSeconds = 180;
                        break;
                    default:
                        timeRemainingInSeconds = 60;
                        break;
                }

                Setup();
            }
        }
    }


    public void SaveState(State stateToSave)
    {
        states.Add(stateToSave);
    }

    public void UndoMove()
    {
        if (states.Count > 0 && !GameEngine.Instance.IsLevelCompleted())
        {
            gameObjects.Clear();
            amountOfBoxesInCurrentLevel = 0;
            State state = states.Last();
            states.Remove(state);
            foreach (var savedGameObject in state.gameObjects)
            {
                GameObject gameObject = gameObjectFactory.LoadGameObject(savedGameObject);
                if (gameObject is Box && gameObject.PosX != -1 && gameObject.PosY != -1)
                {
                    amountOfBoxesInCurrentLevel++;
                }

                AddGameObject(gameObject);
            }

            Player.Instance.PosX = state.playerPosX;
            Player.Instance.PosY = state.playerPosY;
            AddGameObject(Player.Instance);
            _focusedObject = Player.Instance;
        }
    }

    public bool IsGameOver()
    {
        return currentLevel >= totalAmountOfLevels - 1 && IsLevelCompleted();
    }

    public bool IsLevelCompleted()
    {
        return amountOfBoxesInCurrentLevel == 0;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void SaveGame(int saveSlot)
    {
        Save save = new Save(GetCurrentState(), currentLevel, map.MapHeight, map.MapWidth, DateTime.Now);
        FileHandler.SaveGame(save, saveSlot);
    }

    public State GetCurrentState()
    {
        return new State(Player.Instance.PosX, Player.Instance.PosY, gameObjects);
    }

    public void LoadGame(int saveSlot)
    {
        if (FileHandler.SaveExists(saveSlot))
        {
            Save save = FileHandler.GetSave(saveSlot);
            currentLevel = save.CurrentLevel;
            currentLevelName = "Level " + currentLevel;
            states.Clear();
            gameObjects.Clear();
            amountOfBoxesInCurrentLevel = 0;
            foreach (dynamic savedGameObject in save.MapState.gameObjects)
            {
                GameObject gameObject = gameObjectFactory.CreateGameObject(savedGameObject);
                if (gameObject is Box && gameObject.PosX != -1 && gameObject.PosY != -1)
                {
                    amountOfBoxesInCurrentLevel++;
                }

                AddGameObject(gameObject);
            }

            Player.Instance.PosX = save.MapState.playerPosX;
            Player.Instance.PosY = save.MapState.playerPosY;
            AddGameObject(Player.Instance);
            _focusedObject = Player.Instance;
            map.MapHeight = save.MapHeight;
            map.MapWidth = save.MapWidth;
            SaveState(GetCurrentState());
        }
    }

    public void MainMenu()
    {
        Console.Clear();
        Console.WriteLine("===== ESCAPE ROOM GAME =====");
        Console.WriteLine("1. START");
        Console.WriteLine("2. LOAD");
        Console.WriteLine("3. EXIT");
        Console.Write("Select an option: ");

        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                StartGame();
                break;
            case "2":
                LoadMenu();
                break;
            case "3":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid choice, please try again.");
                MainMenu();
                break;
        }
    }

    private void StartGame()
    {
        currentLevel = 0;
        Setup();
    }

    private void LoadMenu()
    {
        Console.Clear();
        Console.WriteLine("Select a save slot to load:");
        for (int i = 1; i <= 5; i++)
        {
            Console.WriteLine($"{i}. {GetInformationStringAboutSaveSlot(i)}");
        }
        Console.Write("Select a slot: ");
        if (int.TryParse(Console.ReadLine(), out int slot) && slot >= 1 && slot <= 5)
        {
            LoadGame(slot);
            Render();
        }
        else
        {
            Console.WriteLine("Invalid choice, please try again.");
            LoadMenu();
        }
    }

    public static void Main(string[] args)
    {
        GameEngine.Instance.MainMenu();
    }
}
