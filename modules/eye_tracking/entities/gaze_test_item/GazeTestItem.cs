using Godot;
using System;

public partial class GazeTestItem : Control
{
    public GazeTestData TestData;
    
    private Button _selectButton;
    private Button _renameButton;
    private Button _deleteButton;
    
    public override void _Ready()
    {
        _selectButton = GetNode<Button>("HBoxContainer/SelectButton");
        _renameButton = GetNode<Button>("HBoxContainer/RenameButton");
        _deleteButton = GetNode<Button>("HBoxContainer/DeleteButton");
        
        _selectButton.Text = TestData.Name;
        _selectButton.Pressed += () => EyeTrackingRadio.Instance.EmitSignal("LoadTestFile", TestData.Name);
        _renameButton.Pressed += () => EyeTrackingRadio.Instance.EmitSignal("RenameTestFile", TestData.Name);
        _deleteButton.Pressed += () => EyeTrackingRadio.Instance.EmitSignal("DeleteTestFile", TestData.Name);
    }
}
