using Godot;
using System;

public partial class Radio : Node
{
    // Player Menu
    [Signal] public delegate void ResetPlayerPositionEventHandler();
    [Signal] public delegate void ToggleGazeDotEventHandler();
    [Signal] public delegate void AddTargetEventHandler();
    
    // Target Menu
    [Signal] public delegate void AssignTargetToMenuEventHandler(TargetMenu targetMenu);
    [Signal] public delegate void ExitTargetMenuEventHandler();
    
    public static Radio Instance { get; private set; }
    
    public override void _Ready()
    {
        Instance = this;
    }
}
