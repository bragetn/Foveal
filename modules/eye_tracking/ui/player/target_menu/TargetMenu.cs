using Godot;
using System;
using System.Globalization;

public partial class TargetMenu : Control
{
    public GazeTarget Target { get; set; }
    
    private Label _delayLabel;
    private Button _addDelayButton;
    private Button _subtractDelayButton; 
    private Slider _sizeSlider;
    private Button _deleteTargetButton;
    private Button _exitButton;
    
    private Button _type0Button;
    private Button _type1Button;
    private Button _type2Button;

    public override void _EnterTree()
    {
        EyeTrackingRadio.Instance.EmitSignal("AssignTargetToMenu", this);
    }

    public override void _Ready()
    {
        _delayLabel = GetNode<Label>("MarginContainer/VBoxContainer/DelayHBox/DelayLabel");
        _addDelayButton = GetNode<Button>("MarginContainer/VBoxContainer/DelayHBox/CounterVBox/AddDelayButton");
        _subtractDelayButton = GetNode<Button>("MarginContainer/VBoxContainer/DelayHBox/CounterVBox/SubtractDelayButton");
        _sizeSlider = GetNode<Slider>("MarginContainer/VBoxContainer/SizeSlider");
        _deleteTargetButton = GetNode<Button>("MarginContainer/VBoxContainer/DeleteTargetButton");
        _exitButton = GetNode<Button>("MarginContainer/VBoxContainer/ExitButton");
        
        _type0Button = GetNode<Button>("MarginContainer/VBoxContainer/TypeHBox/Type0Button");
        _type1Button = GetNode<Button>("MarginContainer/VBoxContainer/TypeHBox/Type1Button");
        _type2Button = GetNode<Button>("MarginContainer/VBoxContainer/TypeHBox/Type2Button");

        _addDelayButton.Pressed += () => UpdateDelay(false);
        _subtractDelayButton.Pressed += () => UpdateDelay(true);
        _deleteTargetButton.Pressed += DeleteTarget;
        _sizeSlider.ValueChanged += UpdateSize;
        _exitButton.Pressed += () => EyeTrackingRadio.Instance.EmitSignal("ExitTargetMenu");

        _type0Button.Pressed += () => UpdateType(0);
        _type1Button.Pressed += () => UpdateType(1);
        _type2Button.Pressed += () => UpdateType(2);
        
        _delayLabel.Text = Target.Delay.ToString("n2");
        _sizeSlider.Value = Target.GetSize();
    }

    private void UpdateDelay(bool sign)
    {
        Target.Delay += sign ? -0.5f : 0.5f;
        Target.Delay = MathF.Max(Target.Delay, 0.0f);
        _delayLabel.Text = Target.Delay.ToString("n2");
    }

    private void UpdateSize(double value)
    {
        Target.SetSize((float) value);
    }

    private void UpdateType(int value)
    {
        Target.SetType(value);
    }

    private void DeleteTarget()
    {
        EyeTrackingRadio.Instance.EmitSignal("ExitTargetMenu");
        Target.Delete();
    }
}
