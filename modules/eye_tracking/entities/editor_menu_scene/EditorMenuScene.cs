using Godot;
using System;

public partial class EditorMenuScene : Node3D
{
    private Node3D _keyboard;
    public override void _Ready()
    {
        _keyboard = GetNode<Node3D>("VirtualKeyboard");
        
        EyeTrackingRadio.Instance.ShowKeyboard += () => _keyboard.Visible = true;
        EyeTrackingRadio.Instance.HideKeyboard += () => _keyboard.Visible = false;
    }
}
