using UnityEngine;

public class RopePoint
{
    private Vector3 position, prevPosition, accumulatedForce;
    public Vector3 Position { get => position; set => position = value; }
    public Vector3 PrevPosition { get => prevPosition; set => prevPosition = value; }
    public Vector3 AccumulatedForce { get => accumulatedForce; set => accumulatedForce = value; }

    private float friction = 0;
    public float Friction { get => friction; set => friction = value; }
    private bool locked = false;
    public bool IsLocked { get => locked; set => locked = value; }
    public RopePoint(Vector3 position)
    {
        this.position = position;
        this.prevPosition = position;
        this.accumulatedForce = Vector3.zero;
    }
}

public class RopeStick
{
    private RopePoint pointA, pointB;
    public RopePoint PointA { get => pointA; }
    public RopePoint PointB { get => pointB; }

    private float length;
    public float Length { get => length; set => length = value; }

    public RopeStick(RopePoint pointA, RopePoint pointB, float length)
    {
        this.pointA = pointA;
        this.pointB = pointB;
        this.length = length;
    }
}