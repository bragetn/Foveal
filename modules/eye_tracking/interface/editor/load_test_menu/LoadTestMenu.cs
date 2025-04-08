using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public partial class LoadTestMenu : Control
{
    const String DataPath = "data/eye_tracking/tests/";
    
    private PackedScene _gazeTestItemPackedScene = ResourceLoader.Load<PackedScene>("uid://yscns1tbnrae");
    
    private Control _container;
    private List<GazeTestData> _testDataList;

    public override void _Ready()
    {
        _container = GetNode<Control>("ScrollContainer/MarginContainer/VBoxContainer");
        _testDataList = new List<GazeTestData>();
        foreach (string filePath in Directory.GetFiles(DataPath))
        {
            string loadedJson = File.ReadAllText(filePath);
            GazeTestData loadedGazeTestData = JsonSerializer.Deserialize<GazeTestData>(loadedJson);
            _testDataList.Add(loadedGazeTestData);
        }

        foreach (GazeTestData testData in _testDataList)
        {
            GazeTestItem testItem = _gazeTestItemPackedScene.Instantiate<GazeTestItem>();
            testItem.TestData = testData;
            _container.AddChild(testItem);
        }
    }
}
