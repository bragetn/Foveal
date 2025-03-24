using Godot;
using System;

public partial class GazeTarget : StaticBody3D, IGazeable
{
    [Export] public float Seconds { get; set; } = 1.0f;
    [Export] public float Radius { get; set; } = 0.1f * MathF.Pow(2, 0.5f * 1.5f);
    [Export] public float Delay { get; set; } = 0.0f;
    
    private MeshInstance3D _meshInstance;
    private CollisionShape3D _collisionShape;
    private float _value;
    private float _valueDelta;
    private bool _completed;

    private bool _isGrabbed;
    private float _pointerDistance;
    private Node3D _pointer;


    public override void _Ready()
    {
        _meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
        _collisionShape = GetNode<CollisionShape3D>("CollisionShape3D");
        UpdateSize();
    }

    public override void _PhysicsProcess(double delta)
    {
        ProcessGrab();
        ProcessEyeGaze(delta);
    }

    public void OnGazeEnter()
    {
        GD.Print("GAZE ENTER");
    }

    public void OnGazeExit()
    {
        GD.Print("GAZE EXIT");
    }

    public void OnGazeStay(double delta)
    {
        GD.Print("GAZE STAY");
    }

    public void AddValue(float value)
    {
        if (_completed) return;
        _valueDelta = value;
    }

    public void Grab(PointerUtil.PointerEvent pointerEvent)
    {
        _isGrabbed = true;
        _pointerDistance = pointerEvent.Pointer.GlobalPosition.DistanceTo(GlobalPosition);
        _pointer = pointerEvent.Pointer;
    }

    public void Release()
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
            shape.Radius = Radius;
            _collisionShape.Shape = shape;
        }
    }

    private void ProcessGrab()
    {
        if (!_isGrabbed) return;
        
        GlobalPosition = _pointer.GlobalPosition - _pointer.GlobalBasis.Z * _pointerDistance;
    }
    
    private void ProcessEyeGaze(double delta)
    {
        if (_completed) return;
        
        if (_valueDelta == 0.0f)
        {
            _value -= (float) delta;
            if (_value < 0.0f) _value = 0.0f;
        }
        else
        {
            _value += _valueDelta;
            _valueDelta = 0.0f;
        }
        
        if (_value > Seconds)
        {
            _value = Seconds;
            _completed = true;
            _meshInstance.SetInstanceShaderParameter("completed", true);
        }
        
        if (Seconds > 0.0f)
        {
            _meshInstance.SetInstanceShaderParameter("t", _value / Seconds);
        }
    }
}
