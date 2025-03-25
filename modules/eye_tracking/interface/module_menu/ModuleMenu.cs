using Godot;
using System;

public partial class ModuleMenu : VBoxContainer
{
    private Button _testEditorButton;

    public override void _Ready()
    {
        _testEditorButton = GetNode<Button>("MarginContainer/MenuVBox/TestEditorButton");

        _testEditorButton.Pressed += () =>
        {
            GD.Print("Pressing button");
            CoreRadio.Instance.EmitSignal("LoadScene", "res://core/stages/playground/playground.tscn");
        };
    }
}
