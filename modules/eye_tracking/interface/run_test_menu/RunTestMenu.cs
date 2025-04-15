using Godot;
using System;

public partial class RunTestMenu : Control
{
    private Button _openTestButton;
    private Button _runTestButton;
    private Button _mainMenuButton;
    private Button _toggleGazeDotButton;
    private HSlider _distanceSlider;
    private LineEdit _gazeTimeEdit;
    private Panel _loadTestPanel;
    private bool _running = false;
    
    public override void _Ready()
    {
        _openTestButton = GetNode<Button>("HBoxContainer/MenuPanel/MarginContainer/VBoxContainer/OpenTestButton");
        _runTestButton = GetNode<Button>("HBoxContainer/MenuPanel/MarginContainer/VBoxContainer/RunTestButton");
        _mainMenuButton = GetNode<Button>("HBoxContainer/MenuPanel/MarginContainer/VBoxContainer/MainMenuButton");
        _toggleGazeDotButton = GetNode<Button>("HBoxContainer/MenuPanel/MarginContainer/VBoxContainer/ToggleGazeDotButton");
        _distanceSlider = GetNode<HSlider>("HBoxContainer/MenuPanel/MarginContainer/VBoxContainer/DistanceSlider");
        _gazeTimeEdit = GetNode<LineEdit>("HBoxContainer/MenuPanel/MarginContainer/VBoxContainer/GazeTimeEdit");
        _loadTestPanel = GetNode<Panel>("HBoxContainer/LoadTestPanel");

        EyeTrackingRadio.Instance.LoadTestFile += OnLoadTestFile;
        EyeTrackingRadio.Instance.EmitSignal("LoadTestsEditable", false);
        
        _openTestButton.Pressed += () =>
        {
            _loadTestPanel.Visible = true;
        };
        _runTestButton.Pressed += ToggleRunning;
        _toggleGazeDotButton.Pressed += () =>
        {
            _loadTestPanel.Visible = false;
            CoreRadio.Instance.EmitSignal("ToggleGazeDot");
        };
        _mainMenuButton.Pressed += () =>
        {
            CoreRadio.Instance.EmitSignal("LoadScene", "uid://bcskthtw74py2");
        };
        _gazeTimeEdit.TextChanged += text =>
        {
            //if (float.TryParse(text, out float gazeTime))
            //{
            //    EyeTrackingRadio.Instance.EmitSignal("SetGazeTime", gazeTime);
            //}
        };
    }

    public override void _ExitTree()
    {
        EyeTrackingRadio.Instance.LoadTestFile -= OnLoadTestFile;
    }

    private void OnLoadTestFile(string name)
    {
        if (_running) ToggleRunning();
    }

    private void ToggleRunning()
    {
        _loadTestPanel.Visible = false;
        _running = !_running;
        EyeTrackingRadio.Instance.EmitSignal("PreviewTest", _running);
        _runTestButton.Text = _running ? "Avslutt" : "Start";
    }
}
