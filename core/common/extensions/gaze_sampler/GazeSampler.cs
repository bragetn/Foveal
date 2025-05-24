using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class GazeSampler : Node
{
    [Export] public EyeTracker ETracker;

    private List<GazeSampleEntry> _samples = new List<GazeSampleEntry>();
    private Stopwatch _stopwatch = new Stopwatch();

    public void Start()
    {
        _samples.Clear();
        _stopwatch.Reset();
        _stopwatch.Start();
        
        ETracker.GazeSample += HandleGazeSample;
    }

    public void Stop()
    {
        _stopwatch.Stop();
        
        ETracker.GazeSample -= HandleGazeSample;
    }

    public List<GazeSampleEntry> GetSamples()
    {
        return _samples;
    }

    private void HandleGazeSample(Camera3D camera, Vector3 gazeRayRotation, Vector3 hitPoint)
    {
        GazeSampleEntry sample = new GazeSampleEntry
        {
            Time = _stopwatch.Elapsed,
            CameraPositionX = camera.GlobalPosition.X,
            CameraPositionY = camera.GlobalPosition.Y,
            CameraPositionZ = camera.GlobalPosition.Z,
            CameraRotationX = camera.GlobalRotation.X,
            CameraRotationY = camera.GlobalRotation.Y,
            CameraRotationZ = camera.GlobalRotation.Z,
            GazeRayRotationX = gazeRayRotation.X,
            GazeRayRotationY = gazeRayRotation.Y,
            GazeRayRotationZ = gazeRayRotation.Z,
            HitPointX = hitPoint.X,
            HitPointY = hitPoint.Y,
            HitPointZ = hitPoint.Z,
        };
        
        _samples.Add(sample);
    }
}
