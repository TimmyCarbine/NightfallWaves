using Godot;

[GlobalClass]
public partial class EnemyGroupConfig : Resource
{
    [Export] public PackedScene EnemyScene { get; set; }        // Scene for this type of enemy
    [Export] public int Count { get; set; } = 5;                // Number of this type of enemy to spawn during wave
    [Export] public float SpawnInterval { get; set; } = 0.05f;  // Seconds between spawns for this group (e.g 0.5 = two per second)
    [Export] public bool SpawnOnLeft { get; set; } = true;      // For both sides, two groups need to be created set to different sides
}