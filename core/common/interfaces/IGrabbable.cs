using Godot;
using System;

public interface IGrabbable
{
    void OnGrabEnter(PointerUtil.PointerEvent pointerEvent) {}
    void OnGrabStay(double delta) {}
    void OnGrabExit() {}
}