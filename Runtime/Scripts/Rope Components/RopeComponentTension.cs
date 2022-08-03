using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/*
public interface IRopeTension
{
    public bool TensionEnabled { get; set; }
    /// <summary>
    /// How far the rope needs to be stretched beyond its maximum to start exerting force
    /// </summary>
    public float ThresholdTension { get; set; }
    /// <summary>
    /// How strongly the rope pulls back when the threshold tension is reached
    /// </summary>
    public float SpringStrength { get; set; }
    /// <summary>
    /// How strongly outward movement is reduced on rigidbodies attached to the tips of the rope depending on threshold tension.
    /// </summary>
    public float DampingStrength { get; set; }
}
*/

[RequireComponent(typeof(RopeGameObject), typeof(RopeComponentRigidbody))]
public class RopeComponentTension : RopeComponentBase
{
    [SerializeField]
    private float thresholdTension = 1.33f;
    [SerializeField]
    private float springStrength = 100f;
    [SerializeField]
    [Range(0f, 1f)]
    private float dampingStrength = .1f;

    private RopeActorTension ropeActorTension;
    public override RopeActorBase RopeActor { get { return ropeActorTension; } }

    protected override void Start()
    {
        requiredActorTypes.Add(typeof(RopeActorRigidbody));
        base.Start();
    }

    public override void Initialize(Rope rope, IEnumerable<RopeActorBase> requiredActors)
    {
        RopeActorRigidbody ropeActorRigidbody = requiredActors.FirstOrDefault(x => x.GetType() == typeof(RopeActorRigidbody)) as RopeActorRigidbody;
        // if (ropeRigidbody == null) 
        ropeActorTension = new RopeActorTension(
            rope,
            ropeActorRigidbody,
            thresholdTension,
            springStrength,
            dampingStrength
            );
        ropeActorTension.EnableActor();
    }

    protected override void OnValidate()
    {
        thresholdTension = Mathf.Max(1, thresholdTension);
        springStrength = Mathf.Max(0, springStrength);
    }

    public float ThresholdTension { get => ropeActorTension.ThresholdTension; set => ropeActorTension.ThresholdTension = value; }
    public float SpringStrength { get => ropeActorTension.SpringStrength; set => ropeActorTension.SpringStrength = value; }
    public float DampingStrength { get => ropeActorTension.DampingStrength; set => ropeActorTension.DampingStrength = value; }
}

