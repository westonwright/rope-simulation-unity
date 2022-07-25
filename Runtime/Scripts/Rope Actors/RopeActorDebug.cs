using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeActorDebug : RopeActorBase
{
    private RopeSimulatorDebug ropeSimulatorDebug;
    public RopeSimulatorDebug RopeSimulatorDebug { get { return ropeSimulatorDebug; } }

    public RopeActorDebug(
        Rope rope,
        Color lineColor
        )
        : base(rope)
    {
        this.ropeSimulatorDebug = new RopeSimulatorDebug(
            lineColor
            );

        actionExecutions.Add((ExecutionOrder, DrawLines));
    }

    public override int ExecutionOrder { get { return int.MaxValue; } }

    public Color LineColor { get => ropeSimulatorDebug.LineColor; set => ropeSimulatorDebug.LineColor = value; }

    private void DrawLines()
    {
        ropeSimulatorDebug.DrawLines(Rope.Sticks);
    }
}
