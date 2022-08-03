using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RopeContact
{
    private int pointIndex;
    public int PointIndex { get => pointIndex; }

    protected Vector3 position;
    public Vector3 Position { get => position; }

    private Rigidbody rigidbody;
    public Rigidbody Rigidbody { get => rigidbody; }

    //low contact is closwer to the rope tip, high contact is closer to the player along the rope
    private RopeContact lowContact;
    public RopeContact LowContact { get => lowContact; }

    private RopeContact highContact;
    public RopeContact HighContact { get => highContact; }

    public abstract bool Replacable { get; }

    public RopeContact(int pointIndex, Vector3 position, Rigidbody rigidbody = null)
    {
        this.pointIndex = pointIndex;
        this.position = position;
        this.rigidbody = rigidbody;
    }

    public void InsertAboveContact(RopeContact lowContact)
    {
        if (lowContact != null)
        {
            if (lowContact.highContact != null)
            {
                lowContact.highContact.lowContact = this;
                this.highContact = lowContact.highContact;
            }
            lowContact.highContact = this;
            this.lowContact = lowContact;
        }
    }

    public void InsertBelowContact(RopeContact highContact)
    {
        if (highContact != null)
        {
            if (highContact.lowContact != null)
            {
                highContact.lowContact.highContact = this;
                this.lowContact = highContact.lowContact;
            }
            highContact.lowContact = this;
            this.highContact = highContact;
        }
    }

    public void ReplaceContact(RopeContact oldContact)
    {
        if (!oldContact.Replacable) return;
        if (oldContact.lowContact != null)
        {
            this.lowContact = oldContact.lowContact;
            this.lowContact.highContact = this;
        }
        if (oldContact.highContact != null)
        {
            this.highContact = oldContact.highContact;
            this.highContact.lowContact = this;
        }
    }

    public abstract void LimitVelocity(float strength, float vectorLerp);

    public abstract void ApplyForce(float strength, float vectorLerp, float timeStep);
}

public class RopeContactCollision : RopeContact
{
    private Vector3 lowCollisionVector;
    public Vector3 LowCollisionVector
    {
        get => lowCollisionVector;

    }
    private Vector3 highCollisionVector;
    public Vector3 HighCollisionVector { get => highCollisionVector; }

    public override bool Replacable => true;

    public RopeContactCollision(int pointIndex, Vector3 position, Vector3 lowCollisionVector, Vector3 highCollisionVector, Rigidbody rb = null) : base(pointIndex, position, rb)
    {
        this.lowCollisionVector = lowCollisionVector;
        this.highCollisionVector = highCollisionVector;
    }

    public override void LimitVelocity(float strength, float vectorLerp)
    {
        if (Rigidbody == null) return;

        // Dont need anything here
    }
    public override void ApplyForce(float strength, float vectorLerp, float timeStep)
    {
        if (Rigidbody == null) return;

        //Rigidbody.AddForceAtPosition(CollisionVector * strength * timeStep, Position, ForceMode.Impulse);
        Rigidbody.AddForceAtPosition(Vector3.Lerp(LowCollisionVector, HighCollisionVector, vectorLerp).normalized * strength * timeStep, Position, ForceMode.Impulse);
    }
}

public class RopeContactAttachment : RopeContact
{
    /*
    private Vector3 attachmentVector = Vector3.zero;
    public Vector3 AttachmentVector
    {
        get
        {
            // calculate attachment vector if it doesnt exist
            if (attachmentVector == Vector3.zero)
            {
                if (LowContact == null)
                {
                    if (HighContact == null)
                    {
                        attachmentVector = Vector3.zero;
                    }
                    attachmentVector = (HighContact.Position - Position).normalized;
                    return attachmentVector;
                }
                if (HighContact == null)
                {
                    if (LowContact == null)
                    {
                        attachmentVector = Vector3.zero;
                    }
                    attachmentVector = (LowContact.Position - Position).normalized;
                    return attachmentVector;
                }

                attachmentVector = Vector3.Slerp((LowContact.Position - Position).normalized, (HighContact.Position - Position).normalized, .5f);
                return attachmentVector;
            }
            return attachmentVector;
        }
    }*/

