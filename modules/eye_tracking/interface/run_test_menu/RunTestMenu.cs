using Godot;
using System;
using System.Text.RegularExpressions;

public partial class RunTestMenu : Control
{
    private Button _runTestButton;
    private Button _openTestButton;
    private Button _toggleGazeDotButton;
    private Button _toggleColliderVisualizationButton;
    private Button _mainMenuButton;
    
    private LineEdit _testNameEdit;
    private HSlider _distanceSlider;
    private HSlider _colliderSizeSlider;
    
    private Button _cancelButton;
    private Button _saveResultButton;
    private LineEdit _gazeTimeEdit;
    private Panel _loadTestPanel;
    private Panel _testResultPanel;
    
    
    private bool _running = false;
    
    public override void _Ready()
    {
        _runTestButton = GetNode<Button>("HBoxContainer/MainPanel/MarginContainer/VBoxContainer/RunTestButton");
        _openTestButton = GetNode<Button>("HBoxContainer/MainPanel/MarginContainer/VBoxContainer/OpenTestButton");
        _toggleGazeDotButton = GetNode<Button>("HBoxContainer/MainPanel/MarginContainer/VBoxContainer/ToggleGazeDotButton");
        _toggleColliderVisualizationButton = GetNode<Button>("HBoxContainer/MainPanel/MarginContainer/VBoxContainer/ToggleColliderVisualizationButton");
        _mainMenuButton = GetNode<Button>("HBoxContainer/MainPanel/MarginContainer/VBoxContainer/MainMenuButton");
        
        _gazeTimeEdit = GetNode<LineEdit>("SettingsPanel/MarginContainer/VBoxContainer/GazeTimeEdit");
        _distanceSlider = GetNode<HSlider>("SettingsPanel/MarginContainer/VBoxContainer/DistanceSlider");
        _colliderSizeSlider = GetNode<HSlider>("SettingsPanel/MarginContainer/VBoxContainer/ColliderSizeSlider");
        
        _cancelButton = GetNode<Button>("TestResultPanel/MarginContainer/PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/CancelButton");
        _saveResultButton = GetNode<Button>("TestResultPanel/MarginContainer/PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/SaveButton");
        _testNameEdit = GetNode<LineEdit>("TestResultPanel/MarginContainer/PanelContainer/MarginContainer/VBoxContainer/TestNameEdit");
        _loadTestPanel = GetNode<Panel>("HBoxContainer/LoadTestPanel");
        _testResultPanel = GetNode<Panel>("TestResultPanel");

        EyeTrackingRadio.Instance.StartTest += StartTest;
        EyeTrackingRadio.Instance.LoadTestFile += OnLoadTestFile;
        EyeTrackingRadio.Instance.SetTestParameters += SetTestParameters;
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
        EyeTrackingRadio.Instance.StartTest -= StartTest;
        EyeTrackingRadio.Instance.LoadTestFile -= OnLoadTestFile;
        EyeTrackingRadio.Instance.SetTestParameters -= SetTestParameters;
    }
    
    private void StartTest()
    {
        _runTestButton.Disabled = false;
    }

    private void OnLoadTestFile(string name)
    {
        if (_running) ToggleRunning();
    }

    private void SetTestParameters(float gazeTime, float colliderSize)
    {
        _gazeTimeEdit.Text = gazeTime.ToString();
        _colliderSizeSlider.Value = colliderSize;
    }

    private void ToggleRunning()
    {
        _loadTestPanel.Visible = false;

        if (_running)
        {
            _running = false;
            _runTestButton.Text = "Start";
            _testResultPanel.Visible = true;
            
            _runTestButton.Disabled = false;
            _openTestButton.Disabled = false;
            _toggleGazeDotButton.Disabled = false;
            _toggleColliderVisualizationButton.Disabled = false;
            _mainMenuButton.Disabled = false;
            
            _gazeTimeEdit.Editable = true;
            _gazeTimeEdit.SelectingEnabled = true;
            _gazeTimeEdit.FocusMode = FocusModeEnum.Click;
            _distanceSlider.Editable = true;
            _colliderSizeSlider.Editable = true;
            
            EyeTrackingRadio.Instance.EmitSignal("StopTest");
        }
        else
        {
            _running = true;
            _runTestButton.Text = "Avslutt";
            _testResultPanel.Visible = false;
            
            _runTestButton.Disabled = true;
            _openTestButton.Disabled = true;
            _toggleGazeDotButton.Disabled = true;
            _toggleColliderVisualizationButton.Disabled = true;
            _mainMenuButton.Disabled = true;
            
            _gazeTimeEdit.Editable = false;
            _gazeTimeEdit.SelectingEnabled = false;
            _gazeTimeEdit.FocusMode = FocusModeEnum.None;
            _distanceSlider.Editable = false;
            _colliderSizeSlider.Editable = false;
            
            EyeTrackingRadio.Instance.EmitSignal("StartCountdownTimer", 3);
        }
    }
    
    private static bool IsValidFileName(string fileName)
    {
        string pattern = @"[<>;#)\\/\[\]:\""\|?*]";
        return !Regex.IsMatch(fileName, pattern);
    }
}
