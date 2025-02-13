using Godot;
using System;

public partial class GazeTarget : StaticBody3D
{
    [Export] public float Seconds = 1.0f;

    private MeshInstance3D _meshInstance;
    private float _value;
    private float _valueDelta;
    private bool _completed;


    public override void _Ready()
    {
        _meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
    }

    public override void _PhysicsProcess(double delta)
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

    public void AddValue(float value)
    {
        if (_completed) return;
        _valueDelta = value;
    }
}
