using Godot;
using System;

public partial class ModuleMenu : VBoxContainer
{
    private Button _testEditorButton;
    private Button _runTestButton;

    public override void _Ready()
    {
        _testEditorButton = GetNode<Button>("MarginContainer/MenuVBox/TestEditorButton");
        _runTestButton = GetNode<Button>("MarginContainer/MenuVBox/RunTestButton");

        _testEditorButton.Pressed += () => CoreRadio.Instance.EmitSignal("LoadScene", "uid://bgohwu2kxayoc");
        _runTestButton.Pressed += () => CoreRadio.Instance.EmitSignal("LoadScene", "uid://c0c1k3sfkkta4");
        
    }
}
