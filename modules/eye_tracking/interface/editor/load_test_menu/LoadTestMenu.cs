using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public partial class LoadTestMenu : Control
{
    const String DataPath = "data/eye_tracking/tests/";
    private Control _container;
    private List<GazeTestData> _testDataList;

    public override void _Ready()
    {
        _container = GetNode<Control>("ScrollContainer/VBoxContainer");
        _testDataList = new List<GazeTestData>();
        foreach (string filePath in Directory.GetFiles(DataPath))
        {
            string loadedJson = File.ReadAllText(filePath);
            GazeTestData loadedGazeTestData = JsonSerializer.Deserialize<GazeTestData>(loadedJson);
            _testDataList.Add(loadedGazeTestData);
        }

        foreach (GazeTestData testData in _testDataList)
        {
            Button TestDataButton = new Button();
            TestDataButton.Text = testData.Name;
            TestDataButton.Pressed += () => EyeTrackingRadio.Instance.EmitSignal("LoadTestFile", testData.Name);
            _container.AddChild(TestDataButton);
        }
    }
}
