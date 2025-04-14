using Godot;
using System;

public partial class RunTestMenu : Control
{
    private Button _openTestButton;
    private Button _runTestButton;
    private Button _toggleGazeDotButton;
    private Panel _loadTestPanel;
    private bool _running = false;
    
    public override void _Ready()
    {
        _openTestButton = GetNode<Button>("HBoxContainer/MenuPanel/VBoxContainer/OpenTestButton");
        _runTestButton = GetNode<Button>("HBoxContainer/MenuPanel/VBoxContainer/RunTestButton");
        _toggleGazeDotButton = GetNode<Button>("HBoxContainer/MenuPanel/VBoxContainer/ToggleGazeDotButton");
        _loadTestPanel = GetNode<Panel>("HBoxContainer/LoadTestPanel");

        EyeTrackingRadio.Instance.LoadTestFile += name =>
        {
            if (_running) _toggleRunning();
        };

        EyeTrackingRadio.Instance.EmitSignal("LoadTestsEditable", false);
        
        _openTestButton.Pressed += () =>
        {
            _loadTestPanel.Visible = true;
        };
        _runTestButton.Pressed += _toggleRunning;
        _toggleGazeDotButton.Pressed += () =>
        {
            _loadTestPanel.Visible = false;
            CoreRadio.Instance.EmitSignal("ToggleGazeDot");
        };
    }

    private void _toggleRunning()
    {
        _loadTestPanel.Visible = false;
        _running = !_running;
        EyeTrackingRadio.Instance.EmitSignal("PreviewTest", _running);
        _runTestButton.Text = _running ? "Avslutt" : "Start";
    }
}
