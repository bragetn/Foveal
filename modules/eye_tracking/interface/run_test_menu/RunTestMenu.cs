using Godot;
using System;

public partial class RunTestMenu : Control
{
    private Button _openTestButton;
    private Button _runTestButton;
    private LoadTestMenu _loadTestMenu;
    private bool _running = false;
    
    public override void _Ready()
    {
        _openTestButton = GetNode<Button>("HBoxContainer/VBoxContainer/OpenTestButton");
        _runTestButton = GetNode<Button>("HBoxContainer/VBoxContainer/RunTestButton");
        _loadTestMenu = GetNode<LoadTestMenu>("HBoxContainer/LoadTestMenu");
        
        _openTestButton.Pressed += () =>
        {
            _loadTestMenu.Visible = true;
        };
        _runTestButton.Pressed += () =>
        {
            _loadTestMenu.Visible = false;
            _running = !_running;
            EyeTrackingRadio.Instance.EmitSignal("PreviewTest", _running);
            if (_running)
            {
                _runTestButton.Text = "Avslutt";
            }
            else
            {
                _runTestButton.Text = "Start";
            }
        };
    }
}
