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
            CoreRadio.Instance.EmitSignal("LoadScene", "uid://bgohwu2kxayoc");
        };
    }
}
