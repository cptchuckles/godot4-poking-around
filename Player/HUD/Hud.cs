using Godot;
using Godot.Collections;

public partial class Hud : Control
{
    [Export] private PackedScene _screenLogMessage;
    [Export] private VBoxContainer _screenLog;

    public void AddMessage(string message)
    {
        Array<Node> children = _screenLog.GetChildren();

        if (children.Count > 10)
        {
            for (int i = 0; i < children.Count - 10; i++)
            {
                children[i].QueueFree();
            }
        }

        _screenLog.AddChild(
            _screenLogMessage.Instantiate<ScreenLogMessage>()
                             .WithText(message)
        );
    }
}
