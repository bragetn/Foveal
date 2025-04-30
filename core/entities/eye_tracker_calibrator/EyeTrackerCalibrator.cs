using Godot;
using System;
using System.Collections.Generic;

public partial class EyeTrackerCalibrator : Node3D
{
    [ExportGroup("Calibration Frame")]
    [Export] public Vector2 FrameSize { get; set; } = new Vector2(4, 3);
    [Export] public Vector2 FramePosition { get; set; } = new Vector2(0, 0);
    [Export] public float FrameDistance { get; set; } = 3.0f;
    [Export] public float FrameChangeSpeed { get; set; } = 0.25f;
    
    [ExportGroup("Calibration Dot")]
    [Export] public float DotRadius { get; set; } = 0.1f;
    [Export] public float DotSpeed { get; set; } = 1.0f;
    
    [ExportGroup("Assign")]
    [Export] public Camera3D Camera { get; set; }
    [Export] public XRController3D RightHandController { get; set; }
    [Export] public XRController3D LeftHandController { get; set; }

    private MeshInstance3D _calibrationFrame;
    private MeshInstance3D _calibrationDot;
    private Node3D _tutorial;
    
    private List<Vector3> _calibrationPoints;

    private bool _enabled = false;
    private bool _running = false;
    
    public override void _Ready()
    {
        _calibrationFrame = GetNode<MeshInstance3D>("CalibrationFrame");
        _calibrationDot = GetNode<MeshInstance3D>("CalibrationFrame/CalibrationDot");
        _tutorial = GetNode<Node3D>("CalibrationFrame/Viewport2Din3D");
        
        RightHandController.ButtonPressed += RightHandButtonPressed;

        CoreRadio.Instance.CalibrateEyeTracker += EnableCalibrator;
    }

    public override void _ExitTree()
    {
        CoreRadio.Instance.CalibrateEyeTracker -= EnableCalibrator;
    }

    public override void _Process(double delta)
    {
        if (!_enabled) return;
        
        if (Camera == null)
            return;
        
        Vector3 origin = Camera.GlobalTransform.Origin + Camera.GlobalBasis.Z * -FrameDistance;
        GlobalTransform = new Transform3D(Camera.GlobalBasis, origin);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_enabled) return;
        
        if (_running) return;
        
        Vector2 rightPrimaryInput = RightHandController.GetVector2("primary");
        Vector2 leftPrimaryInput = RightHandController.GetVector2("primary");
        
        FrameSize += FrameSize * FrameChangeSpeed * rightPrimaryInput * (float) delta;
        FrameSize = FrameSize.Clamp(Vector2.One * 2.5f, Vector2.One * 8.0f);
        
        FramePosition += FramePosition * FrameChangeSpeed * leftPrimaryInput * (float) delta;
        FramePosition = FramePosition.Clamp(Vector2.One * 10.0f, Vector2.One * 10.0f);
        UpdateFrameSize();
    }

    private void EnableCalibrator()
    {
        _enabled = true;
        _calibrationFrame.Visible = true;
        _tutorial.Visible = true;
        _calibrationFrame.SetInstanceShaderParameter("enabled", true);
        UpdateFrameSize();
    }

    private List<Vector3> GenerateCalibrationPoints()
    {
        List<Vector3> points = new List<Vector3>();
        points.Add(Vector3.Zero);
        points.Add(Vector3.Up * (FrameSize.Y / 2.0f - DotRadius - 0.1f));
        points.Add(Vector3.Down * (FrameSize.Y / 2.0f - DotRadius - 0.1f));
        points.Add(Vector3.Left * (FrameSize.X / 2.0f - DotRadius - 0.1f));
        points.Add(Vector3.Right * (FrameSize.X / 2.0f - DotRadius - 0.1f));
        points.Add(new Vector3((-FrameSize.X / 2.0f) * 0.68f + DotRadius, (FrameSize.Y / 2.0f) * 0.68f - DotRadius, 0.0f));
        points.Add(new Vector3((-FrameSize.X / 2.0f) * 0.68f + DotRadius, (-FrameSize.Y / 2.0f) * 0.68f + DotRadius, 0.0f));
        points.Add(new Vector3((FrameSize.X / 2.0f) * 0.68f - DotRadius, (FrameSize.Y / 2.0f) * 0.68f - DotRadius, 0.0f));
        points.Add(new Vector3((FrameSize.X / 2.0f) * 0.68f - DotRadius, (-FrameSize.Y / 2.0f) * 0.68f + DotRadius, 0.0f));
        return points;
    }

    private void UpdateFrameSize()
    {
        if (_calibrationFrame.Mesh is QuadMesh quadMesh)
        {
            quadMesh.Size = FrameSize;
        }
    }

    private void RightHandButtonPressed(string name)
    {
        if (name == "ax_button")
        {
            _running = true;
            _calibrationPoints = GenerateCalibrationPoints();
            _calibrationFrame.SetInstanceShaderParameter("enabled", false);
            _tutorial.Visible = false;
            _calibrationDot.Visible = true;
            Calibrate(0);
        }
    }

    private void Calibrate(int i)
    {
        Timer timer = new Timer();
        timer.OneShot = true;
        timer.WaitTime = 1.0f;
        AddChild(timer);

        if (i < _calibrationPoints.Count - 1)
        {
            timer.Timeout += () =>
            {
                MoveCalibrationDot(i + 1);
                RemoveChild(timer);
                timer.QueueFree();
            };
        }
        else
        {
            timer.Timeout += () =>
            {
                GD.Print("Calibration done");
                _calibrationDot.Visible = false;
            };
        }
        
        timer.Start();
    }

    private void MoveCalibrationDot(int i)
    {
        float t = _calibrationPoints[i - 1].DistanceTo(_calibrationPoints[i]) / DotSpeed;
        
        Tween tween = CreateTween();
        tween.TweenProperty(_calibrationDot, "position", _calibrationPoints[i], t);
        tween.TweenCallback(Callable.From(() => Calibrate(i)));
    }
}
