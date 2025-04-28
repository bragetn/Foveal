using Godot;

[GlobalClass]
public partial class ModuleResource : Resource
{
    [Export] public string ModuleName { get; set; }
    [Export] public PackedScene ModuleMenuScene { get; set; }
}