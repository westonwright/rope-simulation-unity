using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSimulatorTrigger
{
    private LayerMask layerMask;
    public LayerMask LayerMask { set => layerMask = value; }

    private bool triggerEnabled = true;
    public bool TriggerEnabled { set { triggerEnabled = value; } }

    public RopeSimulatorTrigger(
        LayerMask layerMask,
        bool triggerEnabled = true
        )
    {
        this.layerMask = layerMask;
        this.triggerEnabled = triggerEnabled;
    }

    public List<RopeContactTrigger> GatherContacts(IEnumerable<RopeStick> sticks)
    {
        List<RopeContactTrigger> contacts = new List<RopeContactTrigger>();
        if (!triggerEnabled) return contacts;

        int count = 0;
        foreach (RopeStick stick in sticks)
        {
            Ray ray = new Ray(stick.PointA.Position, (stick.PointB.Position - stick.PointA.Position).normalized);
            if (Physics.Raycast(ray, out RaycastHit hit, stick.Length, layerMask))
            {
                Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                if (rb == null) continue;
                contacts.Add(new RopeContactTrigger(count, hit.point, rb));
                GameObject go = hit.transform.gameObject;
            }
            count++;
        }
        return contacts;
    }
}
