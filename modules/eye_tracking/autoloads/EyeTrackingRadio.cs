using Godot;
using System;

public partial class EyeTrackingRadio : Node
{
    
    [Signal] public delegate void ResetPlayerPositionEventHandler();
    [Signal] public delegate void AddTargetEventHandler();
    [Signal] public delegate void AssignTargetToMenuEventHandler(TargetMenu targetMenu);
    [Signal] public delegate void ExitTargetMenuEventHandler();
    [Signal] public delegate void SaveTestFileEventHandler(String fileName);
    [Signal] public delegate void LoadTestFileEventHandler(String filePath);
    [Signal] public delegate void ClearTargetsEventHandler();
    [Signal] public delegate void ShowKeyboardEventHandler();
    [Signal] public delegate void HideKeyboardEventHandler();
    
    [Signal] public delegate void PreviewTestEventHandler(bool running);
    [Signal] public delegate void PreviewEndedEventHandler();
    
    public static EyeTrackingRadio Instance { get; private set; }
    
    public override void _Ready()
    {
        Instance = this;
    }
}
