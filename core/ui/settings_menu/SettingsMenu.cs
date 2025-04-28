using Godot;
using System;

public partial class SettingsMenu : VBoxContainer
{
    private Button _eyeTrackerCalibrationButton;

    public override void _Ready()
    {
        _eyeTrackerCalibrationButton = GetNode<Button>("MarginContainer/MenuVBox/CalibrateEyeTrackerButton");
        _eyeTrackerCalibrationButton.Pressed += CalibrateEyeTracker;
    }

    public override void _ExitTree()
    {
        _eyeTrackerCalibrationButton.Pressed -= CalibrateEyeTracker;
    }

    private void CalibrateEyeTracker()
    {
        CoreRadio.Instance.EmitSignal("CalibrateEyeTracker");
    }
}
