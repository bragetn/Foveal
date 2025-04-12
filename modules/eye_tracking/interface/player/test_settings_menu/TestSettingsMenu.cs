using Godot;
using System;

public partial class TestSettingsMenu : Control
{
    private Label _timeLabel;
    private Button _addTimeButton;
    private Button _subtractTimeButton; 
    private Slider _sizeSlider;
    private Button _toggleButton;
    private Button _exitButton;

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
    }

    private void UpdateTime(bool sign)
    {
        // Target.Delay += sign ? -0.5f : 0.5f;
        // Target.Delay = MathF.Max(Target.Delay, 0.0f);
        // _timeLabel.Text = Target.Delay.ToString("n2");
    }

    private void UpdateColliderSize(double value)
    {
        // Target.SetSize((float) value);
    }

    private void ToggleColliderVisualization()
    {
        return;
    }


}
