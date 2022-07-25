using UnityEngine;

public abstract class RopeAttachment
{
    private int pointIndex;
    public int RawPointIndex { get => pointIndex; } // this can be negative
    public int CorrectedPointIndex(int pointCount)
    {
        return pointIndex >= 0 ? pointIndex : Mathf.Max(pointCount + pointIndex, 0);
    }

    public abstract Vector3 Position { get; }
    public abstract Rigidbody Rigidbody { get; }

    public RopeAttachment(int pointIndex)
    {
        this.pointIndex = pointIndex;
    }
}

public class RopeAttachmentPoint : RopeAttachment
{
    private Vector3 position;

    public RopeAttachmentPoint(int pointIndex, Vector3 position) : base(pointIndex)
    {
        this.position = position;
    }

    public override Vector3 Position { get => position; }
    public override Rigidbody Rigidbody { get => null; }
}

public class RopeAttachmentTransform : RopeAttachment
{
    private Transform transform;
    private Vector3 offset;

    public RopeAttachmentTransform(int pointIndex, Transform transform, Vector3 offset) : base(pointIndex)
    {
        this.transform = transform;
        this.offset = offset;
    }

    public override Vector3 Position { get => transform.TransformPoint(offset); }
    public override Rigidbody Rigidbody { get => null; }
}

public class RopeAttachmentRigidbody : RopeAttachmentTransform
{
    private Rigidbody rigidbody;

    public RopeAttachmentRigidbody(int pointIndex, Transform transform, Vector3 offset, Rigidbody rigidbody) : base(pointIndex, transform, offset)
    {
        this.rigidbody = rigidbody;
    }

    public override Rigidbody Rigidbody { get => rigidbody; }
}
