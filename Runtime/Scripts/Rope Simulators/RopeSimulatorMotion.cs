using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSimulatorMotion
{
    private int movementIterations = 10;
    public int MovementIterations { get => movementIterations; set => movementIterations = Mathf.Max(1, value); }

    private float maximumAngle = 60f;
    public float MaximumAngle { get => maximumAngle; set => maximumAngle = Mathf.Clamp(value, 0, 359); }

    private float ropeDrag = .1f;
    public float RopeDrag { get => ropeDrag; set => ropeDrag = Mathf.Clamp01(value); }

    private SetStickPosition1Delegate setStickPosition1;
    public SetStickPosition1Delegate SetStickPosition1 {
        get
        {
            if(setStickPosition1 == null)
            {
                setStickPosition1 = DefaultSetStickPosition1;
            }
            return setStickPosition1;
        }
        set
        {
            if(value == null)
            {
                setStickPosition1 = DefaultSetStickPosition1;
            }
            else
            {
                setStickPosition1 = value;
            }
        }
    }
    public SetStickPosition1Delegate GetDefaultSetStickPosition1{ get { return DefaultSetStickPosition1; } }
    
    private SetStickPosition2Delegate setStickPosition2;
    public SetStickPosition2Delegate SetStickPosition2 {
        get
        {
            if(setStickPosition2 == null)
            {
                setStickPosition2 = DefaultSetStickPosition2;
            }
            return setStickPosition2;
        }
        set
        {
            if(value == null)
            {
                setStickPosition2 = DefaultSetStickPosition2;
            }
            else
            {
                setStickPosition2 = value;
            }
        }
    }
    public SetStickPosition2Delegate GetDefaultSetStickPosition2 { get { return DefaultSetStickPosition2; } }


    public RopeSimulatorMotion(
        int movementIterations = 10,
        float maximumAngle = 60f,
        float ropeDrag = .1f
        )
    {
        this.movementIterations = movementIterations;
        this.maximumAngle = maximumAngle;
        this.ropeDrag = ropeDrag;
    }

    /// <summary>
    /// Applies forces and velocity to the rope after theyve been accumulated.
    /// This should be called every time the rope is updated
    /// </summary>
    /// <param name="points">The list of points to be modified</param>
    public void ApplyVelocity<T>(IEnumerable<T> points)
        where T : IRopePointPosition, IRopePointPrevPosition, IRopePointAccumulatedForce, IRopePointIsAttached
    {
        foreach (T p in points)
        {
            if (!p.IsAttached)
            {
                Vector3 positionBeforeUpdate = p.Position;
                // adds current velocity
                p.Position += (p.Position - p.PrevPosition) * (1 - ropeDrag); //multiply for drag
                p.Position += p.AccumulatedForce;
                p.PrevPosition = positionBeforeUpdate;
                // reset accumulated force to start building for next update
                p.AccumulatedForce = Vector3.zero;
            }
        }

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sticks">The list of sticks to be modified</param>
    public void ApplyMotion(IList<RopeStick> sticks)
    {
        for (int i = 0; i < movementIterations; i++)
        {
            bool order = i % 2 == 0;
            Vector3 prevStickDir = Vector3.zero;
            //read forwards then backwards along the rope to improve stability
            for (
                int j = order ? (sticks.Count - 1) : 0;
                order ? (j >= 0) : (j < sticks.Count);
                j += order ? -1 : 1
                )
            {
                RopeStick s = sticks[j];
                RopePoint p1 = order ? s.PointA : s.PointB;
                RopePoint p2 = order ? s.PointB : s.PointA;

                float stickDist = Vector3.Distance(p2.Position, p1.Position);
                Vector3 stickDir;
                if (stickDist != 0)
                {
                    stickDir = (p1.Position - p2.Position).normalized;
                }
                else
                {
                    stickDir = Vector3.down;
                }
                Vector3 stickCenter = (p1.Position + p2.Position) / 2;

                if (!p1.IsAttached)
                {
                    //limit angle
                    if (prevStickDir != Vector3.zero && Vector3.Angle(prevStickDir, stickDir) > maximumAngle)
                    {
                        stickDir = Quaternion.AngleAxis(maximumAngle, (Vector3.Cross(prevStickDir, stickDir))) * prevStickDir;
                    }
                    SetStickPosition1(ref s, ref stickCenter, ref stickDir, order, sticks.Count - j);
                }

                if (!p2.IsAttached)
                {
                    SetStickPosition2(ref s, ref stickCenter, ref stickDir, order);
                }

                prevStickDir = stickDir;
                
                /*
                Debug.DrawRay(p2.Position, stickDir * s.Length, Color.Lerp(Color.black, order ? Color.green : Color.red, (float)i / movementIterations));
                if (i == movementIterations - 1)
                {
                    //Debug.DrawRay(p2.Position, stickDir * s.Length, Color.blue);
                }
                */
            }
        }
    }

    /// <summary>
    /// Set the final stick position for an iteration. This is for the first point on the stick
    /// </summary>
    /// <param name="s">The stick to use</param>
    /// <param name="stickCenter">The center of the stick</param>
    /// <param name="stickDirection">The direction of the stick</param>
    /// <param name="order">Are we reading forward or backward</param>
    private void DefaultSetStickPosition1(ref RopeStick s, ref Vector3 stickCenter, ref Vector3 stickDirection, bool order, int stickIndex = 0)
    {
        IRopePointPosition p = order ? s.PointA : s.PointB;
        p.Position = stickCenter + stickDirection * s.Length / 2;
    }
    /// <summary>
    /// Set the final stick position for an iteration. This is for the second point on the stick
    /// </summary>
    /// <param name="s">The stick to use</param>
    /// <param name="stickCenter">The center of the stick</param>
    /// <param name="stickDirection">The direction of the stick</param>
    /// <param name="order">Are we reading forward or backward</param>
    private void DefaultSetStickPosition2(ref RopeStick s, ref Vector3 stickCenter, ref Vector3 stickDirection, bool order)
    {
        IRopePointPosition p = order ? s.PointB : s.PointA;
        p.Position = stickCenter - stickDirection * s.Length / 2;
    }
}
