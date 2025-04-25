using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using CsvHelper;

public partial class FileHandler : Node
{
    const String TestDataPath = "data/eye_tracking/tests/";
    const String ResultDataPath = "data/eye_tracking/results/";
    
    public String TestName;
    
    private GazeSampler _gazeSampler;
    private TargetBox _targetBox;
    private String _testData;
    
    public override void _Ready()
    {
        _gazeSampler = GetNode<GazeSampler>("../XROrigin3D/GazeSampler");
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
        TestName = null;
    }

    private void SaveTestFile(string fileName)
    {
        if (string.IsNullOrEmpty(TestName))
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            TestName = fileName;
        }
        else
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                TestName = fileName;
            }
        }
        
        GazeTestData data = new GazeTestData
        {
            Name = TestName,
            GazeTime = _targetBox.GazeTime,
            ColliderSize = _targetBox.ColliderSize,
            Targets = new List<GazeTargetData>()
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
        Directory.CreateDirectory(TestDataPath);
        File.WriteAllText(TestDataPath + TestName + ".json", jsonString);
        _testData = jsonString;
    }

    private void SaveTestResults(string fileName)
    {
        string path = ResultDataPath + fileName + "/";
        Directory.CreateDirectory(path);
        
        // Save Test Result

        if (_targetBox != null)
        {
            TestResultData testResultData = _targetBox.GetTestResult();
            if (testResultData == null)
            {
                GD.PrintErr("There is no test result");
                return;
            }
            testResultData.TestName = TestName;
            
            string testResultJson = JsonSerializer.Serialize(testResultData);
            File.WriteAllText(path + "test_result.json", testResultJson);
        }
        
        // Save Gaze Samples

        if (_gazeSampler != null)
        {
            List<GazeSampleEntry> gazeSamples = _gazeSampler.GetSamples();

            if (gazeSamples.Count <= 0)
            {
                GD.PrintErr("There is no gaze samples");
                return;
            }

            using StreamWriter writer = new StreamWriter(path + "gaze_samples.txt");
            using CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(gazeSamples);
        }
        
    }

    private void LoadTestFile(string fileName)
    {
        _targetBox.ClearTargets();
        string loadedJson = File.ReadAllText(TestDataPath + fileName + ".json");
        GazeTestData loadedGazeTestData = JsonSerializer.Deserialize<GazeTestData>(loadedJson);
        TestName = loadedGazeTestData.Name;

        _targetBox.GazeTime = loadedGazeTestData.GazeTime;
        _targetBox.ColliderSize = loadedGazeTestData.ColliderSize;
        
        EyeTrackingRadio.Instance.EmitSignal("SetTestParameters", loadedGazeTestData.GazeTime, loadedGazeTestData.ColliderSize);

        foreach (var target in loadedGazeTestData.Targets)
        {
            _targetBox.AddTarget(new Vector3(target.X, target.Y, target.Z), target.Radius, target.Delay);
        }
    }

    private void RenameTestFileTo(string fileName, string newName)
    {
        if (TestName == fileName)
        {
            TestName = newName;
        }
        
        string loadedJson = File.ReadAllText(TestDataPath + fileName + ".json");
        GazeTestData loadedGazeTestData = JsonSerializer.Deserialize<GazeTestData>(loadedJson);
        
        loadedGazeTestData.Name = newName;
        string jsonString = JsonSerializer.Serialize(loadedGazeTestData);
        Directory.CreateDirectory(TestDataPath);
        File.WriteAllText(TestDataPath + newName + ".json", jsonString);
        _testData = jsonString;
        
        DeleteTestFileForReal(fileName);
    }

    private void DeleteTestFileForReal(string fileName)
    {
        File.Delete(TestDataPath + fileName + ".json");
    }
}
