using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RopeActorTrigger : RopeActorBase
{
    private RopeSimulatorTrigger ropeSimulatorTrigger;
    public RopeSimulatorTrigger RopeSimulatorTrigger { get { return ropeSimulatorTrigger; } }

    List<RopeContactTrigger> oldContacts;
    List<RopeContactTrigger> newContacts;

    public RopeActorTrigger(
        Rope rope,
        bool triggerEnabled
        ) : base(rope)
    {
        this.ropeSimulatorTrigger = new RopeSimulatorTrigger(
            CalculateLayermask(),
            triggerEnabled
            );

        actionExecutions.Add((ExecutionOrder, DetectTrigger));
    }

    public bool TriggerEnabled { set => ropeSimulatorTrigger.TriggerEnabled = value; }

    public override int ExecutionOrder { get { return 2; } }

    public void UpdateLayermask()
    {
        ropeSimulatorTrigger.LayerMask = CalculateLayermask();
    }

    private LayerMask CalculateLayermask()
    {
        return RopeCollisionMatrixLayerMask.MaskForLayer(Rope.ParentGameObject.layer);
    }

    /// <summary>
    /// call this every fixed update
    /// </summary>
    private void DetectTrigger()
    {
        oldContacts = newContacts.ToList();
        newContacts = ropeSimulatorTrigger.GatherContacts(Rope.Sticks);
        SendCollisionEvents();
    }

    private void SendCollisionEvents()
    {
        foreach (RopeContactTrigger c in newContacts)
        {
            if (c.Rigidbody == null) continue;
            if (oldContacts.Find(x => x.Rigidbody == c.Rigidbody) == null)
            {
                SendTriggerEnter(c);
            }
            else
            {
                SendTriggerStay(c);
            }
        }
        foreach (RopeContactTrigger c in oldContacts)
        {
            if (c.Rigidbody == null) continue;
            if (newContacts.Find(x => x.Rigidbody == c.Rigidbody) == null)
            {
                SendTriggerExit(c);
            }
        }

    }
    private void SendTriggerEnter(RopeContactTrigger contact)
    {
        RopeTriggerEvent triggerEvent = new RopeTriggerEvent(Rope, contact);
        if (triggerEvent.Contact.Rigidbody != null)
        {
            triggerEvent.Contact.Rigidbody.gameObject.SendMessage("OnRopeTriggerEnter", triggerEvent, SendMessageOptions.DontRequireReceiver);
            Rope.ParentGameObject.SendMessage("OnRopeTriggerEnter", triggerEvent, SendMessageOptions.DontRequireReceiver);
        }
    }
    private void SendTriggerStay(RopeContactTrigger contact)
    {
        RopeTriggerEvent triggerEvent = new RopeTriggerEvent(Rope, contact);
        if (triggerEvent.Contact.Rigidbody != null)
        {
            triggerEvent.Contact.Rigidbody.gameObject.SendMessage("OnRopeTriggerStay", triggerEvent, SendMessageOptions.DontRequireReceiver);
            Rope.ParentGameObject.SendMessage("OnRopeTriggerStay", triggerEvent, SendMessageOptions.DontRequireReceiver);
        }
    }
    private void SendTriggerExit(RopeContactTrigger contact)
    {
        RopeTriggerEvent triggerEvent = new RopeTriggerEvent(Rope, contact);
        if (triggerEvent.Contact.Rigidbody != null)
        {
            triggerEvent.Contact.Rigidbody.gameObject.SendMessage("OnRopeTriggerExit", triggerEvent, SendMessageOptions.DontRequireReceiver);
            Rope.ParentGameObject.SendMessage("OnRopeTriggerExit", triggerEvent, SendMessageOptions.DontRequireReceiver);
        }
    }
}