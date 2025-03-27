using Godot;
using System;

public partial class CoreRadio : Node
{
    [Signal] public delegate void LoadSceneEventHandler(String path);
    [Signal] public delegate void ToggleGazeDotEventHandler();
    [Signal] public delegate void GrabEnteredEventHandler();
    [Signal] public delegate void GrabExitedEventHandler();
    
    public static CoreRadio Instance { get; private set; }
    
    public override void _Ready()
    {
        Instance = this;
    }
}
