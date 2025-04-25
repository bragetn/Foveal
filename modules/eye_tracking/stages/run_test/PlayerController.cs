using Godot;
using System;

public partial class PlayerController : Node
{
    [Export] public Node3D PlayerBody;
    
    private GazeSampler _sampler;
    
    public override void _Ready()
    {
        _sampler = GetNode<GazeSampler>("../GazeSampler");
        
        EyeTrackingRadio.Instance.ChangePlayerDistance += MovePlayer;
        EyeTrackingRadio.Instance.StartTest += _sampler.Start;
        EyeTrackingRadio.Instance.StopTest += _sampler.Stop;
    }

    public override void _ExitTree()
    {
        EyeTrackingRadio.Instance.ChangePlayerDistance -= MovePlayer;
        EyeTrackingRadio.Instance.StartTest -= _sampler.Start;
        EyeTrackingRadio.Instance.StopTest -= _sampler.Stop;
    }

    private void MovePlayer(float distance)
    {
        float maxZ = 3f;
        float minZ = -3f;

        Transform3D currentTransform = PlayerBody.GlobalTransform;

        float newZ = currentTransform.Origin.Z + distance;

        newZ = Mathf.Clamp(newZ, minZ, maxZ);

        Vector3 newPosition = new Vector3(currentTransform.Origin.X, currentTransform.Origin.Y, newZ);

        Transform3D teleportTransform = new Transform3D(currentTransform.Basis, newPosition);

        PlayerBody.Call("teleport", teleportTransform);
    }



}
