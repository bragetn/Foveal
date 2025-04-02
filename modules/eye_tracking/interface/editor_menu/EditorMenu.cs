using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public partial class EditorMenu : Control
{
    private Button _newButton;
    private Button _openButton;
    private Button _saveButton;
    private Button _saveAsButton;
    
    private Button _mainMenuButton;
    private Panel _rightMenu;

    private Node _currentScene;
    
    PackedScene _saveAsPackedScene = GD.Load<PackedScene>("uid://bm3fu54guqawa");
    PackedScene _loadTestPackedScene = GD.Load<PackedScene>("uid://bed0c47cubn6q");
    PackedScene _confirmationPackedScene = GD.Load<PackedScene>("uid://bxtuspc7tq5ja"); 
    
    public override void _Ready()
    {
        _newButton = GetNode<Button>("HBoxContainer/ButtonPanel/MarginContainer/VBoxContainer/NewButton");
        _openButton = GetNode<Button>("HBoxContainer/ButtonPanel/MarginContainer/VBoxContainer/OpenButton");
        _saveButton = GetNode<Button>("HBoxContainer/ButtonPanel/MarginContainer/VBoxContainer/SaveButton");
        _saveAsButton = GetNode<Button>("HBoxContainer/ButtonPanel/MarginContainer/VBoxContainer/SaveAsButton");
        _mainMenuButton = GetNode<Button>("HBoxContainer/ButtonPanel/MarginContainer/VBoxContainer/MainMenuButton");
        _rightMenu = GetNode<Panel>("HBoxContainer/Panel2");

        _newButton.Pressed += NewTest;
        _openButton.Pressed += OpenTest;
        _saveButton.Pressed += SaveTest;
        _saveAsButton.Pressed += SaveAsTest;
        _mainMenuButton.Pressed += ExitToMainMenu;
    }

    private void NewTest()
    {
        SwitchRightMenu(_confirmationPackedScene);

        if (_currentScene is ConfirmationMenu confirmationMenu)
        {
            confirmationMenu.SetPrompt("Er du sikker?");
            confirmationMenu.ConfirmationYes += () => EyeTrackingRadio.Instance.EmitSignal("ClearTargets");
            confirmationMenu.ConfirmationNo += () => GD.Print("NO!");
        }
    }

    private void OpenTest()
    {
        SwitchRightMenu(_loadTestPackedScene);
    }
    
    private void SaveTest()
    {
        EyeTrackingRadio.Instance.EmitSignal("SaveTestFile", "");
    }

    private void SaveAsTest()
    {
        SwitchRightMenu(_saveAsPackedScene);
    }

    private void ExitToMainMenu()
    {
        SwitchRightMenu(_confirmationPackedScene);

        if (_currentScene is ConfirmationMenu confirmationMenu)
        {
            confirmationMenu.SetPrompt("Er du sikker?");
            confirmationMenu.ConfirmationYes += () => CoreRadio.Instance.EmitSignal("LoadScene", "uid://bcskthtw74py2");
            confirmationMenu.ConfirmationNo += () => GD.Print("NO!");
        }
    }

    private void SwitchRightMenu(PackedScene newMenuScene)
    {
        _currentScene?.QueueFree();
        _currentScene = newMenuScene.Instantiate();
        _rightMenu.AddChild(_currentScene);
    }
}
