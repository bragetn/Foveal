using Godot;
using System;

public partial class EyeTrackingRadio : Node
{
    // Player Menu
    [Signal] public delegate void ResetPlayerPositionEventHandler();
    [Signal] public delegate void AddTargetEventHandler();
    [Signal] public delegate void AssignTargetToMenuEventHandler(TargetMenu targetMenu);
    [Signal] public delegate void AssignTargetBoxToMenuEventHandler(TestSettingsMenu settingsMenu);
    [Signal] public delegate void ExitTargetMenuEventHandler();
    [Signal] public delegate void EnterTestSettingsEventHandler();
    [Signal] public delegate void ExitTestSettingsEventHandler();
    
    // Editor Menu
    [Signal] public delegate void SaveTestFileEventHandler(string fileName);
    [Signal] public delegate void LoadTestFileEventHandler(string fileName);
    [Signal] public delegate void RenameTestFileEventHandler(string fileName);
    [Signal] public delegate void RenameTestFileToEventHandler(string fileName, string newName);
    [Signal] public delegate void DeleteTestFileEventHandler(string fileName);
    [Signal] public delegate void DeleteTestFileForRealEventHandler(string fileName);
    [Signal] public delegate void ClearTargetsEventHandler();

    // Keyboard
    [Signal] public delegate void ShowKeyboardEventHandler();
    [Signal] public delegate void HideKeyboardEventHandler();
    
    // Preview Test
    [Signal] public delegate void PreviewTestEventHandler(bool running);
    [Signal] public delegate void PreviewEndedEventHandler();
    
    public static EyeTrackingRadio Instance { get; private set; }
    
    public override void _Ready()
    {
        Instance = this;
    }
}
