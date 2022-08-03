using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeUpdater
{
    private List<RopeActionExecution> actionExecutions = new List<RopeActionExecution>();

    protected float currentTimeStep;
    public float CurrentTimeStep { get { return currentTimeStep; } }

    public void AddActionExecution(RopeActionExecution actionExecution)
    {
        actionExecutions.Add(actionExecution);
        actionExecutions.Sort((x, y) => x.Order.CompareTo(y.Order));
    }

    public void RemoveActionExecution(RopeActionExecution actionExecution)
    {
        actionExecutions.Remove(actionExecution);
    }

    public void UpdateRope(float timeStep)
    {
        currentTimeStep = timeStep;
        // move this into action executions too?

        // execute the actions of each component in order
        foreach (RopeActionExecution actionExecution in actionExecutions)
        {
            actionExecution.Action();
        }
    }
}
