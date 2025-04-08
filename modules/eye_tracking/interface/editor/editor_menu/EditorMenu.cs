using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public partial class EditorMenu : Control
{
    private PackedScene _defaultPackedScene = GD.Load<PackedScene>("uid://bo5e8fm3iq5bk");
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

        EyeTrackingRadio.Instance.SaveTestFile += name =>
        {
            SetSidePanel(_defaultPackedScene);
            if (_currentScene is DefaultMenu defaultMenu)
            {
                _testName = name;
                defaultMenu.SetFeedbackLabel("\"" + name + "\" ble lagret!");
            }
        };

        EyeTrackingRadio.Instance.LoadTestFile += name =>
        {
            SetSidePanel(_defaultPackedScene);
            if (_currentScene is DefaultMenu defaultMenu)
            {
                _testName = name;
                defaultMenu.SetFeedbackLabel("\"" + name + "\" ble lastet inn!");
            }
        };

        EyeTrackingRadio.Instance.RenameTestFile += name =>
        {
            SetSidePanel(_saveAsPackedScene);
            if (_currentScene is SaveAsMenu saveAsMenu)
            {
                saveAsMenu.SetPreviousName(name);
            }
        };
        
        EyeTrackingRadio.Instance.RenameTestFileTo += (name, newName) =>
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
        };

        EyeTrackingRadio.Instance.DeleteTestFile += name =>
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
        };
        
        SetSidePanel(_defaultPackedScene);
    }

    private void NewTest()
    {
        SetSidePanel(_confirmationPackedScene);

        if (_currentScene is ConfirmationMenu confirmationMenu)
        {
            confirmationMenu.SetPrompt("Er du sikker på at du vil lage en ny test?");
            confirmationMenu.ConfirmationYes += () =>
            {
                _testName = "";
                EyeTrackingRadio.Instance.EmitSignal("ClearTargets");
                SetSidePanel(_defaultPackedScene);
            };
            confirmationMenu.ConfirmationNo += () => SetSidePanel(_defaultPackedScene);
        }
    }

    private void OpenTest()
    {
        SetSidePanel(_loadTestPackedScene);
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
            confirmationMenu.ConfirmationYes += () => CoreRadio.Instance.EmitSignal("LoadScene", "uid://bcskthtw74py2");
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
}
