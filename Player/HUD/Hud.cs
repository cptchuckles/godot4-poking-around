using System;
using Godot;

public partial class Hud : Control
{
    [Export] private PackedScene _screenLogMessage;

    public void AddMessage(string message)
    {
        var screenLog = GetNode<VBoxContainer>("ScreenLog");

        if (screenLog.GetChildren().Count > 10)
        {
            for (int i = 0; i < screenLog.GetChildren().Count - 10; i++)
            {
                screenLog.GetChild(i).QueueFree();
            }
        }

        screenLog.AddChild(_screenLogMessage.Instantiate<ScreenLogMessage>().WithText(message));
    }
}
