using Godot;
using System;

public partial class EditorMenu : Control
{
    private Button _saveTestButton;
    private Button _newTestButton;
    private Button _loadTestButton;
    private Button _mainMenuButton;
    
    public override void _Ready()
    {
        _saveTestButton = GetNode<Button>("HBoxContainer/Panel/VBoxContainer/SaveTestButton");
        _newTestButton = GetNode<Button>("HBoxContainer/Panel/VBoxContainer/NewTestButton");
        _loadTestButton = GetNode<Button>("HBoxContainer/Panel/VBoxContainer/LoadTestButton");
        _mainMenuButton = GetNode<Button>("HBoxContainer/Panel/VBoxContainer/MainMenuButton");
        
        _mainMenuButton.Pressed += ExitToMainMenu;
    }

    private void ExitToMainMenu()
    {
        // Todo: Save test confirmation
        CoreRadio.Instance.EmitSignal("LoadScene", "uid://bcskthtw74py2");
    }
}
