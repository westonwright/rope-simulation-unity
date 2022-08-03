using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeActorLineRenderer : RopeActorBase
{
    private RopeSimulatorLineRenderer ropeSimulatorLineRenderer;
    public RopeSimulatorLineRenderer RopeSimulatorLineRenderer { get { return ropeSimulatorLineRenderer; } }

    public RopeActorLineRenderer(
        Rope rope, 
        LineRenderer lineRenderer,
        float smoothingResolution
        ) : base(rope)
    {
        ropeSimulatorLineRenderer = new RopeSimulatorLineRenderer(
            lineRenderer,
            smoothingResolution
            );

        actionExecutions.Add(new RopeActionExecution(ExecutionOrder, UpdateLineRenderer));
    }

    public override int ExecutionOrder { get { return int.MaxValue; } }

    public float SmoothingResolution { get => ropeSimulatorLineRenderer.SmoothingResolution; set => ropeSimulatorLineRenderer.SmoothingResolution = value; }

    private void UpdateLineRenderer()
    {
        ropeSimulatorLineRenderer.UpdateLineRenderer(Rope.RopeBody);
    }
}
