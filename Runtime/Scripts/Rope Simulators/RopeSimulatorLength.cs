using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RopeSimulatorLength
{
    private readonly float MIN_SEGMENT_LENGTH = .1f;

    private float goalLength = 5f;
    public float GoalLength
    {
        set
        {
            // add debug log if value is set too high or too low?
            goalLength = Mathf.Clamp(value, minRopeLength, maxRopeLength);
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
            return (CurrentLength - MinRopeLength) / (MaxRopeLength - MinRopeLength);
        }
        set
        {
            float percent = Mathf.Clamp01(value);
            GoalLength = ((MaxRopeLength - MinRopeLength) * percent) + MinRopeLength;
        }
    }

    public RopeSimulatorLength(
        float currentLength,
        float goalLength,
        float maxRopeLength,
        float minRopeLength, 
        float segmentLength
        )
    {
        this.currentLength = currentLength;
        this.goalLength = goalLength;
        this.maxRopeLength = maxRopeLength;
        this.minRopeLength = minRopeLength;
        this.segmentLength = segmentLength;
    }

    /// <summary>
    /// Call this onece per fixed update to apply changes in length
    /// </summary>
    public void ApplyLength(IList<RopeStick> sticks, IList<RopePoint> points, Action<int, Vector3, float> InsertPoint, Action<int> RemovePointAt)
    {

        while (currentLength < goalLength)
        {
            if ((sticks.Count > 0) && ((currentLength - sticks[0].Length) + segmentLength >= goalLength))
            {
                currentLength -= sticks[0].Length;
                sticks[0].Length = goalLength - (currentLength - sticks[0].Length);
                currentLength += sticks[0].Length;
            }
            else
            {
                if (currentLength + MIN_SEGMENT_LENGTH > goalLength) break;

                if (sticks.Count > 0)
                {
                    currentLength -= sticks[0].Length;
                    sticks[0].Length = SegmentLength;
                    currentLength += sticks[0].Length;
                }

                // makes sure the tip point stays locked
                if (points.Count > 0)
                {
                    //Rope.Points[0].IsLocked = false;
                    points[0].Position += Vector3.down * .1f;
                    // COME UP WITH BETTER SOLUTION THAN THIS
                }

                //InsertPoint(sticks, points, new RopePoint(points[0].Position - Vector3.down * .1f), 0, ResolveAttachments);
                InsertPoint(0, points[0].Position - Vector3.down * .1f, segmentLength);

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
            if (sticks.Count == 0) break;

            //if ((prevRopeLength - sticks[0].Length) > newRopeLength)
            if ((currentLength - sticks[0].Length) > goalLength)
            {
                currentLength -= sticks[0].Length;
                RemovePointAt(0);
                //rope.Points.RemoveAt(0);
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
                if (goalLength - (currentLength - sticks[0].Length) < MIN_SEGMENT_LENGTH)
                {
                    currentLength -= (sticks[0].Length - MIN_SEGMENT_LENGTH);
                    //Debug.Log("stick diff: " + (sticks[0].Length - MIN_SEGMENT_LENGTH) + " prev len: " + prevRopeLength);
                    sticks[0].Length = MIN_SEGMENT_LENGTH;
                    break;
                }
                currentLength -= sticks[0].Length;
                sticks[0].Length = goalLength - currentLength;
                currentLength += sticks[0].Length;
            }
        }

        // dont return anything because current length is stored as a class variable
    }
}
