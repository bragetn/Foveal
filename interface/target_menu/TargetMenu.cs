using Godot;
using System;

public partial class TargetMenu : Control
{
    
    private Button _exitButton;
    
    public override void _Ready()
    {
        _exitButton = GetNode<Button>("MarginContainer/VBoxContainer/ExitButton");

        _exitButton.Pressed += () => Radio.Instance.EmitSignal("ExitTargetMenu");
    }
}
