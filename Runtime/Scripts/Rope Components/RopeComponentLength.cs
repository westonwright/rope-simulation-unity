using System;
using System.Collections.Generic;
using UnityEngine;
/*
public interface IRopeLength
{
    /// <summary>
    /// The initial length of the rope
    /// </summary>
    float CurrentLength { get; set; }
    /// <summary>
    /// The maximum total length allowed for the rope
    /// </summary>
    float MaxRopeLength { get; set; }
    /// <summary>
    /// The minimum total length allowed for the rope
    /// </summary>
    float MinRopeLength { get; set; }
    /// <summary>
    /// How long a single segment or "stick" of rope is. aka: the distance between any two points
    /// </summary>
    float SegmentLength { get; set; }
    /// <summary>
    /// What percent of the total length of the rope is out
    /// </summary>
    float PercentReleased { get; set; }
}
*/
[RequireComponent(typeof(RopeGameObject))]
public class RopeComponentLength : RopeComponentBase
{
    [SerializeField]
    private float goalLength = 5f;
    [SerializeField]
    private float maxRopeLength = 10f;
    [SerializeField]
    private float minRopeLength = .1f;
    [SerializeField]
    private float segmentLength = .2f;

    private RopeActorLength ropeActorLength;
    public override RopeActorBase RopeActor { get { return ropeActorLength; } }

    public override void Initialize(Rope rope, IEnumerable<RopeActorBase> requiredActors)
    {
        ropeActorLength = new RopeActorLength(
            rope,
            goalLength,
            maxRopeLength,
            minRopeLength,
            segmentLength
            );
        ropeActorLength.EnableActor();
    }

    protected override void OnValidate()
    {
        goalLength = Mathf.Clamp(goalLength, minRopeLength, maxRopeLength);
        maxRopeLength = Mathf.Max(maxRopeLength, Mathf.Max(0, minRopeLength) + .1f);
        minRopeLength = Mathf.Min(Mathf.Max(minRopeLength, 0), maxRopeLength);
    }

    public float CurentLength { get => ropeActorLength.CurrentLength; }
    public float GoalLength { set => ropeActorLength.GoalLength = value; }
    public float PercentReleased { get => ropeActorLength.PercentReleased; set => ropeActorLength.PercentReleased = value; }
    public float MaxRopeLength { get => ropeActorLength.MaxRopeLength; set => ropeActorLength.MaxRopeLength = value; }
    public float MinRopeLength { get => ropeActorLength.MinRopeLength; set => ropeActorLength.MinRopeLength = value; }
    public float SegmentLength { get => ropeActorLength.SegmentLength; set => ropeActorLength.SegmentLength = value; }
}


