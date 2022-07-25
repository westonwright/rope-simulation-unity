using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeCollisionEvent
{
    private Rope rope;
    public Rope Rope { get { return rope; } }

    private RopeContactCollision contact;
    public RopeContactCollision Contact { get { return contact; } }

    public RopeCollisionEvent(Rope rope, RopeContactCollision contact)
    {
        this.rope = rope;
        this.contact = contact;
    }
}

public class RopeTriggerEvent
{
    private Rope rope;
    public Rope Rope { get { return rope; } }

    RopeContactTrigger contact;
    public RopeContactTrigger Contact { get { return contact; } }

    public RopeTriggerEvent(Rope rope, RopeContactTrigger contact)
    {
        this.rope = rope;
        this.contact = contact;
    }
}
