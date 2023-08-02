using System.Collections.Generic;
using Godot;

public partial class Hud : Control
{
    [Export] private PackedScene _screenLogMessageScene;
    [Export] private VBoxContainer _screenLog;

    public void AddMessage(string message)
    {
        Queue<Node> children = new(_screenLog.GetChildren());

        while (children.Count > 10)
        {
            children.Dequeue().QueueFree();
        }

        _screenLog.AddChild(
            _screenLogMessageScene.Instantiate<ScreenLogMessage>()
                                  .WithText(message)
        );
    }
}
