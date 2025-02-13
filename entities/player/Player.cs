using Godot;
using System;
using System.Diagnostics;

public partial class Player : XROrigin3D
{
    private XRCamera3D _camera;
    private XRController3D _eyes;
    private Node3D _target;
    
    public override void _Ready()
    {
        _camera = GetNode<XRCamera3D>("XRCamera3D");
        _eyes = GetNode<XRController3D>("Eyes");
        _target = GetNode<Node3D>("Eyes/MeshInstance3D");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 viewDir = _target.GlobalPosition - _camera.GlobalPosition;

        var spaceState = GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(_camera.GlobalPosition, _target.GlobalPosition);
        var result = spaceState.IntersectRay(query);

        if (result.Count <= 0) return;
        
        var collider = result["collider"].As<Node>();
        if (collider is GazeTarget gazeTarget)
        {
            gazeTarget.AddValue((float) delta);
        }
    }
}
