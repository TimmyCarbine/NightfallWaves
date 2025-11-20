using Godot;

[GlobalClass]
public partial class WaveConfig : Resource
{
    // How long in seconds the player has to prepare before the wave starts
    [Export] public float PrepTime { get; set; } = 3f;
    // Maximum time in seconds the wave will last (Can end early if enemies killed)
    [Export] public float Duration { get; set; } = 20f;
    // Enemy groups that will be spawned during this wave
    [Export] public EnemyGroupConfig[] EnemyGroups { get; set; } = System.Array.Empty<EnemyGroupConfig>();
}