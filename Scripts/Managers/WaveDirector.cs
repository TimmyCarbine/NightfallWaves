using System;
using Godot;

public enum WaveState { Idle, Preparing, Active, Intermission, Finished }

public partial class WaveDirector : Node
{
    [Signal] public delegate void WaveChangedEventHandler(int currentWaveIndex, int totalWaves);
    [Signal] public delegate void WaveStateChangedEventHandler(int newState);
    [Signal] public delegate void WaveTimerUpdatedEventHandler(float secondsRemaing);
    [Signal] public delegate void RunFinishedEventHandler();

    [Export] public WaveConfig[] Waves { get; set; } = Array.Empty<WaveConfig>();
    [Export] public float IntermissionDuration { get; set; } = 5f;
    [Export] private NodePath _enemySpawnerPath;

    private EnemySpawner _enemySpawner;
    private int _currentWaveIndex = -1;
    private WaveState _state = 0f;
    private float _waveStateTimer = 0f;

    public override void _Ready()
    {
        if (!string.IsNullOrEmpty(_enemySpawnerPath))
        {
            _enemySpawner = GetNode<EnemySpawner>(_enemySpawnerPath);
        }
        else
        {
            _enemySpawner = GetNode<EnemySpawner>("../EnemySpawner");
        }

        StartRun();
    }

    public override void _Process(double delta)
    {
        if (_state == WaveState.Idle || _state == WaveState.Finished) return;

        _waveStateTimer -= (float)delta;
        EmitSignal(SignalName.WaveTimerUpdated, Mathf.Max(_waveStateTimer, 0f));

        if (_waveStateTimer <= 0f) AdvancedState();
    }

    public void StartRun()
    {
        if (Waves.Length == 0)
        {
            GD.PushWarning("WaveDirector: No waves configured.");
            return;
        }

        _currentWaveIndex = 0;
        StartPreparingForCurrentWave();
    }

    private void StartPreparingForCurrentWave()
    {
        _state = WaveState.Preparing;
        EmitSignal(SignalName.WaveStateChanged, (int)_state);
        EmitSignal(SignalName.WaveChanged, _currentWaveIndex + 1, Waves.Length);

        var wave = Waves[_currentWaveIndex];
        _waveStateTimer = wave.PrepTime;
    }

    private void StartActiveWave()
    {
        _state = WaveState.Active;
        EmitSignal(SignalName.WaveStateChanged, (int)_state);

        var wave = Waves[_currentWaveIndex];
        _waveStateTimer = wave.Duration;

        _enemySpawner.BeginWave(wave);
    }

    private void StartIntermission()
    {
        _state = WaveState.Intermission;
        EmitSignal(SignalName.WaveStateChanged, (int)_state);

        _waveStateTimer = IntermissionDuration;
    }

    private void AdvancedState()
    {
        switch (_state)
        {
            case WaveState.Preparing:
                StartActiveWave();
                break;

            case WaveState.Active:
                StartIntermission();
                break;

            case WaveState.Intermission:
                _currentWaveIndex++;

                if (_currentWaveIndex >= Waves.Length)
                {
                    _state = WaveState.Finished;
                    EmitSignal(SignalName.WaveStateChanged, (int)_state);
                    EmitSignal(SignalName.RunFinished);
                }
                else
                {
                    StartPreparingForCurrentWave();
                }
                break;
        }
    }

    public void OnEnemiesCleared()
    {
        if (_state == WaveState.Active && _waveStateTimer >= 3f) _waveStateTimer = 0f;
    }

    public int GetCurrentWaveIndex() => _currentWaveIndex;
    public WaveState GetState() => _state;
}