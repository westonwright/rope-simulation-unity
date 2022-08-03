using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeActorDebug : RopeActorBase
{
    private RopeSimulatorDebug ropeSimulatorDebug;
    public RopeSimulatorDebug RopeSimulatorDebug { get { return ropeSimulatorDebug; } }

    private RopeActorLength ropeActorLength;
    public RopeActorLength RopeActorLength { get { return ropeActorLength; } }

    public RopeActorDebug(
        Rope rope,
        RopeActorLength ropeActorLength,
        Color lineColor
        )
        : base(rope)
    {
        this.ropeActorLength = ropeActorLength;
        this.ropeSimulatorDebug = new RopeSimulatorDebug(
            lineColor
            );

        actionExecutions.Add(new RopeActionExecution(ExecutionOrder, DrawLines));
    }

    public override int ExecutionOrder { get { return int.MaxValue; } }

    public Color LineColor { get => ropeSimulatorDebug.LineColor; set => ropeSimulatorDebug.LineColor = value; }

    private void DrawLines()
    {
        ropeSimulatorDebug.DrawLines(ropeActorLength.RopeSimulatorLength);
    }
}
