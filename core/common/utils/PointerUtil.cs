using System;
using Godot;

public class PointerUtil
{
    public enum EventType
    {
        Entered,
        Exited,
        Pressed,
        Released,
        Moved,
    }
    
    public class PointerEvent
    {
        public EventType EventType { get; private set; }
        public Node3D Pointer { get; private set; }
        public Node3D Target { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector3 LastPosition { get; private set; }
        
        public PointerEvent(EventType eventType, Node3D pointer, Node3D target, Vector3 position, Vector3 lastPosition)
        {
            EventType = eventType;
            Pointer = pointer;
            Target = target;
            Position = position;
            LastPosition = lastPosition;
        }
    }

    public static PointerEvent ParseEvent(Variant variant)
    {
        var obj = variant.AsGodotObject();
        var eventType = obj.Get("event_type").As<EventType>();
        var pointer = obj.Get("pointer").As<Node3D>();
        var target = obj.Get("target").As<Node3D>();
        var position = obj.Get("position").As<Vector3>();
        var lastPosition = obj.Get("last_position").As<Vector3>();
        return new PointerEvent(eventType, pointer, target, position, lastPosition);
    }

}