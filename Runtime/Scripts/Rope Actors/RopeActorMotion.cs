using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeActorMotion : RopeActorBase
{
    private RopeSimulatorMotion ropeSimulatorMotion;
    public RopeSimulatorMotion RopeSimulatorMotion { get { return ropeSimulatorMotion; } }

    private RopeActorLength ropeActorLength;
    public RopeActorLength RopeActorLength { get { return ropeActorLength; } }

    public RopeActorMotion(
        Rope rope,
        RopeActorLength ropeActorLength,
        int movementIterations,
        float maximumAngle,
        float ropeDrag
        )
        : base(rope)
    {
        this.ropeSimulatorMotion = new RopeSimulatorMotion(
            movementIterations,
            maximumAngle,
            ropeDrag
            );

        this.ropeActorLength = ropeActorLength;
        actionExecutions.Add(new RopeActionExecution(ExecutionOrder, UpdateRope));
    }
    public override int ExecutionOrder { get { return 0; } }

    public int MovementIterations { get => ropeSimulatorMotion.MovementIterations; set => ropeSimulatorMotion.MovementIterations = value; }
    public float MaximumAngle { get => ropeSimulatorMotion.MaximumAngle; set => ropeSimulatorMotion.MaximumAngle = value; }
    public float RopeDrag { get => ropeSimulatorMotion.RopeDrag; set => ropeSimulatorMotion.RopeDrag = value; }

    private void UpdateRope()
    {
        ropeSimulatorMotion.ApplyVelocity(Rope.RopeBody);
        ropeSimulatorMotion.ApplyMotion(ropeActorLength.RopeSimulatorLength);
    }
}

