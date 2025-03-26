using Godot;
using System;

public partial class SceneLoader : Node
{
    private Node3D _parent;
    
    public override void _Ready()
    {
        _parent = GetParent<Node3D>();
        CoreRadio.Instance.LoadScene += _loadScene;
    }

    private void _loadScene(String path)
    {
        GD.Print("Loading Scene");
        _parent.Call("load_scene", path);
    }
}
