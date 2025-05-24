using Godot;
using System;

public partial class MainMenu : Control
{
    [Export] public ModuleResource[] Modules = [];
    
    private PackedScene _moduleButton = GD.Load<PackedScene>("uid://b1trqh82qsub5");
    private PackedScene _settingsMenu = GD.Load<PackedScene>("uid://b1d77bki2hwac");
    
    private Control _modulesContainer;
    private Control _menuContainer;
    private Button _settingsButton;

    private Control _currentMenu;
    
    public override void _Ready()
    {
        _modulesContainer = GetNode<Control>("MarginContainer/HBoxContainer/ModulesContainer/ColorRect/MarginContainer/VBoxContainer/ModulesScroll/ModulesVBox");
        _menuContainer = GetNode<Control>("MarginContainer/HBoxContainer/MenuContainer/MenuPanel/MarginContainer");
        _settingsButton = GetNode<Button>("MarginContainer/HBoxContainer/ModulesContainer/ColorRect/MarginContainer/VBoxContainer/SettingsButton");

        _settingsButton.Pressed += () => SetMenu(_settingsMenu);

        if (Modules.Length <= 0) return;

        foreach (ModuleResource module in Modules)
        {
            Button button = _moduleButton.Instantiate<Button>();
            button.Text = module.ModuleName;
            button.Pressed += () => SetMenu(module.ModuleMenuScene);
            _modulesContainer.AddChild(button);
        }
        
        SetMenu(Modules[0].ModuleMenuScene);
        _modulesContainer.GetChild<Button>(0).GrabFocus();
    }

    private void SetMenu(PackedScene menu)
    {
        _currentMenu?.QueueFree();
        _currentMenu = menu.Instantiate<Control>();
        _menuContainer.AddChild(_currentMenu);
    }
}
