using Godot;
using System;

public partial class GazeTarget : StaticBody3D, IGazeable, IGrabbable
{
    [Export] public float GazeTime { get; set; }
    [Export] public float Radius { get; set; }
    [Export] public float Delay { get; set; }
    
    public Vector3 Bounds { get; set; }
    public float ColliderSize { get; set; } = 1.0f;
    
    private MeshInstance3D _meshInstance;
    private CollisionShape3D _collisionShape;
    private Timer _testTimer;

    private bool _running;
    private bool _completed;
    private float _value;

    private bool _isGrabbed;
    private float _pointerDistance;
    private Node3D _pointer;


    public override void _Ready()
    {
        _meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
        _collisionShape = GetNode<CollisionShape3D>("CollisionShape3D");
        _testTimer = GetNode<Timer>("TestTimer");

        _testTimer.Timeout += SetRunning;
        
        UpdateSize();
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
        if (_completed) return;
        _value = 0.0f;
        _meshInstance.SetInstanceShaderParameter("t", 0.0f);
    }

    public void OnGazeStay(double delta)
    {
        if (!_running) return;
        
        _value += (float) delta;
        
        if (_value > GazeTime)
        {
            _value = GazeTime;
            _completed = true;
            _meshInstance.SetInstanceShaderParameter("completed", true);
        }
        
        if (GazeTime > 0.0f)
        {
            _meshInstance.SetInstanceShaderParameter("t", _value / GazeTime);
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
    
    public void UpdatePointerDistance(float value)
    {
        if (!_isGrabbed) return;
        
        _pointerDistance += value;
            
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
        UpdateSize();
    }

    public void SetColliderSize(float value) {
        ColliderSize = value;
        UpdateSize();
    }

    public void Delete()
    {
        if (GetParent() is TargetBox targetBox)
        {
            targetBox.Targets.Remove(this);
        }
        QueueFree();
    }

    public void StartTest()
    {
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

    public void StopTest()
    {
        _running = false;
        Visible = true;
        _collisionShape.Disabled = false;
        _value = 0.0f;
        _completed = false;
        _testTimer.Stop();
        _meshInstance.SetInstanceShaderParameter("completed", false);
        _meshInstance.SetInstanceShaderParameter("t", 0);
    }

    private void SetRunning()
    {
        _running = true;
        Visible = true;
        _collisionShape.Disabled = false;
    }

    private void UpdateSize()
    {
        if (_meshInstance.GetMesh().Duplicate() is SphereMesh mesh)
        {
            mesh.Radius = Radius;
            mesh.Height = Radius * 2.0f;
            _meshInstance.Mesh = mesh;
        }

        if (_collisionShape.GetShape().Duplicate() is SphereShape3D shape)
        {
            shape.Radius = Radius * ColliderSize;
            _collisionShape.Shape = shape;
        }
    }

}
