using System.Collections.Generic;

public class GazeTestData
{
    public string Name { get; set; }
    public float GazeTime { get; set; }
    public float ColliderSize { get; set; }
    public List<GazeTargetData> Targets { get; set; }
}