    private Vector3 lowAttachmentVector = Vector3.zero;
    public Vector3 LowAttachmentVector
    {
        get
        {
            if (lowAttachmentVector == Vector3.zero)
            {
                if (LowContact == null) lowAttachmentVector = Vector3.zero;
                else lowAttachmentVector = (LowContact.Position - Position).normalized;
            }
            return lowAttachmentVector;
        }
    }

    private Vector3 highAttachmentVector = Vector3.zero;
    public Vector3 HighAttachmentVector
    {
        get
        {
            if (highAttachmentVector == Vector3.zero)
            {
                if (HighContact == null) highAttachmentVector = Vector3.zero;
                else highAttachmentVector = (HighContact.Position - Position).normalized;
            }
            return highAttachmentVector;
        }
    }

    public override bool Replacable => false;

    public RopeContactAttachment(int pointIndex, Vector3 position, Rigidbody rb = null) : base(pointIndex, position, rb)
    {
    }

    public override void LimitVelocity(float strength, float vectorLerp)
    {
        if (Rigidbody == null) return;
        Vector3 attachmentVector = Vector3.Lerp(LowAttachmentVector, HighAttachmentVector, vectorLerp);
        //Vector3 dampenedVelocity = Vector3.Lerp(contact.rb.velocity, Vector3.ProjectOnPlane(contact.rb.velocity, contact.contactVector), tensionDamping);
        Vector3 velocity = Rigidbody.velocity;
        float velcoityMagnitude = velocity.magnitude;
        Vector3 tempAV = attachmentVector;
        Vector3.OrthoNormalize(ref tempAV, ref velocity);
        Vector3 tangentVelocity = velocity * velcoityMagnitude;
        //Debug.DrawRay(contact.rb.position, tangentVelocity, Color.blue);
        //float t = 1 - Mathf.Abs(Vector3.Dot(contact.contactVector, contact.rb.velocity.normalized));
        float t = -Vector3.Dot(attachmentVector, Rigidbody.velocity.normalized);
        t = t < 0 ? 0 : 1 - t;
        tangentVelocity = Vector3.Lerp(Vector3.ProjectOnPlane(Rigidbody.velocity, attachmentVector), tangentVelocity, t);
        //Debug.DrawRay(contact.rb.position, tangentVelocity * 3, Color.black);

        Vector3 dampenedVelocity = Vector3.Lerp(Rigidbody.velocity, tangentVelocity, strength); // strength here was initially called tension damping
        //Debug.DrawRay(Rigidbody.position, dampenedVelocity.normalized * 2, Color.blue);
        //Debug.DrawRay(contact.rb.position, dampenedVelocity, Color.green);
        //Debug.Log("limit: " + Rigidbody.position + " force: " + dampenedVelocity);

        Rigidbody.velocity = dampenedVelocity;
    }

    public override void ApplyForce(float strength, float vectorLerp, float timeStep)
    {

        if (Rigidbody == null) return;
        //Debug.DrawRay(Rigidbody.transform.position, AttachmentVector.normalized * 2, Color.green);
        if (LowAttachmentVector == Vector3.zero && HighAttachmentVector == Vector3.zero) return;
        //Rigidbody.AddForceAtPosition(AttachmentVector * strength * timeStep, Position, ForceMode.Impulse);

        Rigidbody.AddForceAtPosition(Vector3.Lerp(LowAttachmentVector, HighAttachmentVector, vectorLerp).normalized * strength * timeStep, Position, ForceMode.Impulse);
    }
}

public class RopeContactTrigger : RopeContact
{
    public RopeContactTrigger(int pointIndex, Vector3 position, Rigidbody rigidbody = null) : base(pointIndex, position, rigidbody)
    {
    }

    public override void ApplyForce(float strength, float vectorLerp, float timeStep)
    {
    }

    public override void LimitVelocity(float strength, float vectorLerp)
    {
    }
    public override bool Replacable { get { return true; } }

}

public class RopeContactChain : IEnumerable<RopeContact>
{
    private class InsertReturn
    {
        private int index;
        /// <summary>
        /// The index the contact was inserted at
        /// </summary>
        public int Index { get => index; }

        private bool replacedContact;
        /// <summary>
        /// If the contact replaced a different one
        /// </summary>
        public bool ReplacedContact { get => replacedContact; }

        public InsertReturn(int index, bool replacedContact)
        {
            this.index = index;
            this.replacedContact = replacedContact;
        }
    }

