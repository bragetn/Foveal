using Godot;
using System;

public partial class GazeTestItem : Control
{
    public GazeTestData TestData;
    public bool Editable;
    
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
        
        if (Editable)
        {
            _renameButton.Pressed += () => EyeTrackingRadio.Instance.EmitSignal("RenameTestFile", TestData.Name);
            _deleteButton.Pressed += () => EyeTrackingRadio.Instance.EmitSignal("DeleteTestFile", TestData.Name);
        }
        else
        {
            _renameButton.Visible = false;
            _deleteButton.Visible = false;
        }
    }
}
