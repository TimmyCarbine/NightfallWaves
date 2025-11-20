using System.Collections.Generic;
using Godot;

public partial class EnemySpawner : Node
{
    [Export] private NodePath _enemyContainerPath;
    [Export] private NodePath _cameraPath;
    [Export] private NodePath _outpostPath;

    [Export] public float SpawnSceenPadding { get; set; } = 64f;

    private Node2D _enemyContainer;
    private Camera2D _camera;
    private Node2D _outpost;

    private class GroupSpawnRate
    {
        public EnemyGroupConfig Config;
        public int SpawnedCount;
        public float SpawnAccumulator;
    }

    private List<GroupSpawnRate> _activeGroups = new();
    private bool _isWaveActive = false;

    public override void _Ready()
    {
        _enemyContainer = GetNode<Node2D>(_enemyContainerPath);
        _camera = GetNode<Camera2D>(_cameraPath);
        _outpost = GetNode<Node2D>(_outpostPath);
    }

    public override void _Process(double delta)
    {
        if (!_isWaveActive) return;

        float dt = (float)delta;

        for (int i = _activeGroups.Count - 1; i >= 0; i--)
        {
            var groupState = _activeGroups[i];
            groupState.SpawnAccumulator += dt;

            float interval = groupState.Config.SpawnInterval;
            if (interval <= 0f) interval = 0.1f;

            while (groupState.SpawnAccumulator >= interval && groupState.SpawnedCount < groupState.Config.Count)
            {
                groupState.SpawnAccumulator -= interval;
                SpawnEnemy(groupState.Config);
                groupState.SpawnedCount++;
            }

            if (groupState.SpawnedCount >= groupState.Config.Count) _activeGroups.RemoveAt(i);
        }
    }

    public void BeginWave(WaveConfig wave)
    {
        _activeGroups.Clear();
        _isWaveActive = true;

        if (wave == null || wave.EnemyGroups == null) return;

        foreach (var group in wave.EnemyGroups)
        {
            if (group?.EnemyScene == null) continue;

            _activeGroups.Add(new GroupSpawnRate
            {
                Config = group,
                SpawnedCount = 0,
                SpawnAccumulator = 0f
            });
        }
    }

    private void SpawnEnemy(EnemyGroupConfig config)
    {
        if (config.EnemyScene == null) return;

        var enemyInstance = config.EnemyScene.Instantiate<Node2D>();
        _enemyContainer.AddChild(enemyInstance);

        Vector2 campos = _camera.GlobalPosition;
        Rect2 viewportRect = GetViewport().GetVisibleRect();
        float halfWidth = viewportRect.Size.X * 0.5f * _camera.Zoom.X;

        float sideSign = config.SpawnOnLeft ? -1f : 1f;
        float spawnX = campos.X + sideSign * (halfWidth + SpawnSceenPadding);
        float SpawnY = _outpost.GlobalPosition.Y;

        enemyInstance.GlobalPosition = new Vector2(spawnX, SpawnY);
    }
}