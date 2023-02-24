using System;
using Godot;

public partial class ScreenLogMessage : PanelContainer
{
    private static int _count;

    public double Lifetime { get; private set; } = 5d;
    public double FadeTime { get; private set; } = 1d;

    public ScreenLogMessage()
    {
        _count++;
    }

    public ScreenLogMessage WithText(string message)
    {
        GetNode<Label>("%Label").Text = message;
        return this;
    }

    public ScreenLogMessage WithLifetime(double lifetime)
    {
        Lifetime = lifetime;
        return this;
    }

    public ScreenLogMessage WithFadeTime(double fadeTime)
    {
        FadeTime = fadeTime;
        return this;
    }

    public override void _Ready()
    {
        GetNode<Label>("%Count").Text = _count.ToString();

        GetTree().CreateTimer(Lifetime).Timeout += () =>
        {
            CreateTween()
                .TweenProperty(this, "modulate", new Color(0f, 0f, 0f, 0f), FadeTime)
                .Finished += QueueFree;
        };
    }
}
