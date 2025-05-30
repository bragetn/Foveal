using Godot;
using System;
using System.Diagnostics;

public partial class GazeTarget : StaticBody3D, IGazeable, IGrabbable
{
    [Export] public float GazeTime { get; set; }
    [Export] public float Radius { get; set; }
    [Export] public float Delay { get; set; }
    [Export] public int Type { get; set; }
    
    public Vector3 Bounds { get; set; }
    public float ColliderSize { get; set; } = 1.0f;

    private Mesh _fullSphereMesh = GD.Load<Mesh>("uid://b02wcophph3fq");
    private Mesh _halfSphereMesh = GD.Load<Mesh>("uid://ds2vkivpml1a0");
    
    private MeshInstance3D _targetMeshInstance;
    private MeshInstance3D _colliderMeshInstance;
    private CollisionShape3D _collisionShape;
    private Timer _testTimer;

    private bool _running;
    private bool _completed;
    private float _value;
    
    private Stopwatch _stopwatch;
    private TimeSpan _timeBeforeSeen;

    private bool _isGrabbed;
    private float _pointerDistance;
    private Node3D _pointer;


    public override void _Ready()
    {
        _targetMeshInstance = GetNode<MeshInstance3D>("TargetMeshInstance3D");
        _colliderMeshInstance = GetNode<MeshInstance3D>("ColliderMeshInstance3D");
        _collisionShape = GetNode<CollisionShape3D>("CollisionShape3D");
        _testTimer = GetNode<Timer>("TestTimer");

        _testTimer.Timeout += SetRunning;
        
        UpdateGazeTarget();
    }
    
    public override void _ExitTree()
    {
        _testTimer.Timeout -= SetRunning;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_running) return;
        
        if (_isGrabbed)
        {
            GlobalPosition = _pointer.GlobalPosition - _pointer.GlobalBasis.Z * _pointerDistance;
        }
        
        Position = new Vector3(
            Mathf.Clamp(Position.X, -Bounds.X + Radius, Bounds.X - Radius),
            Mathf.Clamp(Position.Y, -Bounds.Y + Radius, Bounds.Y - Radius),
            Mathf.Clamp(Position.Z, -Bounds.Z + Radius, Bounds.Z - Radius)
        );
    }

    public void OnGazeExit()
    {
        if (!_running) return;
        if (_completed) return;
        
        _value = 0.0f;
        _targetMeshInstance.SetInstanceShaderParameter("t", 0.0f);
    }

    public void OnGazeStay(double delta)
    {
        if (!_running) return;
        
        _value += (float) delta;
        
        if (_value > GazeTime)
        {
            _value = GazeTime;
            _completed = true;
            _timeBeforeSeen = _stopwatch.Elapsed;
            _targetMeshInstance.SetInstanceShaderParameter("completed", true);
        }
        
        if (GazeTime > 0.0f)
        {
            _targetMeshInstance.SetInstanceShaderParameter("t", _value / GazeTime);
        }
    }
    
    public void OnGrabEnter(PointerUtil.PointerEvent pointerEvent)
    {
        _isGrabbed = true;
        _pointerDistance = pointerEvent.Pointer.GlobalPosition.DistanceTo(GlobalPosition);
        _pointer = pointerEvent.Pointer;
    }
    
    public void OnGrabExit()
    {
        _isGrabbed = false;
    }

    public void OnGrabStay(double delta, float value)
    {
        if (!_isGrabbed) return;
        
        _pointerDistance += (float) (value * delta);
            
        if (_pointerDistance < Radius)
        {
            _pointerDistance = Radius;
        }
    }
    
    public float GetSize()
    {
        return MathF.Log2(Radius / 0.1f) / 1.5f;
    }

    public void SetSize(float value)
    {
        // t ∈ [0.0, 1.0]
        // radius(t) = 0.1 * 2^(2t)
        Radius = 0.1f * MathF.Pow(2, value * 1.5f);
        UpdateGazeTarget();
    }

    public void SetColliderSize(float value) {
        ColliderSize = value;
        UpdateGazeTarget();
    }

    public void SetType(int value)
    {
        Type = value;
        UpdateGazeTarget();
    }

    public void Delete()
    {
        if (GetParent() is TargetBox targetBox)
        {
            targetBox.Targets.Remove(this);
        }
        QueueFree();
    }

    public GazeTargetData GetTargetData()
    {
        return new GazeTargetData
        {
            X = Position.X,
            Y = Position.Y,
            Z = Position.Z,
            Radius = Radius,
            Delay = Delay,
            Type = Type,
        };
    }

    public void StartTest(Stopwatch stopwatch)
    {
        _stopwatch = stopwatch;
        
        Visible = false;
        _collisionShape.Disabled = true;

        if (Delay > 0)
        {
            _testTimer.WaitTime = Delay;
            _testTimer.Start();
        }
        else
        {
            SetRunning();
        }
    }

    public TargetResultData StopTest()
    {
        TargetResultData result = new TargetResultData
        {
            GazeTargetData = GetTargetData(),
            HasBeenSeen = _completed,
            TimeBeforeSeen = _completed ? _timeBeforeSeen : TimeSpan.Zero,
        };
        
        _running = false;
        Visible = true;
        _collisionShape.Disabled = false;
        _value = 0.0f;
        _completed = false;
        _testTimer.Stop();
        _targetMeshInstance.SetInstanceShaderParameter("completed", false);
        _targetMeshInstance.SetInstanceShaderParameter("t", 0);
        
        return result;
    }

    private void SetRunning()
    {
        _running = true;
        Visible = true;
        _collisionShape.Disabled = false;
    }

    private void UpdateGazeTarget()
    {
        _targetMeshInstance.Scale = Vector3.One * Radius * 2.0f;
        _colliderMeshInstance.Scale = Vector3.One * Radius * 2.0f;
        
        if (_targetMeshInstance != null)
        {
            switch (Type)
            {
                case 0:
                    _targetMeshInstance.Mesh = _fullSphereMesh;
                    break;
                case 1:
                    _targetMeshInstance.Mesh = _halfSphereMesh;
                    _targetMeshInstance.Scale = new Vector3(-1, 1, 1) * Radius * 2.0f;
                    break;
                case 2:
                    _targetMeshInstance.Mesh = _halfSphereMesh;
                    break;
                default:
                    _targetMeshInstance.Mesh = _fullSphereMesh;
                    break;
            }
        }

        if (_collisionShape.GetShape().Duplicate() is SphereShape3D shape)
        {
            shape.Radius = Radius * ColliderSize;
            _collisionShape.Shape = shape;
        }
    }

}
