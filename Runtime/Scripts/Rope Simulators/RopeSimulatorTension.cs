using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSimulatorTension
{
    private readonly float MAX_TENSION_CONTRIBUTION = 5f;

    private RopeSimulatorRigidbody ropeSimulatorRigidbody;
    public RopeSimulatorRigidbody RopeSimulatorRigidbody { get { return ropeSimulatorRigidbody; } }

    private float thresholdTension = 1.33f;
    public float ThresholdTension { get => thresholdTension; set => thresholdTension = value; }

    private float springStrength = 100f;
    public float SpringStrength { get => springStrength; set => springStrength = value; }
    
    private float dampingStrength = 1;
    public float DampingStrength { get => dampingStrength; set => dampingStrength = value; }

    public RopeSimulatorTension(
        RopeSimulatorRigidbody ropeSimulatorRigidbody,
        float thresholdTension = 1.33f,
        float springStrength = 100f,
        float dampingStrength = 1
        )
    {
        this.ropeSimulatorRigidbody = ropeSimulatorRigidbody;
        this.thresholdTension = thresholdTension;
        this.springStrength = springStrength;
        this.dampingStrength = dampingStrength;
    }

    /// <summary>
    /// Uses tension and contacts to exert forces along the rope
    /// </summary>
    /// <param name="contacts"></param>
    public void ExertForces(IList<RopeStick> sticks, float timeStep)
    {
        /*
        if (tensionAcrossRope > 0)
        {
            float tensionDamping = tensionAcrossRope * dampingStrength;
            float tensionSpring = tensionAcrossRope * springStrength;
            foreach (Contact contact in contacts)
            {
                contact.LimitVelocity(tensionDamping);
                contact.ApplyForce(tensionSpring, timeStep);
            }
        }
        */

        foreach (RopeContact contact in ropeSimulatorRigidbody.Contacts)
        {
            float lowSectionTension = 0;
            float highSectionTension = 0;
            if (contact.LowContact != null)
            {
                int count = 0;
                for (int i = contact.LowContact.PointIndex; i < contact.PointIndex; i++)
                {
                    lowSectionTension += Mathf.Clamp(
                        (Vector3.Distance(sticks[i].PointA.Position, sticks[i].PointB.Position) / sticks[i].Length),
                        0,
                        MAX_TENSION_CONTRIBUTION
                        );
                    count++;
                }
                lowSectionTension /= count;
                lowSectionTension = Mathf.Max((lowSectionTension / thresholdTension) - 1, 0);
            }

            if (contact.HighContact != null)
            {
                int count = 0;
                for (int i = contact.PointIndex; i < contact.HighContact.PointIndex; i++)
                {
                    highSectionTension += Mathf.Clamp(
                        (Vector3.Distance(sticks[i].PointA.Position, sticks[i].PointB.Position) / sticks[i].Length),
                        0,
                        MAX_TENSION_CONTRIBUTION
                        );
                    count++;
                }
                highSectionTension /= count;
                highSectionTension = Mathf.Max((highSectionTension / thresholdTension) - 1, 0);
            }

            if (lowSectionTension > 0 || highSectionTension > 0)
            {
                float tensionDamping = (lowSectionTension * dampingStrength) + (highSectionTension * dampingStrength);
                float tensionSpring = (lowSectionTension * springStrength) + (highSectionTension * springStrength);
                float vectorLerp = highSectionTension / (lowSectionTension + highSectionTension);
                contact.LimitVelocity(tensionDamping, vectorLerp);
                contact.ApplyForce(tensionSpring, vectorLerp, timeStep);
            }
        }
    }
}