    private RopeContact startContact;
    private int count = 0;
    public RopeContactChain() { }

    public RopeContactChain(RopeContact baseContact)
    {
        Insert(baseContact);
    }

    public void Clear()
    {
        startContact = null;
        count = 0;
    }

    public void Insert(RopeContact newContact)
    {
        InsertUpdate(newContact, SearchInsert(newContact, startContact, 0));
    }

    /// <summary>
    /// Inserts the contact into the chain, searching up from the start. Returns the inserted index and if something was overwritten
    /// </summary>
    /// <param name="newContact"></param>
    /// <param name="searchContact"></param>
    /// <param name="index">when first called, the starting index of the search</param>
    /// <returns></returns>
    private InsertReturn SearchInsert(RopeContact newContact, RopeContact searchContact, int index)
    {
        if (searchContact == null)
        {
            return new InsertReturn(index, false);
        }
        if (newContact.PointIndex < searchContact.PointIndex)
        {
            if (searchContact.LowContact == null)
            {
                newContact.InsertBelowContact(searchContact);
                return new InsertReturn(0, false);
            }
            else
            {
                if (newContact.PointIndex > searchContact.LowContact.PointIndex)
                {
                    newContact.InsertBelowContact(searchContact);
                    return new InsertReturn(index, false);
                }
                else
                {
                    return SearchInsert(newContact, searchContact.LowContact, index - 1);
                }
            }
        }
        else if (newContact.PointIndex == searchContact.PointIndex)
        {
            if (searchContact.Replacable)
            {
                newContact.ReplaceContact(searchContact);
                return new InsertReturn(index, true);
            }
            else
            {
                return new InsertReturn(-1, true);
            }
        }
        else
        {
            if (searchContact.HighContact == null)
            {
                newContact.InsertAboveContact(searchContact);
                return new InsertReturn(index + 1, false);
            }
            else
            {
                if (newContact.PointIndex < searchContact.HighContact.PointIndex)
                {
                    newContact.InsertAboveContact(searchContact);
                    return new InsertReturn(index + 1, false);
                }
                else
                {
                    return SearchInsert(newContact, searchContact.HighContact, index + 1);
                }
            }
        }
    }

    private void InsertUpdate(RopeContact newContact, InsertReturn ir)
    {
        // don't do anything if it was less than 0 (-1) because nothing was added
        if (ir.Index < 0) return;
        if (!ir.ReplacedContact) count++;
        if (ir.Index == 0) startContact = newContact;
    }

    public RopeContact FindNearest(int searchIndex, RopeContact searchContact)
    {
        if (searchContact == null)
        {
            return null;
        }
        int lowDist = searchContact.LowContact == null ? int.MaxValue : Mathf.Abs(searchContact.LowContact.PointIndex - searchIndex);
        int highDist = searchContact.HighContact == null ? int.MaxValue : Mathf.Abs(searchContact.HighContact.PointIndex - searchIndex);
        int centerDist = Mathf.Abs(searchContact.PointIndex - searchIndex);
        if (lowDist < highDist)
        {
            if (lowDist < centerDist)
            {
                return FindNearest(searchIndex, searchContact.LowContact);
            }
            else
            {
                return searchContact;
            }
        }
        else
        {
            if (highDist < centerDist)
            {
                return FindNearest(searchIndex, searchContact.HighContact);
            }
            else
            {
                return searchContact;
            }
        }
    }

    public (RopeContact lowContactPoint, RopeContact highContactPoint) FindNearestPair(int searchIndex)
    {
        RopeContact nearest = FindNearest(searchIndex, startContact);
        RopeContact pair;
        if (nearest.LowContact == null)
        {
            pair = nearest.HighContact == null ? nearest : nearest.HighContact;
            return (nearest, pair);
        }
        if (nearest.HighContact == null)
        {
            pair = nearest.LowContact == null ? nearest : nearest.LowContact;
            return (pair, nearest);
        }

        if (searchIndex > nearest.PointIndex)
        {
            pair = nearest.HighContact;
            return (nearest, pair);
        }
        else
        {
            pair = nearest.LowContact;
            return (pair, nearest);
        }

    }

    public IEnumerator<RopeContact> GetEnumerator()
    {
        RopeContact contact = startContact;
        while (contact != null)
        {
            yield return contact;
            contact = contact.HighContact;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}