using Godot;
using System;

public partial class EditorMenuScene : Node3D
{
    private Node3D _keyboard;
    public override void _Ready()
    {
        _keyboard = GetNode<Node3D>("VirtualKeyboard");
        
        EyeTrackingRadio.Instance.ShowKeyboard += OnShowKeyboard;
        EyeTrackingRadio.Instance.HideKeyboard += OnHideKeyboard;
    }

    public override void _ExitTree()
    {
        EyeTrackingRadio.Instance.HideKeyboard -= OnHideKeyboard;
        EyeTrackingRadio.Instance.ShowKeyboard -= OnShowKeyboard;
    }

    private void OnShowKeyboard()
    {
        _keyboard.Visible = true;
    }

    private void OnHideKeyboard()
    {
        _keyboard.Visible = false;
    }
}
