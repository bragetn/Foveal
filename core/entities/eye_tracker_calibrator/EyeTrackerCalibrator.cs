using Godot;
using System;

[Tool]
public partial class EyeTrackerCalibrator : Node3D
{
    [Export] public Vector2 FrameSize { get; set; } = new Vector2(4, 3);
    [Export] public float FrameDistance { get; set; } = 5.0f;
    [Export] public float FrameChangeSpeed { get; set; } = 0.25f;
    [Export] public Camera3D Camera { get; set; }
    [Export] public XRController3D HandController { get; set; }

    private MeshInstance3D _calibrationFrame;
    
    public override void _Ready()
    {
        _calibrationFrame = GetNode<MeshInstance3D>("CalibrationFrame");

        HandController.ButtonPressed += HandButtonPressed;

        UpdateFrameSize();
    }

    public override void _Process(double delta)
    {
        if (Camera == null)
            return;
        
        Vector3 origin = Camera.GlobalTransform.Origin + Camera.GlobalBasis.Z * -FrameDistance;
        GlobalTransform = new Transform3D(Camera.GlobalBasis, origin);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint()) return;
        
        Vector2 primaryInput = HandController.GetVector2("primary");
        
        FrameSize += FrameSize * FrameChangeSpeed * primaryInput * (float) delta;
        FrameSize = FrameSize.Clamp(Vector2.One * 2.5f, Vector2.One * 8.0f);
        UpdateFrameSize();
    }

    private void UpdateFrameSize()
    {
        if (_calibrationFrame.Mesh is QuadMesh quadMesh)
        {
            quadMesh.Size = FrameSize;
        }
    }

    private void HandButtonPressed(string name)
    {
        if (name == "ax_button")
        {
            GD.Print("AX");
        }
    }
}
