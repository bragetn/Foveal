using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public partial class EditorMenu : Control
{
    private PackedScene _defaultPackedScene = GD.Load<PackedScene>("uid://bo5e8fm3iq5bk");
    private PackedScene _tutorialPackedScene = GD.Load<PackedScene>("uid://bt4m7utxt37qu");
    private PackedScene _saveAsPackedScene = GD.Load<PackedScene>("uid://bm3fu54guqawa");
    private PackedScene _loadTestPackedScene = GD.Load<PackedScene>("uid://bed0c47cubn6q");
    private PackedScene _confirmationPackedScene = GD.Load<PackedScene>("uid://bxtuspc7tq5ja"); 
    
    private Button _newButton;
    private Button _openButton;
    private Button _saveButton;
    private Button _saveAsButton;
    private Button _mainMenuButton;
    private Panel _sidePanel;

    private Node _currentScene;

    private string _testName;

    public override void _Ready()
    {
        _newButton = GetNode<Button>("HBoxContainer/ButtonPanel/MarginContainer/VBoxContainer/VBoxContainer/NewButton");
        _openButton = GetNode<Button>("HBoxContainer/ButtonPanel/MarginContainer/VBoxContainer/VBoxContainer/OpenButton");
        _saveButton = GetNode<Button>("HBoxContainer/ButtonPanel/MarginContainer/VBoxContainer/VBoxContainer/SaveButton");
        _saveAsButton = GetNode<Button>("HBoxContainer/ButtonPanel/MarginContainer/VBoxContainer/VBoxContainer/SaveAsButton");
        _mainMenuButton = GetNode<Button>("HBoxContainer/ButtonPanel/MarginContainer/VBoxContainer/VBoxContainer/MainMenuButton");
        _sidePanel = GetNode<Panel>("HBoxContainer/SidePanel");

        _newButton.Pressed += NewTest;
        _openButton.Pressed += OpenTest;
        _saveButton.Pressed += SaveTest;
        _saveAsButton.Pressed += SaveAsTest;
        _mainMenuButton.Pressed += ExitToMainMenu;

        EyeTrackingRadio.Instance.SaveTestFile += OnSaveTestFile;
        EyeTrackingRadio.Instance.LoadTestFile += OnLoadTestFile;
        EyeTrackingRadio.Instance.RenameTestFile += OnRenameTestFile;
        EyeTrackingRadio.Instance.RenameTestFileTo += OnRenameTestFileTo;
        EyeTrackingRadio.Instance.DeleteTestFile += OnDeleteTestFile;
        
        SetSidePanel(_tutorialPackedScene);
    }

    public override void _ExitTree()
    {
        EyeTrackingRadio.Instance.SaveTestFile -= OnSaveTestFile;
        EyeTrackingRadio.Instance.LoadTestFile -= OnLoadTestFile;
        EyeTrackingRadio.Instance.RenameTestFile -= OnRenameTestFile;
        EyeTrackingRadio.Instance.RenameTestFileTo -= OnRenameTestFileTo;
        EyeTrackingRadio.Instance.DeleteTestFile -= OnDeleteTestFile;
    }

    private void OnSaveTestFile(string name)
    {
        SetSidePanel(_defaultPackedScene);
        if (_currentScene is DefaultMenu defaultMenu)
        {
            _testName = name;
            defaultMenu.SetFeedbackLabel("\"" + name + "\" ble lagret!");
        }
    }

    private void OnRenameTestFile(string name)
    {
        SetSidePanel(_saveAsPackedScene);
        if (_currentScene is SaveAsMenu saveAsMenu)
        {
            saveAsMenu.SetPreviousName(name);
        }
    }

    private void OnRenameTestFileTo(string name, string newName)
    {
        SetSidePanel(_defaultPackedScene);
        if (_currentScene is DefaultMenu defaultMenu)
        {
            if (_testName == name)
            {
                _testName = newName;
            }
            defaultMenu.SetFeedbackLabel("\"" + name + "\" ble endret til \"" + newName + "\".");
        }
    }

    private void OnDeleteTestFile(string name)
    {
        SetSidePanel(_confirmationPackedScene);
        if (_currentScene is ConfirmationMenu confirmationMenu)
        {
            confirmationMenu.SetPrompt("Er du sikker på at du vil slette \"" + name + "\"?");
            confirmationMenu.ConfirmationYes += () =>
            {
                EyeTrackingRadio.Instance.EmitSignal("DeleteTestFileForReal", name);
                    
                if (_testName == name)
                {
                    _testName = "";
                    EyeTrackingRadio.Instance.EmitSignal("ClearTargets");
                }
                    
                SetSidePanel(_defaultPackedScene);
                if (_currentScene is DefaultMenu defaultMenu)
                {
                    defaultMenu.SetFeedbackLabel("\"" + name + "\" ble slettet.");
                }
            };
            confirmationMenu.ConfirmationNo += () => SetSidePanel(_loadTestPackedScene);
        }
    }

    private void NewTest()
    {
        SetSidePanel(_confirmationPackedScene);

        if (_currentScene is ConfirmationMenu confirmationMenu)
        {
            confirmationMenu.SetPrompt("Er du sikker på at du vil lage en ny test?\nDette vil slette endringer som ikke er lagret.");
            confirmationMenu.ConfirmationYes += () =>
            {
                _testName = "";
                EyeTrackingRadio.Instance.EmitSignal("ClearTargets");
                SetSidePanel(_tutorialPackedScene);
            };
            confirmationMenu.ConfirmationNo += () => SetSidePanel(_tutorialPackedScene);
        }
    }

    private void OpenTest()
    {
        SetSidePanel(_loadTestPackedScene);
        EyeTrackingRadio.Instance.EmitSignal("LoadTestsEditable", true);
    }
    
    private void SaveTest()
    {
        if (string.IsNullOrEmpty(_testName))
        {
            SaveAsTest();
        }
        else
        {
            EyeTrackingRadio.Instance.EmitSignal("SaveTestFile", _testName);
        }
    }

    private void SaveAsTest()
    {
        SetSidePanel(_saveAsPackedScene);
    }

    private void ExitToMainMenu()
    {
        SetSidePanel(_confirmationPackedScene);

        if (_currentScene is ConfirmationMenu confirmationMenu)
        {
            confirmationMenu.SetPrompt("Er du sikker på at du vil gå tilbake til hovedmenyen?");
            confirmationMenu.ConfirmationYes += () =>
            {
                CoreRadio.Instance.EmitSignal("LoadScene", "uid://bcskthtw74py2");
            };
            confirmationMenu.ConfirmationNo += () => SetSidePanel(_defaultPackedScene);
        }
    }

    private void SetSidePanel(PackedScene newSidePanelMenu)
    {
        EyeTrackingRadio.Instance.EmitSignal("PreviewEnded");
        _currentScene?.QueueFree();
        _currentScene = newSidePanelMenu.Instantiate();
        _sidePanel.AddChild(_currentScene);
    }

    private void OnLoadTestFile(string name)
    {
        SetSidePanel(_defaultPackedScene);
        if (_currentScene is DefaultMenu defaultMenu)
        {
            _testName = name;
            defaultMenu.SetFeedbackLabel("\"" + name + "\" ble lastet inn!");
        }
    }
}
