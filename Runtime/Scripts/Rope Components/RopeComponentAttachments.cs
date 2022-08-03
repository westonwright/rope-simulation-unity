using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RopeComponentAttachments : RopeComponentBase
{
    [SerializeField]
    private Transform[] initialAttachmentBodies;
    [SerializeField]
    private int[] initialAttachmentIndexes;
    private RopeAttachment[] initialAttachments;
    public RopeAttachment[] InitialAttachments {
        get 
        { 
            if(initialAttachments == null)
            {
                List<RopeAttachment> ropeAttachments = new List<RopeAttachment>();
                for (int i = 0; i < initialAttachmentBodies.Length; i++)
                {
                    int attachIndex = -1;
                    if (i < initialAttachmentIndexes.Length)
                    {
                        attachIndex = initialAttachmentIndexes[i];
                    }
                    Rigidbody rb = initialAttachmentBodies[i].GetComponent<Rigidbody>();
                    RopeAttachment ropeAttachment = null;
                    if (rb != null)
                    {
                        ropeAttachment = new RopeAttachmentRigidbody(attachIndex, initialAttachmentBodies[i], Vector3.up * .5f, rb);
                    }
                    else
                    {
                        ropeAttachment = new RopeAttachmentTransform(attachIndex, initialAttachmentBodies[i], Vector3.zero);
                    }
                    ropeAttachments.Add(ropeAttachment);
                }
                initialAttachments = ropeAttachments.ToArray();
            }
            return initialAttachments;
        } 
    }

    private RopeActorAttachments ropeActorAttachments;
    public override RopeActorBase RopeActor { get { return ropeActorAttachments; } }

    public override void Initialize(Rope rope, IEnumerable<RopeActorBase> requiredActors)
    {
        ropeActorAttachments = new RopeActorAttachments(
            rope,
            InitialAttachments
            );
        foreach (RopeAttachment ropeAttachment in initialAttachments)
        {
            ropeActorAttachments.AddAttachment(ropeAttachment);
        }
        ropeActorAttachments.EnableActor();

    }

    public void RemoveAttachment(int index)
    {
        ropeActorAttachments.RemoveAttachment(index);
    }

    public void AddAttachment(int pointIndex, Vector3 position)
    {
        ropeActorAttachments.AddAttachment(new RopeAttachmentPoint(pointIndex, position));
    }

    public void AddAttachment(int pointIndex, Transform transform, Vector3 offset)
    {
        ropeActorAttachments.AddAttachment(new RopeAttachmentTransform(pointIndex, transform, offset));
    }

    public void AddAttachment(int pointIndex, Transform transform, Vector3 offset, Rigidbody rigidbody)
    {
        ropeActorAttachments.AddAttachment(new RopeAttachmentRigidbody(pointIndex, transform, offset, rigidbody));
    }
}
