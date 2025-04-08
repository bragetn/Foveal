using Godot;
using System;
using System.Text.RegularExpressions;

public partial class SaveAsMenu : Control
{
    private TextEdit _textEdit;
    private Button _saveButton;
    private Label _feedbackLabel;

    public override void _Ready()
    {
        _textEdit = GetNode<TextEdit>("MarginContainer/VBoxContainer/TextEdit");
        _saveButton = GetNode<Button>("MarginContainer/VBoxContainer/SaveButton");
        _feedbackLabel = GetNode<Label>("MarginContainer/VBoxContainer/FeedbackLabel");
        
        _textEdit.FocusEntered += (() => EyeTrackingRadio.Instance.EmitSignal("ShowKeyboard"));
        _textEdit.FocusExited += (() => EyeTrackingRadio.Instance.EmitSignal("HideKeyboard"));
        _saveButton.Pressed += HandleSave;
    }

    private void HandleSave()
    {
        string fileName = _textEdit.Text;

        if (IsValidFileName(fileName))
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                _feedbackLabel.Text = "Filnavnet kan ikke være tomt.";
            }
            else
            {
                EyeTrackingRadio.Instance.EmitSignal("SaveTestFile", _textEdit.Text);
            }
        }
        else
        {
            _feedbackLabel.Text = "Filnavnet er ikke akseptert.";
        }
    }
    
    private static bool IsValidFileName(string fileName)
    {
        string pattern = @"[<>;#)\\/\[\]:\""\|?*]";
        return !Regex.IsMatch(fileName, pattern);
    }
}
