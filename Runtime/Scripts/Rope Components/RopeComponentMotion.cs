using System;
using System.Collections.Generic;
using UnityEngine;
/*
public interface IRopeMotion
{
    /// <summary>
    /// How many iterations the rope will calculate per frame for movement. 
    /// Higher leads to smoother results and less stretch
    /// </summary>   
    public int MovementIterations { get; set; }

    /// <summary>
    /// How far the rope is able to bend between segments in degrees
    /// </summary>
    public float MaximumAngle { get; set; }

    /// <summary>
    /// How much drag will be exerted on the rope while moving through the air
    /// Must be between 0 and 1
    /// </summary>
    public float RopeDrag { get; set; }

    /// <summary>
    /// The mass of a single segment on the rope.
    /// </summary>
    public float PointMass { get; set; }

    void UpdateRope(float timeStep);

    void ApplyForce(Vector3 forceVector, ForceMode forceMode, float timeStep);
    // different version for applying for to single points or interpolating force across points?
}
*/

[RequireComponent(typeof(RopeGameObject))]
public class RopeComponentMotion : RopeComponentBase
{
    [SerializeField]
    private int movementIterations = 10;
    [SerializeField]
    [Range(.1f, 359.9f)]
    private float maximumAngle = 60f;
    [SerializeField]
    [Range(0, 1)]
    private float ropeDrag = .01f;

    private RopeActorMotion ropeActorMotion;
    public override RopeActorBase RopeActor { get { return ropeActorMotion; } }

    public override void Initialize(Rope rope, IEnumerable<RopeActorBase> requiredActors)
    {
        ropeActorMotion = new RopeActorMotion(
            rope,
            movementIterations,
            maximumAngle,
            ropeDrag
            );
    }

    protected override void OnValidate()
    {
        movementIterations = Mathf.Max(movementIterations, 1);
    }

    public int MovementIterations { get => ropeActorMotion.MovementIterations; set => ropeActorMotion.MovementIterations = value; }
    public float MaximumAngle { get => ropeActorMotion.MaximumAngle; set => ropeActorMotion.MaximumAngle = value; }
    public float RopeDrag { get => ropeActorMotion.RopeDrag; set => ropeActorMotion.RopeDrag = value; }
}

