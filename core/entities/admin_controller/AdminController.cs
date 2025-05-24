using Godot;
using System;

public partial class AdminController : Node
{
    [Export] public XRController3D HandController { get; set; }
    [Export] public Node3D Menu { get; set; }
    [Export] public Node3D Keyboard { get; set; }
    [Export] public Node3D Pointer { get; set; }

    private int pressCounter = 0;
    
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
            if (pressCounter == 0)
            {
                Timer timer = new Timer();
                timer.OneShot = true;
                timer.WaitTime = 0.7f;
                timer.Timeout += () =>
                {
                    pressCounter = 0;
                    RemoveChild(timer);
                    timer.QueueFree();
                };
                AddChild(timer);
                timer.Start();
            }
            
            pressCounter++;
            
            if (pressCounter == 3)
            {
                pressCounter = 0;
                CoreRadio.Instance.EmitSignal("ToggleAdminMode");
            }
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