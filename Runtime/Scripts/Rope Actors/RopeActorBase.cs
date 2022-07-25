using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class RopeActorBase
{
    private Rope rope;
    public Rope Rope { get { return rope; } }

    public abstract int ExecutionOrder { get; }

    protected List<(int, Action)> actionExecutions = new List<(int, Action)>();
    /// <summary>
    /// tuple with execution order, then action to execute
    /// </summary>
    public List<(int, Action)> ActionExecutions { get { return actionExecutions; } }

    public RopeActorBase(Rope rope)
    {
        this.rope = rope;
    }
}
