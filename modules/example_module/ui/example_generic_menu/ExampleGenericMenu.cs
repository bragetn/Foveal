using Godot;
using System;

public partial class ExampleGenericMenu : Control
{
    private Button _button;

    public override void _Ready()
    {
        _button = GetNode<Button>("VBoxContainer/Button");
        _button.Pressed += ButtonPressed;
    }

    public override void _ExitTree()
    {
        _button.Pressed -= ButtonPressed;
    }

    private void ButtonPressed()
    {
        GD.Print("Hello World!");
    }
}
