using System;
using System.Security.Cryptography.X509Certificates;
using Godot;

public partial class PlayerController : CharacterBody2D
{
    // === MOVEMENT SETTINGS ===
    [Export] public float MoveSpeed          { get; set; } = 350f;  // target horizontal speed
    [Export] public float Acceleration       { get; set; } = 2000f; // how fast player reaches MoveSpeed
    [Export] public float Friction           { get; set; } = 2500f; // how fast player slows when no input

    [Export] public float Gravity            { get; set; } = 2000f;
    [Export] public float MaxFallSpeed       { get; set; } = 2000f;

    [Export] public float JumpForce          { get; set; } = 700f;  // initial jump impulse
    [Export] public float CoyoteTime         { get; set; } = 0.1f;  // seconds after leaving ground player can still jump
    [Export] public float JumpBufferTime     { get; set; } = 0.1f;  // seconds jump press is queued

    [Export] public float VariableJumpCut    { get; set; } = 0.5f;  // 0-1 how much to cut upward velocity when jump is released

    // === NODE PATHS ===
    [Export] private NodePath _spritePath;
    [Export] private NodePath _gunPath;

    private Sprite2D _sprite;
    private Node2D _gun;

    // === INTERNAL STATE ===
    private float _coyoteCurrentTime = 0f;
    private float _jumpBufferCurrentTime = 0f;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>(_spritePath);
        _gun = GetNode<Node2D>(_gunPath);
    }

    public override void _Process(double delta)
    {
        HandleJumpBuffer((float)delta);
        HandleShooting();
        AimGunAtMouse();
        FlipSpriteToMatchAim();
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        Vector2 velocity = Velocity;

        // === GRAVITY ===
        if (!IsOnFloor())
        {
            velocity.Y += Gravity * dt;
            if (velocity.Y > MaxFallSpeed) velocity.Y = MaxFallSpeed;

            _coyoteCurrentTime -= dt;
        }
        else
        {
            // Reset coyote timer every time player touches the ground
            _coyoteCurrentTime = CoyoteTime;

            // Ensure player doesn't accumulate tiny downward velocities on ground
            if (velocity.Y > 0) velocity.Y = 0;
        }

        // === HORIZONTAL MOVEMENT ===
        float inputX = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        float targetSpeed = inputX * MoveSpeed;

        float speedDiff = targetSpeed - velocity.X;
        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? Acceleration : Friction;
        float movement = Mathf.Clamp(speedDiff, -accelRate * dt, accelRate * dt);

        velocity.X += movement;

        // === JUMPING ===
        bool canUseCoyote = _coyoteCurrentTime > 0f;
        bool hasBufferedJump = _jumpBufferCurrentTime > 0f;

        if (hasBufferedJump && canUseCoyote)
        {
            velocity.Y = -JumpForce;
            _jumpBufferCurrentTime = 0f;
            _coyoteCurrentTime = 0f;
        }

        // Variable Jump Height
        if (!IsOnFloor() && !Input.IsActionPressed("jump") && velocity.Y < 0f)
        {
            velocity.Y *= 1f - Mathf.Clamp(VariableJumpCut, 0f, 1f);
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    // === INPUT HELPERS ===
    private void HandleJumpBuffer(float dt)
    {
        // If player presses jump, start/restart the buffer timer
        if (Input.IsActionJustPressed("jump")) _jumpBufferCurrentTime = JumpBufferTime;

        // Countdown buffer if active
        if (_jumpBufferCurrentTime > 0f) _jumpBufferCurrentTime -= dt;
    }

    private void HandleShooting()
    {
        if (Input.IsActionJustPressed("shoot")) Shoot();
    }

    private void AimGunAtMouse()
    {
        if (_gun == null) return;

        Vector2 mousePos = GetGlobalMousePosition();
        Vector2 toMouse = mousePos - _gun.GlobalPosition;

        if (toMouse.LengthSquared() > 0.0001f) _gun.Rotation = toMouse.Angle();
    }

    private void FlipSpriteToMatchAim()
    {
        if (_sprite == null || _gun == null) return;

        float angle = Mathf.Wrap(_gun.Rotation, -Mathf.Pi, Mathf.Pi);
        bool facingLeft = (angle > Mathf.Pi / 2f || angle < -Mathf.Pi / 2f);

        _sprite.FlipH = facingLeft;
    }

    private void Shoot()
    {
        
    }
}