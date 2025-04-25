using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

[Tool]
public partial class TargetBox : Node3D
{
    [Export] public PackedScene TargetScene = GD.Load<PackedScene>("uid://ce5n050wc588g");
    [Export] public Vector3 BoxScale
    {
        get => _halfScale * 2.0f;
        set
        {
            _halfScale = (value / 2.0f);
            _halfScale = new Vector3(
                Mathf.Max(_halfScale.X, 0.0f),
                Mathf.Max(_halfScale.Y, 0.0f),
                Mathf.Max(_halfScale.Z, 0.0f));
            UpdateBoxScale();
        }
    }
    
    [ExportGroup("Walls")]
    [Export] public StaticBody3D LeftWall;
    [Export] public StaticBody3D RightWall;
    [Export] public StaticBody3D BackWall;
    [Export] public StaticBody3D FloorWall;
    [Export] public StaticBody3D CeilingWall;
    
    public List<GazeTarget> Targets = [];

    public float GazeTime { get; set; } = 1.0f;
    public float ColliderSize { get; set; } = 1.0f;
    public bool VisualizeCollider { get; set; } = false;

    private ShaderMaterial _colliderMaterial = GD.Load<ShaderMaterial>("uid://bubrshfvrabhd");
    
    private Stopwatch _stopwatch;
    private TestResultData _testResult;
    
    private Vector3 _halfScale = new Vector3(4.0f, 2.0f, 2.0f);
    private float _defaultRadius = 0.1f * MathF.Pow(2, 0.5f * 1.5f);


    public override void _Ready()
    {
        if (Engine.IsEditorHint()) return;

        EyeTrackingRadio.Instance.AddTarget += AddRandomTarget;
        EyeTrackingRadio.Instance.ClearTargets += ClearTargets;
        EyeTrackingRadio.Instance.StartTest += StartTest;
        EyeTrackingRadio.Instance.StopTest += StopTest;
        EyeTrackingRadio.Instance.PreviewTest += OnPreviewTest;
        EyeTrackingRadio.Instance.AssignTargetBoxToMenu += AssignTargetBoxToMenu;
        UpdateBoxScale();
    }

    public override void _ExitTree()
    {
        if (Engine.IsEditorHint()) return;
        
        EyeTrackingRadio.Instance.AddTarget -= AddRandomTarget;
        EyeTrackingRadio.Instance.ClearTargets -= ClearTargets;
        EyeTrackingRadio.Instance.StartTest -= StartTest;
        EyeTrackingRadio.Instance.StopTest -= StopTest;
        EyeTrackingRadio.Instance.PreviewTest -= OnPreviewTest;
        EyeTrackingRadio.Instance.AssignTargetBoxToMenu -= AssignTargetBoxToMenu;
    }

    private void AddRandomTarget()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        GazeTarget target = TargetScene.Instantiate<GazeTarget>();
        
        float x = rng.RandfRange(-_halfScale.X + _defaultRadius, _halfScale.X - _defaultRadius);
        float y = rng.RandfRange(-_halfScale.Y + _defaultRadius, _halfScale.Y - _defaultRadius);
        float z = rng.RandfRange(-_halfScale.Z + _defaultRadius, _halfScale.Z - _defaultRadius);
        
        target.Position = new Vector3(x, y, z);
        target.Radius = _defaultRadius;
        target.GazeTime = GazeTime;
        target.Bounds = _halfScale;
        target.ColliderSize = ColliderSize;
        
        Targets.Add(target);
        AddChild(target);
    }
    
    public void AddTarget(Vector3 targetPosition, float targetRadius, float targetDelay)
    {
        GazeTarget target = TargetScene.Instantiate<GazeTarget>();
        target.Position = targetPosition;
        target.Radius = targetRadius;
        target.Delay = targetDelay;
        target.GazeTime = GazeTime;
        target.Bounds = _halfScale;
        target.ColliderSize = ColliderSize;
        
        Targets.Add(target);
        AddChild(target);
    }

    public void ClearTargets()
    {
        foreach (GazeTarget target in Targets)
        {
            target.QueueFree();
        }
        Targets.Clear();
    }

    public TestResultData GetTestResult()
    {
        return _testResult;
    }

    public void UpdateGazeTime()
    {
        foreach (GazeTarget target in Targets)
        {
            target.GazeTime = GazeTime;
        }
    }

    public void UpdateColliderSize()
    {
        _colliderMaterial.SetShaderParameter("collider_scale", ColliderSize);

        foreach (GazeTarget target in Targets)
        {
            target.SetColliderSize(ColliderSize);
        }
    }

    public void ToggleColliderVisualization()
    {
        VisualizeCollider = !VisualizeCollider;
        _colliderMaterial.SetShaderParameter("enabled", VisualizeCollider);
    }

    private void AssignTargetBoxToMenu(TestSettingsMenu settingsMenu)
    {
        settingsMenu.TBox = this;
    }

    private void UpdateBoxScale()
    {
        Vector3 boxScale = BoxScale;
        
        UpdateWall(LeftWall, Vector3.Left * _halfScale.X, boxScale.Z, boxScale.Y);
        UpdateWall(RightWall, Vector3.Right * _halfScale.X, boxScale.Z, boxScale.Y);
        UpdateWall(BackWall, Vector3.Forward * _halfScale.Z, boxScale.X, boxScale.Y);
        UpdateWall(FloorWall, Vector3.Down * _halfScale.Y, boxScale.X, boxScale.Z);
        UpdateWall(CeilingWall, Vector3.Up * _halfScale.Y, boxScale.X, boxScale.Z);
    }

    private void UpdateWall(StaticBody3D wall, Vector3 position, float x, float y)
    {
        if (wall == null) return;
        
        wall.Position = position;
        
        if (wall.GetNode<CollisionShape3D>("CollisionShape3D").Shape is BoxShape3D shape)
        {
            shape.Size = new Vector3(x, y, 0);
        }
        
        if (wall.GetNode<MeshInstance3D>("MeshInstance3D") is MeshInstance3D meshInstance)
        {
            meshInstance.SetInstanceShaderParameter("size", new Vector2(x, y));
            if (wall.GetNode<MeshInstance3D>("MeshInstance3D").Mesh is QuadMesh mesh)
            {
                mesh.Size = new Vector2(x, y);
            }
        }
    }

    private void StartTest()
    {
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
        
        foreach (GazeTarget target in Targets)
        {
            target.StartTest(_stopwatch);
        }
    }

    private void StopTest()
    {
        _stopwatch.Stop();

        TestResultData testResult = new TestResultData
        {
            TestName = "",
            TestTime = _stopwatch.Elapsed,
            GazeTime = 0,
            PlayerDistance = 0,
            TargetResults = new List<TargetResultData>()
        };
        
        foreach (GazeTarget target in Targets)
        {
            TargetResultData targetResult = target.StopTest();
            testResult.TargetResults.Add(targetResult);
        }
        
        _testResult = testResult;
    }

    private void OnPreviewTest(bool running)
    {
        if (running)
        {
            StartTest();
        }
        else
        {
            StopTest();
        }
    }
}
