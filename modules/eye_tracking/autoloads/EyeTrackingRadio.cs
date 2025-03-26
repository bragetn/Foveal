using Godot;
using System;

public partial class EyeTrackingRadio : Node
{
    
    [Signal] public delegate void ResetPlayerPositionEventHandler();
    [Signal] public delegate void AddTargetEventHandler();
    [Signal] public delegate void AssignTargetToMenuEventHandler(TargetMenu targetMenu);
    [Signal] public delegate void ExitTargetMenuEventHandler();
    
    public static EyeTrackingRadio Instance { get; private set; }
    
    public override void _Ready()
    {
        Instance = this;
    }
}
