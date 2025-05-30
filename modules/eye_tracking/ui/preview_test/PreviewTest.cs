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

        EyeTrackingRadio.Instance.PreviewEnded += OnPreviewEnded;
    }

    public override void _ExitTree()
    {
        EyeTrackingRadio.Instance.PreviewEnded -= OnPreviewEnded;
    }

    private void OnPreviewEnded()
    {
        if (_running) TogglePreview();
        EyeTrackingRadio.Instance.EmitSignal("PreviewTest", false);
    }

    private void TogglePreview()
    {
        _running = !_running;
        _button.Text = _running ? "Stopp Forhåndsvisning" : "Forhåndsvis Test";
    }
}
