using Godot;
using System;
using System.Diagnostics;

public partial class Player : XROrigin3D
{
    private XRCamera3D _camera;
    private XRController3D _eyes;
    private XRController3D _leftHand;
    private Node3D _target;
    private Node3D _playerMenu;
    
    public override void _Ready()
    {
        _camera = GetNode<XRCamera3D>("XRCamera3D");
        _eyes = GetNode<XRController3D>("Eyes");
        _leftHand = GetNode<XRController3D>("LeftHand");
        _target = GetNode<Node3D>("Eyes/MeshInstance3D");
        _playerMenu = GetNode<Node3D>("LeftHand/PlayerMenu");
        
        _leftHand.ButtonPressed += LeftHandButtonPressed;
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

    private void LeftHandButtonPressed(string name)
    {
        if (name == "ax_button")
        {
            if (_playerMenu.ProcessMode == ProcessModeEnum.Inherit)
            {
                _playerMenu.ProcessMode = ProcessModeEnum.Disabled;
                _playerMenu.Visible = false;
            }
            else
            {
                _playerMenu.ProcessMode = ProcessModeEnum.Inherit;
                _playerMenu.Visible = true;
            }
        }
        else
        {
            // For the future generations
            return;
        }
    }
}
