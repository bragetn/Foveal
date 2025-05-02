using Godot;
using System;

public partial class MenuWindow : Window
{
    public override void _Ready()
    {
        CloseRequested += () => GetTree().Quit();
    }
}
