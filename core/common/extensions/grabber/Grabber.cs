using Godot;
using System;

public partial class Grabber : Node
{
    [Export] public float TargetMoveSpeed { get; set; } = 1.0f;
    [Export] public Node3D Pointer;
    [Export] public XRController3D HandController;
    [Export] public Node MovementTurn;
    
    private bool _isGrabbing;
    private IGrabbable _grabbedTarget;
    
    public override void _Ready()
    {
        Pointer.Connect("pointing_event", new Callable(this, nameof(PointingEvent)));
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_grabbedTarget != null)
        {
            Vector2 primaryInput = HandController.GetVector2("primary");
            _grabbedTarget.OnGrabStay(delta, TargetMoveSpeed * primaryInput.Y);
        }
    }

    private void PointingEvent(Variant pointingEvent)
    {
        PointerUtil.PointerEvent pointerEvent = PointerUtil.ParseEvent(pointingEvent);

        if (pointerEvent.Target is IGrabbable grabbable)
        {
            switch (pointerEvent.EventType)
            {
                case PointerUtil.EventType.Pressed:
                    grabbable.OnGrabEnter(pointerEvent);
                    CoreRadio.Instance.EmitSignal("GrabEntered", pointerEvent.Target);
                    MovementTurn.Set("enabled", false);
                    _isGrabbing = true;
                    _grabbedTarget = grabbable;
                    break;
                case PointerUtil.EventType.Released:
                    grabbable.OnGrabExit();
                    MovementTurn.Set("enabled", true);
                    CoreRadio.Instance.EmitSignal("GrabExited");
                    _isGrabbing = false;
                    _grabbedTarget = null;
                    break;
            }
        }
    }
}
