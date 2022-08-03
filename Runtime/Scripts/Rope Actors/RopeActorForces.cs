using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeActorForces : RopeActorBase
{
    private RopeSimulatorForces ropeSimulatorForces;
    public RopeSimulatorForces RopeSimulatorForces { get { return ropeSimulatorForces; } }

    private RopeActorMotion ropeActorMotion;
    public RopeActorMotion RopeActorMotion { get { return ropeActorMotion; } }

    public RopeActorForces(
        Rope rope,
        float pointMass
        )
        : base(rope)
    {
        this.ropeSimulatorForces = new RopeSimulatorForces(
            pointMass
            );
    }

    public override int ExecutionOrder { get { return -10; } }

    public float PointMass { get => ropeSimulatorForces.PointMass; set => ropeSimulatorForces.PointMass = value; }
   
    // could accumulate forces over a frame then apply them but not doing that yet
    public void ApplyForce(Vector3 forceVector, ForceMode forceMode, float timeStep) =>
        ropeSimulatorForces.AccumulateForce(forceVector, forceMode, timeStep, Rope.RopeBody);

    public void ApplyForce(Vector3 forceVector, ForceMode forceMode, float timeStep, RopePoint point) =>
        ropeSimulatorForces.AccumulateForce(forceVector, forceMode, timeStep, point);
}
