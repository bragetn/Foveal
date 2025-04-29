using Godot;
using System;

public partial class AdminController : Node
{
    
    [Export] public XRController3D HandController { get; set; }
    [Export] public Node3D Menu { get; set; }
    [Export] public Node3D Keyboard { get; set; }
    [Export] public Node3D Pointer { get; set; }
    public override void _Ready()
    {
        HandController.ButtonPressed += HandButtonPressed;
        CoreRadio.Instance.ToggleAdminMode += ToggleAdmin;
        ToggleAdmin();
    }

    public override void _ExitTree()
    {
        CoreRadio.Instance.ToggleAdminMode -= ToggleAdmin;
    }

    private void HandButtonPressed(string name)
    {
        if (name == "by_button")
        {
            CoreRadio.Instance.EmitSignal("ToggleAdminMode");
        }
    }

    private void ToggleAdmin()
    {
        if (Menu != null) Menu.Visible = CoreRadio.Instance.AdminMode;
        if (Pointer != null)
        {
            Pointer.Visible =  CoreRadio.Instance.AdminMode;
            Pointer.Set("enabled",  CoreRadio.Instance.AdminMode);
        }
        if (Keyboard != null) Keyboard.Visible =  CoreRadio.Instance.AdminMode;
    }
}