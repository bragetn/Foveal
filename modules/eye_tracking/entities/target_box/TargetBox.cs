using Godot;
using System;

[Tool]
public partial class TargetBox : Node3D
{
    [Export] public PackedScene TargetScene = GD.Load<PackedScene>("uid://ce5n050wc588g");
    [Export] public Vector3 BoxScale
    {
        get => _halfScale * 2.0f;
        set
        {
            _halfScale = (value / 2.0f);
            _halfScale = new Vector3(
                Mathf.Max(_halfScale.X, 0.0f),
                Mathf.Max(_halfScale.Y, 0.0f),
                Mathf.Max(_halfScale.Z, 0.0f));
            UpdateBoxScale();
        }
    }
    
    [ExportGroup("Walls")]
    [Export] public StaticBody3D LeftWall;
    [Export] public StaticBody3D RightWall;
    [Export] public StaticBody3D BackWall;
    [Export] public StaticBody3D FloorWall;
    [Export] public StaticBody3D CeilingWall;

    private Vector3 _halfScale = new Vector3(4.0f, 2.0f, 2.0f);
    
    public override void _Ready()
    {
        if (Engine.IsEditorHint()) return;
        
        EyeTrackingRadio.Instance.AddTarget += AddTarget;
        UpdateBoxScale();
        
        // Target Menu
        // Radio.Instance.AssignTargetToMenu += AssignTargetToMenu;
        // Radio.Instance.ExitTargetMenu += ExitTargetMenu;
    }
    
    private void AddTarget()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        Node3D target = TargetScene.Instantiate<Node3D>();
        
        float x = rng.RandfRange(-_halfScale.X, _halfScale.X);
        float y = rng.RandfRange(-_halfScale.Y, _halfScale.Y);
        float z = rng.RandfRange(-_halfScale.Z, _halfScale.Z);
        
        target.Position = new Vector3(x, y, z);
        AddChild(target);
    }

    private void UpdateBoxScale()
    {
        Vector3 boxScale = BoxScale;
        
        UpdateWall(LeftWall, Vector3.Left * _halfScale.X, boxScale.Z, boxScale.Y);
        UpdateWall(RightWall, Vector3.Right * _halfScale.X, boxScale.Z, boxScale.Y);
        UpdateWall(BackWall, Vector3.Forward * _halfScale.Z, boxScale.X, boxScale.Y);
        UpdateWall(FloorWall, Vector3.Down * _halfScale.Y, boxScale.X, boxScale.Z);
        UpdateWall(CeilingWall, Vector3.Up * _halfScale.Y, boxScale.X, boxScale.Z);
    }

    private void UpdateWall(StaticBody3D wall, Vector3 position, float x, float y)
    {
        if (wall == null) return;
        
        wall.Position = position;
        
        if (wall.GetNode<CollisionShape3D>("CollisionShape3D").Shape is BoxShape3D shape)
        {
            shape.Size = new Vector3(x, y, 0);
        }
        
        if (wall.GetNode<MeshInstance3D>("MeshInstance3D").Mesh is QuadMesh mesh)
        {
            mesh.Size = new Vector2(x, y);
        }
    }
}
