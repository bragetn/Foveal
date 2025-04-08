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
    
    private GazeTarget _grabbedTarget;
    
    private bool _menuEnabled = true;
    private bool _menuVisible = false;
    
    public override void _Ready()
    {
        LeftHand.ButtonPressed += LeftHandButtonPressed;
        CoreRadio.Instance.GrabEntered += GrabEntered;
        
        EyeTrackingRadio.Instance.ResetPlayerPosition += ResetPlayerPosition;
        EyeTrackingRadio.Instance.AssignTargetToMenu += AssignTargetToMenu;
        EyeTrackingRadio.Instance.ExitTargetMenu += ExitTargetMenu;
        
        EyeTrackingRadio.Instance.LoadTestFile += name => SetMenu(_playerMenuScene);
        EyeTrackingRadio.Instance.ClearTargets += () => SetMenu(_playerMenuScene);
        
        EyeTrackingRadio.Instance.PreviewTest += running =>
        {
            if (running)
            {
                _menuEnabled = false;
                PlayerMenu.ProcessMode = ProcessModeEnum.Disabled;
                PlayerMenu.Visible = false;
            }
            else
            {
                _menuEnabled = true;
                
                if (!_menuVisible)
                {
                    PlayerMenu.ProcessMode = ProcessModeEnum.Disabled;
                    PlayerMenu.Visible = false;
                    _menuVisible = false;
                }
                else
                {
                    PlayerMenu.ProcessMode = ProcessModeEnum.Inherit;
                    PlayerMenu.Visible = true;
                    _menuVisible = true;
                }
            }
        };
        
        SetMenu(_playerMenuScene);
        PlayerMenu.ProcessMode = ProcessModeEnum.Disabled;
        PlayerMenu.Visible = false;
    }
    
    private void LeftHandButtonPressed(string name)
    {
        if (!_menuEnabled) return;
        
        if (name == "ax_button")
        {
            if (_menuVisible)
            {
                PlayerMenu.ProcessMode = ProcessModeEnum.Disabled;
                PlayerMenu.Visible = false;
                _menuVisible = false;
            }
            else
            {
                PlayerMenu.ProcessMode = ProcessModeEnum.Inherit;
                PlayerMenu.Visible = true;
                _menuVisible = true;
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

    private void GrabEntered(Node grabbedNode)
    {
        if (!_menuEnabled) return;
        
        if (grabbedNode is GazeTarget gazeTarget)
        {
            _grabbedTarget = gazeTarget;
        }
        SetMenu(_targetMenuScene);
    }
    
    private void ResetPlayerPosition()
    {
        PlayerBody.Call("teleport", new Transform3D(new Basis(Vector3.Up, Mathf.DegToRad(0)), new Vector3(0, 0, 0)));
    }
    
    private void ExitTargetMenu()
    {
        SetMenu(_playerMenuScene);
    }
    
    private void AssignTargetToMenu(TargetMenu targetMenu)
    {
        if (!_menuEnabled) return;
        targetMenu.Target = _grabbedTarget;
    }
}
