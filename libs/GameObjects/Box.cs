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
            // Update the map to remove the box
            GameEngine.Instance.GetMap().Set(new Floor { PosX = PosX, PosY = PosY });
            GameEngine.Instance.RemoveGameObject(this);
            GameEngine.Instance.RemoveFromBoxesInCurrentLevel();
            GameEngine.Instance.Render();
        }
    }
}