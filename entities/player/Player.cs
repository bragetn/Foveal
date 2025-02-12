using Godot;
using System;
using System.Diagnostics;

[Tool]
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

        if (Engine.IsEditorHint())
        {
            DebugDraw3D.DrawRay(_camera.GlobalPosition, viewDir, 1.0f);
        }
        else
        {
            var spaceState = GetWorld3D().DirectSpaceState;
            var query = PhysicsRayQueryParameters3D.Create(_camera.GlobalPosition, _target.GlobalPosition);
            var result = spaceState.IntersectRay(query);

            if (result.Count > 0)
            {
                var collider = result["collider"].As<Node>();
                if (collider is GazeTarget gazeTarget)
                {
                    gazeTarget.ChangeColor();
                }
            }

        }
    }
}
