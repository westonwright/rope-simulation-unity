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
[RequireComponent(typeof(RopeGameObject))]
public class RopeComponentTrigger : RopeComponentBase
{
    [SerializeField]
    private bool triggerEnabled = true;

    private RopeActorTrigger ropeActorTrigger;
    public override RopeActorBase RopeActor { get { return ropeActorTrigger; } }

    public override void Initialize(Rope rope, IEnumerable<RopeActorBase> requiredActors)
    {
        ropeActorTrigger = new RopeActorTrigger(
            rope,
            triggerEnabled
            );
    }

    public bool TriggerEnabled { set => ropeActorTrigger.TriggerEnabled = value; }
}

