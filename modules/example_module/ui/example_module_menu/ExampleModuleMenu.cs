using Godot;
using System;

public partial class ExampleModuleMenu : Control
{
    private Button _button;

    public override void _Ready()
    {
        _button = GetNode<Button>("MarginContainer/MenuVBox/TestEditorButton");
        _button.Pressed += ButtonPressed;
    }

    public override void _ExitTree()
    {
        _button.Pressed -= ButtonPressed;
    }

    private void ButtonPressed()
    {
        CoreRadio.Instance.EmitSignal("LoadScene", "uid://uhjbmjocugah");
    }
}
