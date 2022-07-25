using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeActorRigidbody : RopeActorBase
{
    private RopeSimulatorRigidbody ropeSimulatorRigidbody;
    public RopeSimulatorRigidbody RopeSimulatorRigidbody { get { return ropeSimulatorRigidbody; } }

    private RopeActorMotion ropeActorMotion;
    public RopeActorMotion RopeActorMotion { get { return ropeActorMotion; } }

    private List<RopeContact> oldContacts = new List<RopeContact>();

    public RopeActorRigidbody(
        Rope rope,
        RopeActorMotion ropeActorMotion,
        bool collisionEnabled,
        int collisionIterations
        ) : base(rope)
    {
        this.ropeActorMotion = ropeActorMotion;
        ropeSimulatorRigidbody = new RopeSimulatorRigidbody(
            this.ropeActorMotion.RopeSimulatorMotion,
            CalculateLayermask(),
            collisionEnabled,
            collisionIterations
            );

        actionExecutions.Add((ExecutionOrder + 1, RefreshContacts));
        actionExecutions.Add((ExecutionOrder - 1, SendCollisionEvents));
    }

    public override int ExecutionOrder { get { return RopeActorMotion.ExecutionOrder; } }

    public bool CollisionEnabled { get => ropeSimulatorRigidbody.CollisionEnabled; set => ropeSimulatorRigidbody.CollisionEnabled = value; }
    public int CollisionIterations { get => ropeSimulatorRigidbody.CollisionIterations; set => ropeSimulatorRigidbody.CollisionIterations = value; }


    // before motion
    private void RefreshContacts()
    {
        oldContacts = ropeSimulatorRigidbody.Contacts;
        ropeSimulatorRigidbody.RefreshContacts(Rope.Attachments, Rope.Points.Count);
    }
    // after motion
    private void SendCollisionEvents()
    {
        List<RopeContact> newContacts = ropeSimulatorRigidbody.Contacts;
        RefineContacts(oldContacts, out List<RopeContactCollision> oldC);
        RefineContacts(newContacts, out List<RopeContactCollision> newC);
        SendCollisionEvents(oldC, newC);
        // dont need to copy new list to old list because the componentRigidbody handles that
    }

    public void UpdateLayermask()
    {
        ropeSimulatorRigidbody.LayerMask = CalculateLayermask();
    }

    private LayerMask CalculateLayermask()
    {
        return RopeCollisionMatrixLayerMask.MaskForLayer(Rope.ParentGameObject.layer);
    }

    private void RefineContacts(List<RopeContact> contacts, out List<RopeContactCollision> contactCollisions)
    {
        contactCollisions = new List<RopeContactCollision>();
        foreach (RopeContact contact in contacts)
        {
            if (contact.GetType() == typeof(RopeContactCollision))
            {
                contactCollisions.Add((contact as RopeContactCollision));
            }
        }
    }
    private void SendCollisionEvents(List<RopeContactCollision> oldContacts, List<RopeContactCollision> newContacts)
    {
        foreach (RopeContactCollision c in newContacts)
        {
            if (c.Rigidbody == null) continue;
            if (oldContacts.Find(x => x.Rigidbody == c.Rigidbody) == null)
            {
                SendCollisionEnter(c);
            }
            else
            {
                SendCollisionStay(c);
            }
        }
        foreach (RopeContactCollision c in oldContacts)
        {
            if (c.Rigidbody == null) continue;
            if (newContacts.Find(x => x.Rigidbody == c.Rigidbody) == null)
            {
                SendCollisionExit(c);
            }
        }

    }

    private void SendCollisionEnter(RopeContactCollision contact)
    {
        if (contact.Rigidbody != null)
        {
            RopeCollisionEvent rc = new RopeCollisionEvent(Rope, contact);
            contact.Rigidbody.gameObject.SendMessage("OnRopeCollisionEnter", rc, SendMessageOptions.DontRequireReceiver);
            Rope.ParentGameObject.SendMessage("OnRopeCollisionEnter", rc, SendMessageOptions.DontRequireReceiver);
        }
    }
    private void SendCollisionStay(RopeContactCollision contact)
    {
        if (contact.Rigidbody != null)
        {
            RopeCollisionEvent rc = new RopeCollisionEvent(Rope, contact);
            contact.Rigidbody.gameObject.SendMessage("OnRopeCollisionStay", rc, SendMessageOptions.DontRequireReceiver);
            Rope.ParentGameObject.SendMessage("OnRopeCollisionStay", rc, SendMessageOptions.DontRequireReceiver);
        }
    }
    private void SendCollisionExit(RopeContactCollision contact)
    {
        if (contact.Rigidbody != null)
        {
            RopeCollisionEvent rc = new RopeCollisionEvent(Rope, contact);
            contact.Rigidbody.gameObject.SendMessage("OnRopeCollisionExit", rc, SendMessageOptions.DontRequireReceiver);
            Rope.ParentGameObject.SendMessage("OnRopeCollisionExit", rc, SendMessageOptions.DontRequireReceiver);
        }
    }
}

