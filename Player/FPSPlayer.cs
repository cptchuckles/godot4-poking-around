using System.Text;
using Godot;

namespace PokingAround;

public partial class FPSPlayer : CharacterBody3D
{
    [Export] private float _stickDeadzone = 0.2f;
    [Export] private float _speed = 10.0f;
    [Export] private float _turnSpeed = 3f * Mathf.Pi;
    [Export] private float _jumpVelocity = 4.5f;

    [Export] private Marker3D _head;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    private readonly float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Process(double delta)
    {
        OrientateHead(delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("screenshot"))
        {
            string path = "user://screenshot.png";
            GD.Print($"Saving screenshot to {path}");
            Error err = GetViewport().GetTexture().GetImage().SavePng(path);
            if (err != Error.Ok)
            {
                GD.Print($"Couldn't save screenshot: error {err}");
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        CalculateVelocity(delta);
        if (MoveAndSlide())
        {
            WriteLogMessages();
            MoveRigidBodies();
        }
    }

    private void MoveRigidBodies()
    {
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision3D collision = GetSlideCollision(i);
            if (collision.GetCollider() is RigidBody3D rb)
            {
                rb.ApplyCentralImpulse(-collision.GetNormal());
            }
        }
    }

    private void WriteLogMessages()
    {
        Hud hud = GetNode<Hud>("HUD");

        if (hud is null)
        {
            return;
        }

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision3D collision = GetSlideCollision(i);

            Vector3 normal = collision.GetNormal();

            if (normal.IsEqualApprox(UpDirection))
            {
                continue;
            }

            float angle = Mathf.RadToDeg(normal.AngleTo(UpDirection));

            StringBuilder message = new();
            message.AppendLine($"Collision normal: {normal}")
                   .AppendLine($"Angle: {angle}");

            hud.AddMessage(message.ToString());
        }
    }

    private void OrientateHead(double delta)
    {
        Vector2 stickInput = Input.GetVector("yaw_right", "yaw_left", "pitch_down", "pitch_up", _stickDeadzone);
        stickInput *= stickInput.Length();  // Quadratic ramping
        RotateY(stickInput.X * _turnSpeed * (float)delta);
        _head.RotateX(stickInput.Y * _turnSpeed * (float)delta);
    }

    private void CalculateVelocity(double delta)
    {
        Vector3 velocity = Velocity;

        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
        {
            velocity.Y = _jumpVelocity;
        }

        velocity.Y -= _gravity * (float)delta;

        Vector2 inputDir = Input.GetVector("left", "right", "forward", "back", _stickDeadzone);
        Vector3 direction = Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);
        direction *= direction.Length();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * _speed;
            velocity.Z = direction.Z * _speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, _speed);
            velocity.Z = Mathf.MoveToward(velocity.Z, 0, _speed);
        }

        Velocity = velocity;
    }
}
