using Godot;
using System;

public partial class GazeTarget : StaticBody3D
{
    [Export] public float Seconds { get; set; } = 1.0f;
    [Export] public float Radius { get; set; } = 0.2f;

    private MeshInstance3D _meshInstance;
    private float _value;
    private float _valueDelta;
    private bool _completed;

    private bool _isGrabbed;
    private float _pointerDistance;
    private Node3D _pointer;


    public override void _Ready()
    {
        _meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
    }

    public override void _PhysicsProcess(double delta)
    {
        ProcessGrab();
        ProcessEyeGaze(delta);
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
