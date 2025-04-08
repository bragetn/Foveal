using Godot;
using System;

public partial class ConfirmationMenu : Control
{
    [Signal] public delegate void ConfirmationYesEventHandler();
    [Signal] public delegate void ConfirmationNoEventHandler();
    
    private Label _promptLabel;
    private Button _yesButton;
    private Button _noButton;
    
    public override void _Ready()
    {
        _promptLabel = GetNode<Label>("MarginContainer/VBoxContainer/PromptLabel");
        _yesButton = GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer/YesButton");
        _noButton = GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer/NoButton");
        
        _yesButton.Pressed += () => EmitSignal("ConfirmationYes");
        _noButton.Pressed += () => EmitSignal("ConfirmationNo");
    }

    public void SetPrompt(string prompt)
    {
        _promptLabel.Text = prompt;
    }
}
