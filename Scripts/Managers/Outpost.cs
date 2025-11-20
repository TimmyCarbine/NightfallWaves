using Godot;

public partial class Outpost : Node2D
{

    [Export] public NodePath HealthComponentPath;
    public HealthComponent Health { get; private set; }

    public override void _Ready()
    {
        Health = GetNode<HealthComponent>(HealthComponentPath);
    }

    public void TakeDamage(float amount) => Health.ApplyDamage(amount);
}