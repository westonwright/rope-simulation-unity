using System.Collections;
using System.Collections.Generic;
using System;

public abstract class RopeActorBase
{
    private Rope rope;
    public Rope Rope { get { return rope; } }

    public abstract int ExecutionOrder { get; }

    protected List<RopeActionExecution> actionExecutions = new List<RopeActionExecution>();

    public RopeActorBase(Rope rope)
    {
        this.rope = rope;
    }

    public virtual void EnableActor()
    {
        foreach(RopeActionExecution actionExecution in actionExecutions)
        {
            rope.RopeUpdater.AddActionExecution(actionExecution);
        }
    }

    public virtual void DisableActor()
    {
        foreach (RopeActionExecution actionExecution in actionExecutions)
        {
            rope.RopeUpdater.RemoveActionExecution(actionExecution);
        }
    }
}
