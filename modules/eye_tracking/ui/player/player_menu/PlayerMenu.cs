using Godot;
using System;

public partial class PlayerMenu : Control
{
    private Button _addTargetButton;
    private Button _toggleGazeDotButton;
    private Button _resetPlayerPositionButton;
    private Button _testSettingsButton;
    
    public override void _Ready()
    {
        _addTargetButton = GetNode<Button>("MarginContainer/VBoxContainer/AddTargetButton");
        _toggleGazeDotButton = GetNode<Button>("MarginContainer/VBoxContainer/ToggleGazeDotButton");
        _resetPlayerPositionButton = GetNode<Button>("MarginContainer/VBoxContainer/ResetPlayerPositionButton");
        _testSettingsButton = GetNode<Button>("MarginContainer/VBoxContainer/TestSettingsButton");

        _resetPlayerPositionButton.Pressed += () => EyeTrackingRadio.Instance.EmitSignal("ResetPlayerPosition");
        _toggleGazeDotButton.Pressed += () => CoreRadio.Instance.EmitSignal("ToggleGazeDot");
        _addTargetButton.Pressed += () => EyeTrackingRadio.Instance.EmitSignal("AddTarget");
        _testSettingsButton.Pressed += () => EyeTrackingRadio.Instance.EmitSignal("EnterTestSettings");
    }
    
}
