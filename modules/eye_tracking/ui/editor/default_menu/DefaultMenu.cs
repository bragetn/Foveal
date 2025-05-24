using Godot;
using System;

public partial class DefaultMenu : Control
{
    private Label _feedbackLabel;

    public override void _Ready()
    {
        _feedbackLabel = GetNode<Label>("FeedbackLabel");
    }

    public void SetFeedbackLabel(string feedback)
    {
        _feedbackLabel.Text = feedback;
    }
}
