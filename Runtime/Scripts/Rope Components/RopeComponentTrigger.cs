using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
interface IRopeTrigger
{
    //  Enable/Disable Trigger detection?
    bool TriggerEnabled { set; }
    void UpdateLayermask();
    void DetectTrigger();
}
*/
[RequireComponent(typeof(RopeGameObject), typeof(RopeComponentLength))]
public class RopeComponentTrigger : RopeComponentBase
{
    [SerializeField]
    private bool triggerEnabled = true;

    private RopeActorTrigger ropeActorTrigger;
    public override RopeActorBase RopeActor { get { return ropeActorTrigger; } }

    protected override void Start()
    {
        requiredActorTypes.Add(typeof(RopeActorLength));
        base.Start();
    }

    public override void Initialize(Rope rope, IEnumerable<RopeActorBase> requiredActors)
    {
        RopeActorLength ropeActorLength = requiredActors.FirstOrDefault(x => x.GetType() == typeof(RopeActorLength)) as RopeActorLength;

        ropeActorTrigger = new RopeActorTrigger(
            rope,
            ropeActorLength,
            triggerEnabled
            );
        ropeActorTrigger.EnableActor();
    }

    public bool TriggerEnabled { set => ropeActorTrigger.TriggerEnabled = value; }
}

