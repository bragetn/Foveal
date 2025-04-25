using Godot;
using System;
using System.Text.RegularExpressions;

public partial class RunTestMenu : Control
{
    private Button _openTestButton;
    private Button _runTestButton;
    private Button _mainMenuButton;
    private Button _toggleGazeDotButton;
    private Button _cancelButton;
    private Button _saveResultButton;
    private HSlider _distanceSlider;
    private LineEdit _gazeTimeEdit;
    private LineEdit _testNameEdit;
    private Panel _loadTestPanel;
    private Panel _testResultPanel;
    private bool _running = false;
    
    public override void _Ready()
    {
        _openTestButton = GetNode<Button>("HBoxContainer/MenuPanel/MarginContainer/VBoxContainer/OpenTestButton");
        _runTestButton = GetNode<Button>("HBoxContainer/MenuPanel/MarginContainer/VBoxContainer/RunTestButton");
        _mainMenuButton = GetNode<Button>("HBoxContainer/MenuPanel/MarginContainer/VBoxContainer/MainMenuButton");
        _toggleGazeDotButton = GetNode<Button>("HBoxContainer/MenuPanel/MarginContainer/VBoxContainer/ToggleGazeDotButton");
        _cancelButton = GetNode<Button>("TestResultPanel/MarginContainer/PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/CancelButton");
        _saveResultButton = GetNode<Button>("TestResultPanel/MarginContainer/PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/SaveButton");
        _distanceSlider = GetNode<HSlider>("HBoxContainer/MenuPanel/MarginContainer/VBoxContainer/DistanceSlider");
        _testNameEdit = GetNode<LineEdit>("TestResultPanel/MarginContainer/PanelContainer/MarginContainer/VBoxContainer/TestNameEdit");
        _gazeTimeEdit = GetNode<LineEdit>("HBoxContainer/MenuPanel/MarginContainer/VBoxContainer/GazeTimeEdit");
        _loadTestPanel = GetNode<Panel>("HBoxContainer/LoadTestPanel");
        _testResultPanel = GetNode<Panel>("TestResultPanel");
        
        EyeTrackingRadio.Instance.StartTest += () => _runTestButton.Disabled = false;
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
        _distanceSlider.ValueChanged += value =>
        {
            EyeTrackingRadio.Instance.EmitSignal("ChangePlayerDistance", value);
        };
        _cancelButton.Pressed += () => _testResultPanel.Visible = false;
        _saveResultButton.Pressed += () =>
        {
            if (IsValidFileName(_testNameEdit.Text))
            {
                _testResultPanel.Visible = false;
                EyeTrackingRadio.Instance.EmitSignal("SaveTestResults", _testNameEdit.Text);
                _testNameEdit.Text = "";
            }
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

        if (_running)
        {
            _running = false;
            _runTestButton.Text = "Start";
            _testResultPanel.Visible = true;
            EyeTrackingRadio.Instance.EmitSignal("StopTest");
        }
        else
        {
            _running = true;
            _runTestButton.Text = "Avslutt";
            _runTestButton.Disabled = true;
            _testResultPanel.Visible = false;
            EyeTrackingRadio.Instance.EmitSignal("StartCountdownTimer", 3);
        }
    }
    
    private static bool IsValidFileName(string fileName)
    {
        string pattern = @"[<>;#)\\/\[\]:\""\|?*]";
        return !Regex.IsMatch(fileName, pattern);
    }
}
