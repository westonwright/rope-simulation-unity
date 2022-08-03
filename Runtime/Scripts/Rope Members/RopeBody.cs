using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RopeBody : IList<RopePoint>
{
    private List<RopePoint> points = new List<RopePoint>();

    private List<IRopeBodyListner> ropeBodyListners = new List<IRopeBodyListner>();

    public RopeBody(RopeBuilder ropeBuilder)
    {
        points = ropeBuilder.BuildRope();
    }

    public int Count => points.Count;

    public bool IsReadOnly => true;

    public RopePoint this[int index] { get { return points[index]; } set { points[index] = value; } }

    public void AddListner(IRopeBodyListner listner)
    {
        if (!ropeBodyListners.Contains(listner))
        {
            ropeBodyListners.Add(listner);
        }
    }

    public void RemoveListner(IRopeBodyListner listner)
    {
        ropeBodyListners.Remove(listner);
    }

    /// <summary>
    /// Adds a point to the beginning if true or end if false
    /// </summary>
    /// <param name="direction"></param>
    public void AddRopePoint(RopePoint point, bool direction)
    {
        if (direction) points.Insert(0, point);
        else points.Add(point);

        foreach (IRopeBodyListner ropeBodyListner in ropeBodyListners)
        {
            if(ropeBodyListner != null) ropeBodyListner.RopePointAdded(direction);
        }
    }
    /// <summary>
    /// Removes a point from the beginning if true or the end if false
    /// </summary>
    /// <param name="direction"></param>
    public void RemoveRopePoint(bool direction)
    {
        if (direction) points.RemoveAt(0);
        else points.RemoveAt(points.Count - 1);

        foreach (IRopeBodyListner ropeBodyListner in ropeBodyListners)
        {
            if (ropeBodyListner != null) ropeBodyListner.RopePointRemoved(direction);
        }
    }

    public IEnumerator<RopePoint> GetEnumerator()
    {
        foreach(RopePoint point in points)
        {
            yield return point;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int IndexOf(RopePoint item) => points.IndexOf(item);

    public void Insert(int index, RopePoint item) => points.Insert(index, item);

    public void RemoveAt(int index) => points.RemoveAt(index);

    public void Add(RopePoint item) => points.Add(item);

    public void Clear() => points.Clear();

    public bool Contains(RopePoint item) => points.Contains(item);

    public void CopyTo(RopePoint[] array, int arrayIndex) => points.CopyTo(array, arrayIndex);

    public bool Remove(RopePoint item) => points.Remove(item);
}

public interface IRopeBodyListner
{
    void RopePointAdded(bool direction);
    void RopePointRemoved(bool driection);
}

public interface IRopePointPosition
{
    public Vector3 Position { get; set; }
}

public interface IRopePointPrevPosition
{
    public Vector3 PrevPosition { get; set; }

}

public interface IRopePointAccumulatedForce
{
    public Vector3 AccumulatedForce { get; set; }
}

public interface IRopePointFriction
{
    public float Friction { get; set; }
}

public interface IRopePointIsAttached
{
    public bool IsAttached { get; set; }
}

public class RopePoint : 
    IRopePointPosition, 
    IRopePointPrevPosition, 
    IRopePointAccumulatedForce, 
    IRopePointFriction, 
    IRopePointIsAttached
{
    private Vector3 position, prevPosition, accumulatedForce = Vector3.zero;
    public Vector3 Position { get => position; set => position = value; }
    public Vector3 PrevPosition { get => prevPosition; set => prevPosition = value; }
    public Vector3 AccumulatedForce { get => accumulatedForce; set => accumulatedForce = value; }

    private float friction = 0;
    public float Friction { get => friction; set => friction = value; }
    private bool attached = false;
    public bool IsAttached { get => attached; set => attached = value; }
    public RopePoint(Vector3 position)
    {
        this.position = position;
        this.prevPosition = position;
    }
}
