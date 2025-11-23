using Godot;

public partial class Enemy : CharacterBody2D
{
    [Export] public float Speed = 20f;
    private Node2D _outpost;

    public override void _Ready()
    {
        _outpost = GetNode<Node2D>("/root/Main/World/Outpost");
    }

    public override void _Process(double delta)
    {
        if (_outpost == null) return;
        Vector2 dir = (_outpost.GlobalPosition - GlobalPosition).Normalized();
        Velocity = dir * Speed;
        MoveAndSlide();
    }
}