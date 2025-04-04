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
    }

    private void NewTest()
    {
        SetSidePanel(_confirmationPackedScene);

        if (_currentScene is ConfirmationMenu confirmationMenu)
        {
            confirmationMenu.SetPrompt("Er du sikker?");
            confirmationMenu.ConfirmationYes += () =>
            {
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
        EyeTrackingRadio.Instance.EmitSignal("SaveTestFile", "");
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
            confirmationMenu.SetPrompt("Er du sikker?");
            confirmationMenu.ConfirmationYes += () => CoreRadio.Instance.EmitSignal("LoadScene", "uid://bcskthtw74py2");
            confirmationMenu.ConfirmationNo += () => SetSidePanel(_defaultPackedScene);
        }
    }

    private void SetSidePanel(PackedScene newSidePanelMenu)
    {
        _currentScene?.QueueFree();
        _currentScene = newSidePanelMenu.Instantiate();
        _sidePanel.AddChild(_currentScene);
    }
}
