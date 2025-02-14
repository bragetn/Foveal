using Godot;
using System;

public partial class Radio : Node
{
    [Signal]
    public delegate void ResetPlayerPositionEventHandler();
    
    public static Radio Instance { get; private set; }
    
    public override void _Ready()
    {
        Instance = this;
    }
}
