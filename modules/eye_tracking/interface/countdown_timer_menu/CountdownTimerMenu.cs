using Godot;
using System;

public partial class CountdownTimerMenu : Control
{
    private Label _label;
    private Timer _timer;
    
    private int _counter;

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
        _timer = GetNode<Timer>("Timer");
        
        _timer.Timeout += ProcessTimer;

        EyeTrackingRadio.Instance.StartCountdownTimer += Start;
    }

    private void Start(int seconds)
    {
        _counter = seconds;
        _label.Text = _counter.ToString();
        _timer.Start();
    }

    private void ProcessTimer()
    {
        _counter--;
        
        if (_counter <= 0)
        {
            _label.Text = "";
            _timer.Stop();
            EyeTrackingRadio.Instance.EmitSignal("StartTest");
        }
        else
        {
            _label.Text = _counter.ToString();
        }
    }
}
