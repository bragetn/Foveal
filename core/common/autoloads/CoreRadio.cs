using Godot;
using System;

public partial class CoreRadio : Node
{
    [Signal] public delegate void LoadSceneEventHandler(string path);
    [Signal] public delegate void ToggleGazeDotEventHandler();
    [Signal] public delegate void GrabEnteredEventHandler(Node grabbedNode);
    [Signal] public delegate void GrabExitedEventHandler();
    [Signal] public delegate void ToggleAdminModeEventHandler();
    [Signal] public delegate void CalibrateEyeTrackerEventHandler();
    
    public bool AdminMode = false;
    public static CoreRadio Instance { get; private set; }
    
    public override void _Ready()
    {
        Instance = this;
        ToggleAdminMode += () => AdminMode = !AdminMode;
    }
}
