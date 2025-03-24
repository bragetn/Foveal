using Godot;
using System;

public interface IGazeable
{
    void OnGazeEnter() {}
    void OnGazeStay(double delta) {}
    void OnGazeExit() {}
}
