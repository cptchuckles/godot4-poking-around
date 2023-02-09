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

    public override void _PhysicsProcess(double delta)
    {
        CalculateVelocity(delta);
        _ = MoveAndSlide();
    }

    public override void _Process(double delta)
    {
        OrientateHead(delta);
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
            velocity.X = Mathf.MoveToward(Velocity.X, 0, _speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, _speed);
        }

        Velocity = velocity with
        {
            Y = velocity.Y - (_gravity * (float)delta),
        };
    }
}
