using Godot;
using System;

public partial class Grabber : Node
{
    [Export] public float TargetMoveSpeed { get; set; } = 1.0f;
    [Export] public Node3D Pointer;
    [Export] public XRController3D HandController;
    [Export] public Node MovementTurn;
    
    private bool _isGrabbing;
    private GazeTarget _grabbedTarget;
    
    
    public override void _Ready()
    {
        Pointer.Connect("pointing_event", new Callable(this, nameof(PointingEvent)));
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_grabbedTarget != null)
        {
            Vector2 primaryInput = HandController.GetVector2("primary");
            _grabbedTarget.UpdatePointerDistance((float)(TargetMoveSpeed * primaryInput.Y * delta));
        }
    }

    private void PointingEvent(Variant pointingEvent)
    {
        PointerUtil.PointerEvent pointerEvent = PointerUtil.ParseEvent(pointingEvent);

        if (pointerEvent.Target is GazeTarget gazeTarget)
        {
            switch (pointerEvent.EventType)
            {
                case PointerUtil.EventType.Pressed:
                    gazeTarget.OnGrabEnter(pointerEvent);
                    CoreRadio.Instance.EmitSignal("GrabEntered", gazeTarget);
                    MovementTurn.Set("enabled", false);
                    _isGrabbing = true;
                    _grabbedTarget = gazeTarget;
                    break;
                case PointerUtil.EventType.Released:
                    gazeTarget.OnGrabExit();
                    MovementTurn.Set("enabled", true);
                    CoreRadio.Instance.EmitSignal("GrabExited");
                    _isGrabbing = false;
                    break;
            }
        }
    }
}
