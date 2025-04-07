using Godot;
using System;

public partial class PreviewTest : Control
{
    private Button _button;
    private bool _running;
    
    public override void _Ready()
    {
        _button = GetNode<Button>("Panel/Button");
        _button.Pressed += () =>
        {
            TogglePreview();
            EyeTrackingRadio.Instance.EmitSignal("PreviewTest", _running);
        };
        
        EyeTrackingRadio.Instance.PreviewEnded += TogglePreview;
    }

    private void TogglePreview()
    {
        _running = !_running;
        _button.Text = _running ? "Stop Preview" : "Preview Test";
    }
}
