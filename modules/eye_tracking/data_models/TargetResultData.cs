using System;

public class TargetResultData
{
    public GazeTargetData GazeTargetData { get; set; }
    public bool HasBeenSeen { get; set; }
    public TimeSpan TimeBeforeSeen { get; set; }
}