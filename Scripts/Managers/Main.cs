using Godot;

public partial class Main : Node
{
    [Export] public NodePath WorldPath;
    [Export] public NodePath RunHudPath;

    private WaveDirector _waveDirector;
    private Outpost _outpost;
    private RunHUD _runHud;

    public override void _Ready()
    {
        var world = GetNode<Node>(WorldPath);
        _runHud = GetNode<RunHUD>(RunHudPath);

        _waveDirector = GetNode<WaveDirector>($"{WorldPath}/WaveDirector");
        _outpost = GetNode<Outpost>($"{WorldPath}/Outpost");

        // Connect WaveDirector -> RunHUD
        _waveDirector.WaveChanged += OnWaveChanged;
        _waveDirector.WaveStateChanged += OnWaveStateChanged;
        _waveDirector.WaveTimerUpdated += OnWaveTimerUpdated;
        _waveDirector.RunFinished += OnRunFinished;

        // Connect Outpost -> RunHUD & GameOver
        _outpost.Health.HealthChanged += OnOutpostHealthChanged;
        _outpost.Health.Died += OnOutpostDied;
    }

    private void OnWaveChanged(int current, int total) => _runHud.SetWave(current, total);

    private void OnWaveStateChanged(int stateInt)
    {
        var state = (WaveState)stateInt;
        _runHud.SetStateFromEnum(state);
    }

    private void OnWaveTimerUpdated(float secondsRemaining) => _runHud.SetTimer(secondsRemaining);

    private void OnOutpostHealthChanged(float current, float max) => _runHud.SetOutpostHealth(current, max);

    private void OnOutpostDied()
    {
        _runHud.ShowGameOver("Outpost Destroyed", _waveDirector.GetCurrentWaveIndex() + 1);
        GetTree().Paused = true;
    }

    private void OnRunFinished()
    {
        _runHud.ShowGameOver("All waves completed!", _waveDirector.GetCurrentWaveIndex() + 1);
        GetTree().Paused = true;
    }
}