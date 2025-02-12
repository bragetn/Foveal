using Godot;
using System;

public partial class GazeTarget : StaticBody3D
{
    public void ChangeColor()
    {
        var meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
        
        if (meshInstance.Mesh is Mesh mesh)
        {
            var material = mesh.SurfaceGetMaterial(0);
        
            if (material is StandardMaterial3D standardMaterial)
            {
                standardMaterial.AlbedoColor = new Color(1, 0, 0);
            }
        }
    }
}
