using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RopeSimulatorLength : IList<RopeStick>
{
    private List<RopeStick> ropeSticks = new List<RopeStick>();

    private RopeBody ropeBody;

    private readonly float MIN_SEGMENT_LENGTH = .1f;

    private float goalLength = 5f;
    public float GoalLength
    {
        set
        {
            // add debug log if value is set too high or too low?
            goalLength = Mathf.Clamp(value, minRopeLength, maxRopeLength);
        }
        get
        {
            return goalLength;
        }
    }

    private float currentLength = 5f;
    public float CurrentLength
    {
        get => currentLength;
    }

    private float maxRopeLength = 10f;
    public float MaxRopeLength
    {
        get => maxRopeLength;
        set
        {
            maxRopeLength = Mathf.Clamp(value, MinRopeLength, Mathf.Infinity);
            if (CurrentLength > maxRopeLength)
            {
                GoalLength = maxRopeLength;
            }
        }
    }

    private float minRopeLength = .1f;
    public float MinRopeLength
    {
        get => minRopeLength;
        set
        {
            minRopeLength = Mathf.Clamp(value, 0, MaxRopeLength);
            if (CurrentLength < minRopeLength)
            {
                GoalLength = minRopeLength;
            }
        }
    }

    private float segmentLength = .2f;
    public float SegmentLength
    {
        get => segmentLength;
        set
        {
            // must adjust length to get rope back to proper length now
            // that segments have changes size
            float newLength = Mathf.Clamp(value, MIN_SEGMENT_LENGTH, maxRopeLength);
            float ratio = newLength / segmentLength;

            segmentLength = newLength;
            GoalLength = CurrentLength * ratio;
        }
    }

    public float PercentReleased
    {
        get
        {
            return Mathf.Clamp01((goalLength - MinRopeLength) / (MaxRopeLength - MinRopeLength));
        }
        set
        {
            float percent = Mathf.Clamp01(value);
            GoalLength = ((MaxRopeLength - MinRopeLength) * percent) + MinRopeLength;
        }
    }

    public int Count => ropeSticks.Count;

    public bool IsReadOnly => true;

    public RopeStick this[int index] { get => ropeSticks[index]; set => ropeSticks[index] = value; }

    public RopeSimulatorLength(
        RopeBody ropeBody,
        float goalLength,
        float maxRopeLength,
        float minRopeLength, 
        float segmentLength
        )
    {
        this.ropeBody = ropeBody;
        this.goalLength = goalLength;
        this.maxRopeLength = maxRopeLength;
        this.minRopeLength = minRopeLength;
        this.segmentLength = segmentLength;

        int stickCount = Mathf.Max(ropeBody.Count - 1, 0);
        for(int i = 0; i < stickCount; i++)
        {
            ropeSticks.Add(new RopeStick(ropeBody[i], ropeBody[i + 1], this.segmentLength));
        }
        this.currentLength = stickCount * this.segmentLength;

        ApplyLength();
    }

    /// <summary>
    /// Call this onece per fixed update to apply changes in length
    /// </summary>
    // TODO: Make this directional?
    public void ApplyLength()
    {
        Debug.Log("apply length!");
        while (currentLength < goalLength)
        {
            if ((ropeSticks.Count > 0) && ((currentLength - ropeSticks[0].Length) + segmentLength >= goalLength))
            {
                currentLength -= ropeSticks[0].Length;
                ropeSticks[0].Length = goalLength - (currentLength - ropeSticks[0].Length);
                currentLength += ropeSticks[0].Length;
            }
            else
            {
                if (currentLength + MIN_SEGMENT_LENGTH > goalLength) break;

                if (ropeSticks.Count > 0)
                {
                    currentLength -= ropeSticks[0].Length;
                    ropeSticks[0].Length = SegmentLength;
                    currentLength += ropeSticks[0].Length;
                }

                // makes sure the tip point stays locked
                if (ropeBody.Count > 0)
                {
                    //Rope.Points[0].IsLocked = false;
                    ropeBody[0].Position += Vector3.down * .1f;
                    // COME UP WITH BETTER SOLUTION THAN THIS
                }

                ropeBody.AddRopePoint(new RopePoint(ropeBody[0].Position - Vector3.down * .1f), true);
                ropeSticks.Add(new RopeStick(ropeBody[0], ropeBody[1], segmentLength));

                //InsertPoint(0, ropeBody[0].Position - Vector3.down * .1f, segmentLength);

                currentLength += SegmentLength;
            }
        }
        while (currentLength > goalLength)
        {
            //Debug.Log("prev: " + prevRopeLength);
            //Debug.Log("new: " + newRopeLength);
            //Debug.Log("stick length: " + sticks[0].Length);
            //Debug.Log("stick count: " + sticks.Count);
            //Debug.Log("point count: " + rope.Points.Count);
            if (ropeSticks.Count == 0) break;

            //if ((prevRopeLength - sticks[0].Length) > newRopeLength)
            if ((currentLength - ropeSticks[0].Length) > goalLength)
            {
                currentLength -= ropeSticks[0].Length;
                ropeBody.RemoveRopePoint(true);
                //RemovePointAt(0);
                // remove attachments if their point index is higher than the number of points that exist
                // either that or move them down by one?
                // probably better not to move them down and just implement -1 for something that is always supposed to 
                // be attached to the end.
                //if(rope.Points.Count < rope.Attachments[])
                //rope.Points[0].IsLocked = true;
            }
            else
            {
                //if (newRopeLength - (prevRopeLength - sticks[0].Length) < MIN_SEGMENT_LENGTH) break;
                if (goalLength - (currentLength - ropeSticks[0].Length) < MIN_SEGMENT_LENGTH)
                {
                    currentLength -= (ropeSticks[0].Length - MIN_SEGMENT_LENGTH);
                    //Debug.Log("stick diff: " + (sticks[0].Length - MIN_SEGMENT_LENGTH) + " prev len: " + prevRopeLength);
                    ropeSticks[0].Length = MIN_SEGMENT_LENGTH;
                    break;
                }
                currentLength -= ropeSticks[0].Length;
                ropeSticks[0].Length = goalLength - currentLength;
                currentLength += ropeSticks[0].Length;
            }
        }

        // dont return anything because current length is stored as a class variable
    }

    public IEnumerator<RopeStick> GetEnumerator()
    {
        foreach(RopeStick s in ropeSticks)
        {
            yield return s;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int IndexOf(RopeStick item) => ropeSticks.IndexOf(item);

    public void Insert(int index, RopeStick item) => ropeSticks.Insert(index, item);

    public void RemoveAt(int index) => ropeSticks.RemoveAt(index);

    public void Add(RopeStick item) => ropeSticks.Add(item);

    public void Clear() => ropeSticks.Clear();

    public bool Contains(RopeStick item) => ropeSticks.Contains(item);

    public void CopyTo(RopeStick[] array, int arrayIndex) => ropeSticks.CopyTo(array, arrayIndex);

    public bool Remove(RopeStick item) => ropeSticks.Remove(item);
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
    public RopeStick(RopePoint pointA, RopeStick stickToReplace)
    {
        this.pointA = pointA;
        this.pointB = stickToReplace.pointB;
        this.length = stickToReplace.length;
    }
    public RopeStick(RopeStick stickToReplace, RopePoint pointB)
    {
        this.pointA = stickToReplace.pointA;
        this.pointB = pointB;
        this.length = stickToReplace.length;
    }
}
