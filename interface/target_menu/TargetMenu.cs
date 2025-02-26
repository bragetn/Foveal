using Godot;
using System;
using System.Globalization;

public partial class TargetMenu : Control
{
    public GazeTarget Target { get; set; }
    
    private Label _delayLabel;
    private Button _addDelayButton;
    private Button _subtractDelayButton;
    private Button _deleteTargetButton;
    private Button _exitButton;

    public override void _EnterTree()
    {
        Radio.Instance.EmitSignal("AssignTargetToMenu", this);
    }

    public override void _Ready()
    {
        _delayLabel = GetNode<Label>("MarginContainer/VBoxContainer/DelayHBox/DelayLabel");
        _addDelayButton = GetNode<Button>("MarginContainer/VBoxContainer/DelayHBox/CounterVBox/AddDelayButton");
        _subtractDelayButton = GetNode<Button>("MarginContainer/VBoxContainer/DelayHBox/CounterVBox/SubtractDelayButton");
        _deleteTargetButton = GetNode<Button>("MarginContainer/VBoxContainer/DeleteTargetButton");
        _exitButton = GetNode<Button>("MarginContainer/VBoxContainer/ExitButton");

        _addDelayButton.Pressed += () => UpdateDelay(false);
        _subtractDelayButton.Pressed += () => UpdateDelay(true);
        _deleteTargetButton.Pressed += DeleteTarget;
        _exitButton.Pressed += () => Radio.Instance.EmitSignal("ExitTargetMenu");
        
        _delayLabel.Text = Target.Delay.ToString("n2");
    }

    private void UpdateDelay(bool sign)
    {
        Target.Delay += sign ? -0.5f : 0.5f;
        Target.Delay = MathF.Max(Target.Delay, 0.0f);
        _delayLabel.Text = Target.Delay.ToString("n2");
    }

    private void DeleteTarget()
    {
        Radio.Instance.EmitSignal("ExitTargetMenu");
        Target.QueueFree();
    }
}
