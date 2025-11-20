using Godot;

public partial class RunHUD : CanvasLayer
{
    [Export] public NodePath OutpostHealthBarPath;
    [Export] public NodePath PlayerHealthBarPath;
    [Export] public NodePath WaveLabelPath;
    [Export] public NodePath TimerLabelPath;
    [Export] public NodePath StateLabelPath;

    [Export] public NodePath GameOverPanelPath;
    [Export] public NodePath ReasonLabelPath;
    [Export] public NodePath FinalWaveLabelPath;
    [Export] public NodePath RestartButtonPath;

    private TextureProgressBar _outpostHealthBar;
    private TextureProgressBar _playerHealthBar;
    private Label _waveLabel, _timerLabel, _stateLabel;

    private Control _gameOverPanel;
    private Label _reasonLabel, _finalWaveLabel;
    private Button _restartButton;

    public override void _Ready()
    {
        _outpostHealthBar = GetNode<TextureProgressBar>(OutpostHealthBarPath);
        _playerHealthBar = GetNode<TextureProgressBar>(PlayerHealthBarPath);
        _waveLabel = GetNode<Label>(WaveLabelPath);
        _timerLabel = GetNode<Label>(TimerLabelPath);
        _stateLabel = GetNode<Label>(StateLabelPath);

        _gameOverPanel = GetNode<Control>(GameOverPanelPath);
        _reasonLabel = GetNode<Label>(ReasonLabelPath);
        _finalWaveLabel = GetNode<Label>(FinalWaveLabelPath);
        _restartButton = GetNode<Button>(RestartButtonPath);

        _gameOverPanel.Visible = false;

        _restartButton.Pressed += OnRestartButtonPressed;
    }

    public void SetWave(int current, int total) => _waveLabel.Text = $"Wave: {current}/{total}";
    public void SetTimer(float secondsRemaining) => _timerLabel.Text = $"Time: {Mathf.CeilToInt(secondsRemaining)}s";
    public void SetStateText(string text) => _stateLabel.Text = text;
    public void SetStateFromEnum(WaveState state) => SetStateText(state.ToString());
    public void SetOutpostHealth(float current, float max)
    {
        _outpostHealthBar.MaxValue = max;
        _outpostHealthBar.Value = current;
    }
    public void SetPlayerHealth(float current, float max)
    {
        if (_playerHealthBar == null) return;
        _playerHealthBar.MaxValue = max;
        _playerHealthBar.Value = current;
    }
    public void ShowGameOver(string reason, int waveReached)
    {
        _gameOverPanel.Visible = true;
        _reasonLabel.Text = reason;
        _finalWaveLabel.Text = $"Wave Reached: {waveReached}";
    }

    private void OnRestartButtonPressed() => GetTree().ReloadCurrentScene();
}