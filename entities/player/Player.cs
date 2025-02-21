using Godot;
using System;
using System.Diagnostics;

public partial class Player : XROrigin3D
{
    private XRCamera3D _camera;
    private XRController3D _eyes;
    private XRController3D _leftHand;
    private Node3D _gazeDot;
    private Node3D _playerMenu;
    private Node3D _playerBody;
    private Node3D _pointer;

    private PackedScene _targetScene = GD.Load<PackedScene>("res://entities/gaze_target/gaze_target.tscn");

    public override void _Ready()
    {
        _camera = GetNode<XRCamera3D>("XRCamera3D");
        _eyes = GetNode<XRController3D>("Eyes");
        _leftHand = GetNode<XRController3D>("LeftHand");
        _gazeDot = GetNode<Node3D>("Eyes/GazeDot");
        _playerMenu = GetNode<Node3D>("LeftHand/PlayerMenu");
        _playerBody = GetNode<Node3D>("PlayerBody");
        _pointer = GetNode<Node3D>("RightHand/FunctionPointer");

        _leftHand.ButtonPressed += LeftHandButtonPressed;
        
        _pointer.Connect("pointing_event", new Callable(this, nameof(PointingEvent)));

        Radio.Instance.ResetPlayerPosition += ResetPlayerPosition;
        Radio.Instance.ToggleGazeDot += ToggleGazeDot;
        Radio.Instance.AddTarget += AddTarget;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 viewDir = _gazeDot.GlobalPosition - _camera.GlobalPosition;

        var spaceState = GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(_camera.GlobalPosition, _gazeDot.GlobalPosition);
        var result = spaceState.IntersectRay(query);

        if (result.Count <= 0) return;

        var collider = result["collider"].As<Node>();
        if (collider is GazeTarget gazeTarget)
        {
            gazeTarget.AddValue((float)delta);
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

    private void ResetPlayerPosition()
    {
        _playerBody.Call("teleport", new Transform3D(new Basis(Vector3.Up, Mathf.DegToRad(0)), new Vector3(0, 0, 0)));
    }

    private void ToggleGazeDot()
    {
        _gazeDot.Visible = !_gazeDot.Visible;
    }

    private void AddTarget()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        Node3D target = _targetScene.Instantiate<Node3D>();
        target.GlobalPosition = new Vector3(rng.RandfRange(-3, 3), rng.RandfRange(0, 3), -5);
        GetTree().GetRoot().AddChild(target);
    }

    private void PointingEvent(Variant pointingEvent)
    {
        var pointerEvent = PointerUtil.ParseEvent(pointingEvent);

        if (pointerEvent.Target is GazeTarget gazeTarget)
        {
            switch (pointerEvent.EventType)
            {
                case PointerUtil.EventType.Pressed:
                    GD.Print("Grab");
                    gazeTarget.Grab(pointerEvent);
                    break;
                case PointerUtil.EventType.Released:
                    GD.Print("Release");
                    gazeTarget.Release();
                    break;
            }
        }
    }
}