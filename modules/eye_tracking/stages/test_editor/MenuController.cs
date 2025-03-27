using Godot;
using System;

public partial class MenuController : Node
{
    [Export] public Node3D PlayerMenu;
    [Export] public Node3D PlayerBody;
    [Export] public XRController3D LeftHand;
    [Export] public float MenuScale { get; set; } = 0.2f;
    [Export] public Vector3 MenuOffset { get; set; } = Vector3.Zero;
    
    private PackedScene _playerMenuScene = GD.Load<PackedScene>("uid://dnrjqga7ascle");
    private PackedScene _targetMenuScene = GD.Load<PackedScene>("uid://d1tqqk3ddggtd");
    
    public override void _Ready()
    {
        LeftHand.ButtonPressed += LeftHandButtonPressed;
        EyeTrackingRadio.Instance.ResetPlayerPosition += ResetPlayerPosition;
        CoreRadio.Instance.GrabEntered += (() => SetMenu(_targetMenuScene));
        SetMenu(_playerMenuScene);
        PlayerMenu.ProcessMode = ProcessModeEnum.Disabled;
        PlayerMenu.Visible = false;
    }
    
    private void LeftHandButtonPressed(string name)
    {
        if (name == "ax_button")
        {
            if (PlayerMenu.ProcessMode == ProcessModeEnum.Inherit)
            {
                PlayerMenu.ProcessMode = ProcessModeEnum.Disabled;
                PlayerMenu.Visible = false;
            }
            else
            {
                PlayerMenu.ProcessMode = ProcessModeEnum.Inherit;
                PlayerMenu.Visible = true;
            }
        }
        else
        {
            // For the future generations
            return;
        }
    }

    private void SetMenu(PackedScene menuScene)
    {
        PlayerMenu.Set("scene", menuScene);
        
        var menu = PlayerMenu.Get("scene_node").As<Control>();
        var container = menu.GetChild(0).GetChild<Control>(0);

        Vector2 viewportSize = new Vector2(1132.0f, container.Size.Y);
        
        float aspectRatio = viewportSize.Y / viewportSize.X;
        Vector2 screenSize = new Vector2(1.0f, aspectRatio) * MenuScale;

        PlayerMenu.Position = PlayerMenu.Basis.Y * (screenSize.Y / 2.0f) + MenuOffset;
        
        PlayerMenu.Set("viewport_size", viewportSize);
        PlayerMenu.Set("screen_size", screenSize);
    }
    
    private void ResetPlayerPosition()
    {
        PlayerBody.Call("teleport", new Transform3D(new Basis(Vector3.Up, Mathf.DegToRad(0)), new Vector3(0, 0, 0)));
    }
}
