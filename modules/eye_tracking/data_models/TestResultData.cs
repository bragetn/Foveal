using System;
using System.Collections.Generic;

public class TestResultData
{
    public string TestName { get; set; }
    public TimeSpan TestTime { get; set; }
    public float GazeTime { get; set; }
    public float PlayerDistance { get; set; }
    public List<TargetResultData> TargetResults { get; set; }
}