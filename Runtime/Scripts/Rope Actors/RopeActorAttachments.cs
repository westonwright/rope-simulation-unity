using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeActorAttachments : RopeActorBase
{
    private RopeSimulatorAttachments ropeSimulatorAttachments;
    public RopeSimulatorAttachments RopeSimulatorAttachments { get { return ropeSimulatorAttachments; } }


    public RopeActorAttachments(
        Rope rope,
        IEnumerable<RopeAttachment> initialAttachments
        ) : base(rope)
    {
        ropeSimulatorAttachments = new RopeSimulatorAttachments(rope.RopeBody, initialAttachments);
        actionExecutions.Add(new RopeActionExecution(ExecutionOrder, MovePointsToAttachments));
    }
    public override int ExecutionOrder { get { return -15; } }

    public void MovePointsToAttachments()
    {
        ropeSimulatorAttachments.MovePointsToAttachments();
    }

    public void RemoveAttachment(int index)
    {
        ropeSimulatorAttachments.RemoveAttachment(index);
    }

    public void AddAttachment(RopeAttachment attachment)
    {
        ropeSimulatorAttachments.AddAttachment(attachment);
    }

}
