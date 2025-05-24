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
    const string TestDataPath = "data/eye_tracking/tests/";
    const string ResultDataPath = "data/eye_tracking/results/";
    
    [Export] public GazeSampler Sampler { get; set; }
    [Export] public TargetBox Box { get; set; }
    
    public string TestName;
    private string _testData;
    
    public override void _Ready()
    {
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
            GazeTime = Box.GazeTime,
            ColliderSize = Box.ColliderSize,
            Targets = new List<GazeTargetData>()
        };
        
        foreach (var target in Box.Targets)
        {
            data.Targets.Add(target.GetTargetData());
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

        if (Box != null)
        {
            TestResultData testResultData = Box.GetTestResult();
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

        if (Sampler != null)
        {
            List<GazeSampleEntry> gazeSamples = Sampler.GetSamples();

            if (gazeSamples.Count <= 0)
            {
                GD.PrintErr("There is no gaze samples");
                return;
            }

            using StreamWriter writer = new StreamWriter(path + "gaze_samples.csv.txt");
            using CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(gazeSamples);
        }
        
    }

    private void LoadTestFile(string fileName)
    {
        Box.ClearTargets();
        string loadedJson = File.ReadAllText(TestDataPath + fileName + ".json");
        GazeTestData loadedGazeTestData = JsonSerializer.Deserialize<GazeTestData>(loadedJson);
        TestName = loadedGazeTestData.Name;

        Box.GazeTime = loadedGazeTestData.GazeTime;
        Box.ColliderSize = loadedGazeTestData.ColliderSize;
        Box.UpdateColliderSize();
        
        EyeTrackingRadio.Instance.EmitSignal("SetTestParameters", loadedGazeTestData.GazeTime, loadedGazeTestData.ColliderSize);

        foreach (var target in loadedGazeTestData.Targets)
        {
            Box.AddTarget(new Vector3(target.X, target.Y, target.Z), target.Radius, target.Delay, target.Type);
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
