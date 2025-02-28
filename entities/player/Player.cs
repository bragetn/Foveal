using Godot;
using System;
using System.Diagnostics;

public partial class Player : XROrigin3D
{
    [Export] public float TargetMoveSpeed { get; set; } = 1.0f;
    [Export] public float MenuScale { get; set; } = 0.2f;
    [Export] public Vector3 MenuOffset { get; set; } = Vector3.Zero;
    
    private XRCamera3D _camera;
    private XRController3D _eyes;
    private XRController3D _leftHand;
    private XRController3D _rightHand;
    private Node3D _gazeDot;
    private Node3D _playerMenu;
    private Node3D _playerBody;
    private Node3D _pointer;
    private Node _movementTurn;

    private PackedScene _targetScene = GD.Load<PackedScene>("res://entities/gaze_target/gaze_target.tscn");
    private PackedScene _playerMenuScene = GD.Load<PackedScene>("res://interface/player_menu/player_menu.tscn");
    private PackedScene _targetMenuScene = GD.Load<PackedScene>("res://interface/target_menu/target_menu.tscn");

    private bool _isGrabbing;
    private GazeTarget _grabbedTarget;

    public override void _Ready()
    {
        _camera = GetNode<XRCamera3D>("XRCamera3D");
        _eyes = GetNode<XRController3D>("Eyes");
        _leftHand = GetNode<XRController3D>("LeftHand");
        _rightHand = GetNode<XRController3D>("RightHand");
        _gazeDot = GetNode<Node3D>("Eyes/GazeDot");
        _playerMenu = GetNode<Node3D>("LeftHand/PlayerMenu");
        _playerBody = GetNode<Node3D>("PlayerBody");
        _pointer = GetNode<Node3D>("RightHand/FunctionPointer");
        _movementTurn = GetNode<Node>("RightHand/MovementTurn");

        _leftHand.ButtonPressed += LeftHandButtonPressed;
        
        _pointer.Connect("pointing_event", new Callable(this, nameof(PointingEvent)));
        
        // Player Menu
        Radio.Instance.ResetPlayerPosition += ResetPlayerPosition;
        Radio.Instance.ToggleGazeDot += ToggleGazeDot;
        Radio.Instance.AddTarget += AddTarget;
        
        // Target Menu
        Radio.Instance.AssignTargetToMenu += AssignTargetToMenu;
        Radio.Instance.ExitTargetMenu += ExitTargetMenu;

        SetMenu(_playerMenuScene);
        _playerMenu.ProcessMode = ProcessModeEnum.Disabled;
        _playerMenu.Visible = false;
        
    }

    public override void _PhysicsProcess(double delta)
    {
        ProcessEyeGaze(delta);
        ProcessGrab(delta);
    }

    private void ProcessEyeGaze(double delta)
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

    private void ProcessGrab(double delta)
    {
        if (_grabbedTarget != null)
        {
            Vector2 primaryInput = _rightHand.GetVector2("primary");
            _grabbedTarget.UpdatePointerDistance((float)(TargetMoveSpeed * primaryInput.Y * delta));
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
    
    private void PointingEvent(Variant pointingEvent)
    {
        var pointerEvent = PointerUtil.ParseEvent(pointingEvent);

        if (pointerEvent.Target is GazeTarget gazeTarget)
        {
            switch (pointerEvent.EventType)
            {
                case PointerUtil.EventType.Pressed:
                    gazeTarget.Grab(pointerEvent);
                    _movementTurn.Set("enabled", false);
                    _isGrabbing = true;
                    _grabbedTarget = gazeTarget;
                    SetMenu(_targetMenuScene);
                    break;
                case PointerUtil.EventType.Released:
                    gazeTarget.Release();
                    _movementTurn.Set("enabled", true);
                    _isGrabbing = false;
                    break;
            }
        }
    }

    private void SetMenu(PackedScene menuScene)
    {
        _playerMenu.Set("scene", menuScene);
        
        var menu = _playerMenu.Get("scene_node").As<Control>();
        var container = menu.GetChild(0).GetChild<Control>(0);

        Vector2 viewportSize = new Vector2(1132.0f, container.Size.Y);
        
        float aspectRatio = viewportSize.Y / viewportSize.X;
        Vector2 screenSize = new Vector2(1.0f, aspectRatio) * MenuScale;

        _playerMenu.Position = _playerMenu.Basis.Y * (screenSize.Y / 2.0f) + MenuOffset;
        
        _playerMenu.Set("viewport_size", viewportSize);
        _playerMenu.Set("screen_size", screenSize);
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

    private void AssignTargetToMenu(TargetMenu targetMenu)
    {
        targetMenu.Target = _grabbedTarget;
    }

    private void ExitTargetMenu()
    {
        SetMenu(_playerMenuScene);
    }
}