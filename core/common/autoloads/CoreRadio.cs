using Godot;
using System;

public partial class CoreRadio : Node
{
    [Signal] public delegate void LoadSceneEventHandler(String path);
    
    [Signal] public delegate void ResetPlayerPositionEventHandler();
    [Signal] public delegate void ToggleGazeDotEventHandler();
    [Signal] public delegate void AddTargetEventHandler();
    
    [Signal] public delegate void AssignTargetToMenuEventHandler(TargetMenu targetMenu);
    [Signal] public delegate void ExitTargetMenuEventHandler();
    
    public static CoreRadio Instance { get; private set; }
    
    public override void _Ready()
    {
        Instance = this;
    }
}
