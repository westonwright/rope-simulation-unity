using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSimulatorForces
{
    private float pointMass = .01f;
    public float PointMass { get => pointMass; set => pointMass = Mathf.Max(.01f, value); }

    public RopeSimulatorForces(
        float pointMass
        )
    {
        this.pointMass = pointMass;
    }

    /// <summary>
    /// Applies a force to a point. Must be run before ApplyForce!
    /// </summary>
    /// <param name="forceVector">The direciton and strength of the force</param>
    /// <param name="forceMode">How to apply the force</param>
    /// <param name="timeStep">The delta time since last rope update</param>
    /// <param name="point">The point to be modified</param>
    public void AccumulateForce<T>(Vector3 forceVector, ForceMode forceMode, float timeStep, T point)
        where T : IRopePointAccumulatedForce, IRopePointIsAttached
    {
        if (!point.IsAttached)
        {
            switch (forceMode)
            {
                case ForceMode.Force:
                    point.AccumulatedForce += (forceVector / pointMass) * timeStep * timeStep;
                    break;
                case ForceMode.Acceleration:
                    point.AccumulatedForce += forceVector * timeStep * timeStep;
                    break;
                case ForceMode.Impulse:
                    point.AccumulatedForce += forceVector / pointMass;
                    break;
                case ForceMode.VelocityChange:
                    point.AccumulatedForce += forceVector;
                    break;
            }
        }
    }

    /// <summary>
    /// Applies a force to the entire rope. Must be run before ApplyForce
    /// </summary>
    /// <param name="forceVector">The direciton and strength of the force</param>
    /// <param name="forceMode">How to apply the force</param>
    /// <param name="timeStep">The delta time since last rope update</param>
    /// <param name="points">The list of points to be modified</param>
    public void AccumulateForce<T>(Vector3 forceVector, ForceMode forceMode, float timeStep, IEnumerable<T> points)
        where T : IRopePointAccumulatedForce, IRopePointIsAttached
    {
        foreach (T p in points)
        {
            AccumulateForce(forceVector, forceMode, timeStep, p);
        }
    }
}
