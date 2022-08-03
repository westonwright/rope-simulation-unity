using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RopeGameObject), typeof(RopeComponentMotion))]
public class RopeComponentForces : RopeComponentBase
{
    [SerializeField]
    private float pointMass = .01f;

    private RopeActorForces ropeActorForces;
    public override RopeActorBase RopeActor { get { return ropeActorForces; } }

    protected override void Start()
    {
        requiredActorTypes.Add(typeof(RopeActorMotion));
        base.Start();
    }

    public override void Initialize(Rope rope, IEnumerable<RopeActorBase> requiredActors)
    {
        // even though motion is required we dont actually use it so we can just ignore it
        ropeActorForces = new RopeActorForces(
            rope,
            pointMass
            );
        ropeActorForces.EnableActor();
    }

    protected override void OnValidate()
    {
        pointMass = Mathf.Max(0, pointMass);
    }

    public float PointMass { get => ropeActorForces.PointMass; set => ropeActorForces.PointMass = value; }

    public void ApplyForce(Vector3 forceVector, ForceMode forceMode, float timeStep) => 
        ropeActorForces.ApplyForce(forceVector, forceMode, timeStep);

    public void ApplyForceInterpolated(
        int fromIndex, 
        int toIndex,
        Vector3 fromVector,
        Vector3 toVector, 
        ForceMode forceMode, 
        float timeStep
        )
    {
        fromIndex = Mathf.Clamp(fromIndex, 0, ropeActorForces.Rope.RopeBody.Count - 1);
        toIndex = Mathf.Clamp(toIndex, 0, ropeActorForces.Rope.RopeBody.Count - 1);

        if (toIndex == fromIndex)
        {
            ropeActorForces.ApplyForce(Vector3.Lerp(fromVector, toVector, .5f), forceMode, timeStep);
            return;
        }

        if (fromIndex > toIndex)
        {
            int tempIndex = fromIndex;
            fromIndex = toIndex;
            toIndex = tempIndex;
            Vector3 tempVector = fromVector;
            fromVector = toVector;
            toVector = tempVector;
        }

        int indexDifference = Mathf.Abs(fromIndex - toIndex);
        int count = 0;
        for (int i = fromIndex; i < toIndex + 1; i++)
        {
            ropeActorForces.ApplyForce(
                Vector3.Lerp(fromVector, toVector, (float)count / indexDifference), 
                forceMode, 
                timeStep,
                ropeActorForces.Rope.RopeBody[i]
                );

            count++;
        }
    }
}
