using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeActorLength : RopeActorBase
{
    private RopeSimulatorLength ropeSimulatorLength;
    public RopeSimulatorLength RopeSimulatorLength { get { return ropeSimulatorLength; } }

    public RopeActorLength(
        Rope rope,
        float goalLength = 5,
        float maxRopeLength = 10,
        float minRopeLength = .1f,
        float segmentLength = .5f
        ) : base(rope)
    {
        ropeSimulatorLength = new RopeSimulatorLength(
            rope.RopeBody,
            goalLength,
            maxRopeLength,
            minRopeLength,
            segmentLength
            );

        actionExecutions.Add(new RopeActionExecution(ExecutionOrder, ApplyLength));
    }

    public override int ExecutionOrder { get { return -20; } }

    public float GoalLength { set => ropeSimulatorLength.GoalLength = value; }
    
    public float CurrentLength { get => ropeSimulatorLength.CurrentLength; }

    public float MaxRopeLength { get => ropeSimulatorLength.MaxRopeLength; set => ropeSimulatorLength.MaxRopeLength = value; }

    public float MinRopeLength { get => ropeSimulatorLength.MinRopeLength; set => ropeSimulatorLength.MinRopeLength = value; }

    public float SegmentLength { get => ropeSimulatorLength.SegmentLength; set => ropeSimulatorLength.SegmentLength = value; }

    public float PercentReleased { get => ropeSimulatorLength.PercentReleased; set => ropeSimulatorLength.PercentReleased = value; }

    private void ApplyLength()
    {
        ropeSimulatorLength.ApplyLength();
    }

}

