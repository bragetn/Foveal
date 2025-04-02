using Godot;
using System;

public partial class SaveAsMenu : Control
{
    private TextEdit _textEdit;
    private Button _saveButton;

    public override void _Ready()
    {
        _textEdit = GetNode<TextEdit>("MarginContainer/VBoxContainer/TextEdit");
        _saveButton = GetNode<Button>("MarginContainer/VBoxContainer/SaveButton");
        
        _textEdit.FocusEntered += (() => EyeTrackingRadio.Instance.EmitSignal("ShowKeyboard"));
        _textEdit.FocusExited += (() => EyeTrackingRadio.Instance.EmitSignal("HideKeyboard"));
        _saveButton.Pressed += () => EyeTrackingRadio.Instance.EmitSignal("SaveTestFile", _textEdit.Text);
    }
}
