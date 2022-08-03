using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RopeSimulatorRigidbody
{
    private readonly float MIN_COLLISION_ROTATION = 1f;
    private RopeSimulatorMotion ropeSimulatorMotion;
    public RopeSimulatorMotion RopeSimulatorMotion { get { return ropeSimulatorMotion; } }

    private LayerMask layerMask;
    public LayerMask LayerMask { set => layerMask = value; }

    private int collisionIterations = 2;
    public int CollisionIterations { get => collisionIterations; set => collisionIterations = Mathf.Max(value, 1); }

    private RopeContactChain contacts = new RopeContactChain();
    public List<RopeContact> Contacts { get { return contacts.ToList(); } }   

    //private RaycastHit hit, inverseHit;

    public RopeSimulatorRigidbody(
        RopeSimulatorMotion ropeSimulatorMotion,
        LayerMask layerMask,
        int collisionIterations = 2
        )
    {
        this.ropeSimulatorMotion = ropeSimulatorMotion;
        this.ropeSimulatorMotion.SetStickPosition1 = SetStickPosition1;
        this.ropeSimulatorMotion.SetStickPosition2 = SetStickPosition2;

        this.layerMask = layerMask;
        this.collisionIterations = collisionIterations;
    }

    /// <summary>
    /// Sets up contacts for a new time step
    /// </summary>
    public void RefreshContacts(IEnumerable<RopeAttachment> attachments, int pointCount)
    {
        // contacts are recalculated each update. attachments must be manually added each frame
        contacts.Clear();

        foreach(RopeAttachment attachment in attachments)
        {
            contacts.Insert(new RopeContactAttachment(attachment.CorrectedPointIndex(pointCount), attachment.Position, attachment.Rigidbody));
        }
    }
    
    private void SetStickPosition1(ref RopeStick s, ref Vector3 stickCenter, ref Vector3 stickDirection, bool order, int stickIndex = 0)
    {
        RopePoint p1 = order ? s.PointA : s.PointB;
        RopePoint p2 = order ? s.PointB : s.PointA;
        StickCollision(stickIndex, order, ref stickCenter, ref p2, ref p1, ref stickDirection, out RopeContact contact);
        if (contact != null)
        {
            contacts.Insert(contact);
        }

        p1.Position = Vector3.Lerp(stickCenter + stickDirection * s.Length / 2, p1.Position, p1.Friction);
    }
    
    private void SetStickPosition2(ref RopeStick s, ref Vector3 stickCenter, ref Vector3 stickDirection, bool order)
    {
        RopePoint p = order ? s.PointB : s.PointA;
        p.Position = Vector3.Lerp(stickCenter - stickDirection * s.Length / 2, p.Position, p.Friction);
    }

    /// <summary>
    /// Calculates collision with a stick
    /// </summary>
    private void StickCollision<T>(int pointIndex, bool order, ref Vector3 stickCenter, ref T startPoint, ref T endPoint, ref Vector3 stickDir, out RopeContact contact)
        where T : IRopePointPosition, IRopePointPrevPosition, IRopePointFriction
    {
        float stickLength = Vector3.Distance(startPoint.Position, endPoint.Position);
        contact = null;
        int collisionsCount = 0;
        while (true)
        {
            Ray ray = new Ray(startPoint.Position, stickDir);
            if (Physics.Raycast(ray, out RaycastHit hit, stickLength, layerMask))
            {
                //special case if opposite ray hits a wall too, calculate minimum rotation to resolve the collision
                //could calculate this with binary search just rotating the vector until found
                //or could do some trigonometry but potentially be a little less accurate in some cases.
                Ray inverseRay = new Ray(endPoint.Position, -stickDir);
                if (Physics.Raycast(inverseRay, out RaycastHit inverseHit, stickLength, layerMask))
                {
                    endPoint.Friction = 1 - (hit.distance / stickLength);

                    stickDir = BinarySearchCollision(startPoint.Position, stickDir, stickLength, TriangleCollisionVector(hit, stickDir, stickLength));
                    //contact point can only update around corners
                    contact = new RopeContactCollision
                    (
                        pointIndex,
                        (hit.point + inverseHit.point) / 2,
                        -(order ? hit.normal : inverseHit.normal),
                        -(order ? inverseHit.normal : hit.normal),
                        hit.collider.attachedRigidbody
                    );
                }
                else
                {
                    stickDir = TriangleCollisionVector(hit, stickDir, stickLength);
                }
            }
            else
            {
                if (collisionsCount > 0)
                {
                    //Debug.DrawRay(ray.origin, ray.direction * stickLength, Color.red);
                    endPoint.Position = startPoint.Position + stickDir * stickLength;
                    //Debug.DrawRay(endPoint.position, Vector3.up * .1f, Color.yellow);
                    //Debug.DrawRay(endPoint.prevPosition, Vector3.up * .1f, Color.green);
                    endPoint.PrevPosition = endPoint.Position;
                    Vector3 vectorToCenter = (stickDir * Vector3.Distance(startPoint.Position, stickCenter));
                    stickCenter = startPoint.Position + vectorToCenter;
                }
                else
                {
                    endPoint.Friction = 0;
                }
                break;
            }
            //need to come up with a better solution for what to do in this case
            if (collisionsCount >= collisionIterations)
            {
                endPoint.Position = startPoint.Position + stickDir * stickLength;
                endPoint.PrevPosition = endPoint.Position;
                Vector3 vectorToCenter = (stickDir * Vector3.Distance(startPoint.Position, stickCenter));
                stickCenter = startPoint.Position + vectorToCenter;
                break;
            }

            collisionsCount++;
        }
    }

    private Vector3 TriangleCollisionVector(RaycastHit hit, Vector3 stickDir, float stickLength)
    {
        //Debug.DrawRay(hit.point, hit.normal * .2f, Color.green);
        Vector3 fTan = Vector3.Cross(hit.normal, stickDir);
        Vector3 rTan = Vector3.Cross(hit.normal, fTan);
        //Debug.DrawRay(hit.point, fTan * .2f, Color.magenta);
        //Debug.DrawRay(hit.point, rTan * .2f, Color.cyan);
        //angle from plane of initial triangle
        float sA = Mathf.Abs(Vector3.SignedAngle(rTan, -stickDir, fTan));
        //angle to plane of initial triangle
        float sB = 90 - sA;
        //height from plane for both triangles
        float a = Mathf.Sin(Mathf.Deg2Rad * sA) * hit.distance;
        //a^2 + b^2 = c^2
        //c^2 - a^2 = b^2
        //length along plane of final triangle
        //angle to plane of final triangle
        float fB = Mathf.Rad2Deg * Mathf.Acos(a / stickLength);

        float rotAngle = (fB - sB) + MIN_COLLISION_ROTATION;//add one just to make sure its rotated enough to be outside the collider
        return Quaternion.AngleAxis(rotAngle, -fTan) * stickDir;
    }

    private Vector3 TriangleSurfaceTangent(RaycastHit hit, Vector3 stickDir)
    {
        Vector3 fTan = Vector3.Cross(hit.normal, stickDir);
        return Vector3.Cross(hit.normal, fTan).normalized;
    }

    private Vector3 BinarySearchCollision(Vector3 stickStart, Vector3 stickDir, float stickLength, Vector3 maxStickDir)
    {
        Vector3 bestDir = maxStickDir;
        //with 5 iterations we can get near 1 degree of accuracy
        int iterations = 6;
        float max = 1;
        float min = 0;
        for (int i = 0; i < iterations; i++)
        {
            float percent = min + ((max - min) / 2);
            Vector3 currentDir = Vector3.Slerp(stickDir, maxStickDir, percent);
            Ray searchRay = new Ray(stickStart, currentDir);
            //recalculate if the ray made a collision
            if (Physics.Raycast(searchRay, stickLength, layerMask))
            {
                min = percent;
            }
            else
            {
                max = percent;
                bestDir = currentDir;
            }
        }
        return bestDir;
    }

    private Vector3 BinarySearchTriangle(Vector3 stickStart, Vector3 stickDir, float stickLength, Vector3 stickDown, Vector3 rightMaxStickDir, Vector3 leftMaxStickDir)
    {
        int iterations = 6;

        Vector3 rightBestDir = rightMaxStickDir;
        float rMax = 1;
        float rMin = 0;
        for (int i = 0; i < iterations; i++)
        {
            float percent = rMin + ((rMax - rMin) / 2);
            Vector3 currentDir = Vector3.Slerp(stickDir, rightMaxStickDir, percent);
            Ray searchRay = new Ray(stickStart, currentDir);
            //recalculate if the ray made a collision
            if (Physics.Raycast(searchRay, stickLength, layerMask))
            {
                rMin = percent;
            }
            else
            {
                rMax = percent;
                rightBestDir = currentDir;
            }
        }

        Vector3 leftBestDir = leftMaxStickDir;
        float lMax = 1;
        float lMin = 0;
        for (int i = 0; i < iterations; i++)
        {
            float percent = lMin + ((lMax - lMin) / 2);
            Vector3 currentDir = Vector3.Slerp(stickDir, leftMaxStickDir, percent);
            Ray searchRay = new Ray(stickStart, currentDir);
            //recalculate if the ray made a collision
            if (Physics.Raycast(searchRay, stickLength, layerMask))
            {
                lMin = percent;
            }
            else
            {
                lMax = percent;
                leftBestDir = currentDir;
            }
        }

        float rBestAngle = Vector3.Angle(stickDown, rightBestDir);
        float lBestAngle = Vector3.Angle(stickDown, leftBestDir);
        Vector3 bestDir = lBestAngle < rBestAngle ? leftBestDir : rightBestDir;

        return bestDir;
    }
}
