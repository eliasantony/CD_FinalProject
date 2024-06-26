namespace libs;

public class Box : GameObject
{
    private Dialog _dialog;

    public Box(Dialog dialog) : base()
    {
        _dialog = dialog;
        Type = GameObjectType.Box;
        CharRepresentation = '■';
        Color = ConsoleColor.DarkGreen;
    }

    public override void Interact()
    {
        _dialog.Start();
        // Logic to clear the box if the dialog is completed successfully
        if (_dialog.IsCompletedSuccessfully)
        {
            LogUtility.Log("Correct answer given. Removing box...");
            GameEngine.Instance.RemoveFromBoxesInCurrentLevel();
            GameEngine.Instance.GetMap().Set(new Floor { PosX = PosX, PosY = PosY });
            GameEngine.Instance.RemoveGameObject(this);
            GameEngine.Instance.Render();
        }
        else
        {
            LogUtility.Log("Incorrect answer. Box remains.");
        }
    }
}