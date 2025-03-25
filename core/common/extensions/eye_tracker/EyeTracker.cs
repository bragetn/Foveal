using Godot;
using System;
using Godot.Collections;

public partial class EyeTracker : XRController3D
{
    [Export] public XRCamera3D Camera { get; set; }
    
    private Node3D _gazeDot;
    private IGazeable _prevGazeable;
    
    public override void _Ready()
    {
        _gazeDot = GetNode<Node3D>("GazeDot");
        CoreRadio.Instance.ToggleGazeDot += ToggleGazeDot;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 viewDir = _gazeDot.GlobalPosition - Camera.GlobalPosition;
        
        PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(Camera.GlobalPosition, _gazeDot.GlobalPosition);
        Dictionary result = spaceState.IntersectRay(query);

        if (result.Count <= 0)
        {
            _prevGazeable?.OnGazeExit();
            _prevGazeable = null;
            return;
        }
        
        Node collider = result["collider"].As<Node>();
        
        if (collider is IGazeable gazeable)
        {
            if (gazeable != _prevGazeable)
            {
                _prevGazeable?.OnGazeExit();
                gazeable.OnGazeEnter();
                _prevGazeable = gazeable;
            }
            else
            {
                gazeable.OnGazeStay(delta);
            }
        }
        else
        {
            _prevGazeable?.OnGazeExit();
            _prevGazeable = null;
        }
    }

    private void ToggleGazeDot()
    {
        _gazeDot.Visible = !_gazeDot.Visible;
    }
}
