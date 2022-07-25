using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/*
public interface IRopeRigidbody
{

    public bool CollisionEnabled { get; set; }
    /// <summary>
    /// How many times collision will be checked for an optimal result during each movement iteration
    /// </summary>
    public int CollisionIterations { get; set; }

    /// <summary>
    /// call this if the game objects layer has changed or the collision matrix has changed
    /// </summary>
    public void UpdateLayermask();
}
*/
[RequireComponent(typeof(RopeGameObject), typeof(RopeComponentMotion))]
public class RopeComponentRigidbody : RopeComponentBase
{
    [SerializeField]
    private bool collisionEnabled = true;
    [SerializeField]
    private int collisionIterations = 2;

    private RopeActorRigidbody ropeActorRigidbody;
    public override RopeActorBase RopeActor { get { return ropeActorRigidbody; } }

    protected override void Start()
    {
        requiredActorTypes.Add(typeof(RopeActorMotion));
        base.Start();
    }

    public override void Initialize(Rope rope, IEnumerable<RopeActorBase> requiredActors)
    {
        RopeActorMotion ropeActorMotion = requiredActors.FirstOrDefault(x => x.GetType() == typeof(RopeActorMotion)) as RopeActorMotion;
        // if (ropeMotion == null) 
        ropeActorRigidbody = new RopeActorRigidbody(
            rope,
            ropeActorMotion,
            collisionEnabled,
            collisionIterations
            );
    }
    protected override void OnValidate()
    {
        collisionIterations = Mathf.Max(collisionIterations, 1);
    }

    public bool CollisionEnabled { get => ropeActorRigidbody.CollisionEnabled; set => ropeActorRigidbody.CollisionEnabled = value; }
    public int CollisionIterations { get => ropeActorRigidbody.CollisionIterations; set => ropeActorRigidbody.CollisionIterations = value; }

}

