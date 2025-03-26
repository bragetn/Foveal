using Godot;
using System;

public partial class TargetBox : MeshInstance3D
{
    [Export] public PackedScene TargetScene = GD.Load<PackedScene>("uid://ce5n050wc588g");
    
    public override void _Ready()
    {
        EyeTrackingRadio.Instance.AddTarget += AddTarget;
        
        // Target Menu
        // Radio.Instance.AssignTargetToMenu += AssignTargetToMenu;
        // Radio.Instance.ExitTargetMenu += ExitTargetMenu;

    }
    
    private void AddTarget()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        Node3D target = TargetScene.Instantiate<Node3D>();
        target.GlobalPosition = new Vector3(rng.RandfRange(-3, 3), rng.RandfRange(0, 3), -5);
        GetTree().GetRoot().AddChild(target);
    }
}
