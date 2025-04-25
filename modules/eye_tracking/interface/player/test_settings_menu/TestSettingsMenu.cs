using Godot;
using System;

public partial class TestSettingsMenu : Control
{
    public TargetBox GazeTargetBox { get; set; }

    private Label _timeLabel;
    private Button _addTimeButton;
    private Button _subtractTimeButton; 
    private Slider _sizeSlider;
    private Button _toggleButton;
    private Button _exitButton;

    public override void _EnterTree()
    {
        EyeTrackingRadio.Instance.EmitSignal("AssignTargetBoxToMenu", this);
    }
    
    public override void _Ready()
    {
        _timeLabel = GetNode<Label>("MarginContainer/VBoxContainer/TimeHBox/TimeLabel");
        _addTimeButton = GetNode<Button>("MarginContainer/VBoxContainer/TimeHBox/CounterVBox/AddTimeButton");
        _subtractTimeButton = GetNode<Button>("MarginContainer/VBoxContainer/TimeHBox/CounterVBox/SubtractTimeButton");
        _sizeSlider = GetNode<Slider>("MarginContainer/VBoxContainer/SizeSlider");
        _toggleButton = GetNode<Button>("MarginContainer/VBoxContainer/ToggleButton");
        _exitButton = GetNode<Button>("MarginContainer/VBoxContainer/ExitButton");

        _addTimeButton.Pressed += () => UpdateTime(false);
        _subtractTimeButton.Pressed += () => UpdateTime(true);
        _sizeSlider.ValueChanged += UpdateColliderSize;
        _toggleButton.Pressed += ToggleColliderVisualization;
        _exitButton.Pressed += () => EyeTrackingRadio.Instance.EmitSignal("ExitTestSettings");
        
        _timeLabel.Text = GazeTargetBox.GazeTime.ToString("n2");
        _sizeSlider.Value = GazeTargetBox.ColliderSize;
    }

    private void UpdateTime(bool sign)
    {
        GazeTargetBox.GazeTime += sign ? -0.5f : 0.5f;
        GazeTargetBox.GazeTime = MathF.Max(GazeTargetBox.GazeTime, 0.0f);
        GazeTargetBox.UpdateGazeTime();

        _timeLabel.Text = GazeTargetBox.GazeTime.ToString("n2");
    }

    private void UpdateColliderSize(double value)
    {
        GazeTargetBox.ColliderSize = (float) value;
        GazeTargetBox.UpdateColliderSize();
    }

    private void ToggleColliderVisualization()
    {
        GazeTargetBox.ToggleColliderVisualization();
    }
}
