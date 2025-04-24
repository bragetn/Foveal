using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public partial class FileHandler : Node
{
    const String DataPath = "data/eye_tracking/tests/";
    
    public String FileName;
    private TargetBox _targetBox;
    private String _testData;
    
    public override void _Ready()
    {
        _targetBox = GetNode<TargetBox>("../TargetBox");
        
        EyeTrackingRadio.Instance.ClearTargets += OnClearTargets;
        EyeTrackingRadio.Instance.SaveTestFile += SaveTestFile;
        EyeTrackingRadio.Instance.SaveTestResults += SaveTestResults;
        EyeTrackingRadio.Instance.LoadTestFile += LoadTestFile;
        EyeTrackingRadio.Instance.RenameTestFileTo += RenameTestFileTo;
        EyeTrackingRadio.Instance.DeleteTestFileForReal += DeleteTestFileForReal;
    }

    public override void _ExitTree()
    {
        EyeTrackingRadio.Instance.ClearTargets -= OnClearTargets;
        EyeTrackingRadio.Instance.SaveTestFile -= SaveTestFile;
        EyeTrackingRadio.Instance.SaveTestResults -= SaveTestResults;
        EyeTrackingRadio.Instance.LoadTestFile -= LoadTestFile;
        EyeTrackingRadio.Instance.RenameTestFileTo -= RenameTestFileTo;
        EyeTrackingRadio.Instance.DeleteTestFileForReal -= DeleteTestFileForReal;
    }

    private void OnClearTargets()
    {
        FileName = null;
    }

    private void SaveTestFile(string fileName)
    {
        if (string.IsNullOrEmpty(FileName))
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            FileName = fileName;
        }
        else
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                FileName = fileName;
            }
        }
        
        GazeTestData data = new GazeTestData
        {
            Name = FileName,
            GazeTime = 5f,
            Targets = new List<GazeTargetData>{}
        };
        
        foreach (var target in _targetBox.Targets)
        {
            GazeTargetData newTarget = new GazeTargetData
            {
                X = target.Position.X,
                Y = target.Position.Y,
                Z = target.Position.Z,
                Radius = target.Radius,
                Delay = target.Delay,
            };
            data.Targets.Add(newTarget);
        }
        string jsonString = JsonSerializer.Serialize(data);
        Directory.CreateDirectory(DataPath);
        File.WriteAllText(DataPath + FileName + ".json", jsonString);
        _testData = jsonString;
    }

    private void SaveTestResults(string fileName)
    {
        GD.Print(fileName);
        //magi
    }

    private void LoadTestFile(string fileName)
    {
        _targetBox.ClearTargets();
        string loadedJson = File.ReadAllText(DataPath + fileName + ".json");
        GazeTestData loadedGazeTestData = JsonSerializer.Deserialize<GazeTestData>(loadedJson);
        FileName = loadedGazeTestData.Name;
        foreach (var target in loadedGazeTestData.Targets)
        {
            _targetBox.AddTarget(new Vector3(target.X, target.Y, target.Z), target.Radius, target.Delay);
        }
    }

    private void RenameTestFileTo(string fileName, string newName)
    {
        if (FileName == fileName)
        {
            FileName = newName;
        }
        
        string loadedJson = File.ReadAllText(DataPath + fileName + ".json");
        GazeTestData loadedGazeTestData = JsonSerializer.Deserialize<GazeTestData>(loadedJson);
        
        loadedGazeTestData.Name = newName;
        string jsonString = JsonSerializer.Serialize(loadedGazeTestData);
        Directory.CreateDirectory(DataPath);
        File.WriteAllText(DataPath + newName + ".json", jsonString);
        _testData = jsonString;
        
        DeleteTestFileForReal(fileName);
    }

    private void DeleteTestFileForReal(string fileName)
    {
        File.Delete(DataPath + fileName + ".json");
    }
}
