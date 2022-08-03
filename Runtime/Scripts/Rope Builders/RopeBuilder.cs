using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RopeBuilder
{
    //BuildRope(Mathf.FloorToInt(totalLength / segmentLength), segmentLength);

    protected readonly Vector3 DEFAULT_DIRECTION = Vector3.down;
    protected readonly Vector3 DEFAULT_POSITION = Vector3.zero;

    public RopeBuilder(){}

    public abstract List<RopePoint>BuildRope();

    /*
    RopeAttachment[] attachments = new RopeAttachment[attachmentBodies.Length];
        for(int i = 0; i<attachmentBodies.Length; i++)
        {
            if (attachmentBodies[i].GetComponent<Rigidbody>() != null)
            {
                attachments[i] = new RopeAttachmentRigidbody(
                    attachmentIndexes[i],
                    attachmentBodies[i].GetComponent<Rigidbody>().transform,
                    Vector3.zero,
                    attachmentBodies[i].GetComponent<Rigidbody>()
                    );}
}

            else
{
    attachments[i] = new RopeAttachmentTransform(
        attachmentIndexes[i],
        attachmentBodies[i],
        Vector3.up
        );
}
        }
    */
}

public class RopeBuilderAttachments : RopeBuilder
{
    private RopeAttachment[] attachments;
    private int segments;
    public RopeBuilderAttachments(
        RopeAttachment[] attachments,
        int segments
        )
    {
        this.attachments = attachments;
        this.segments = segments;
    }

    public override List<RopePoint>BuildRope()
    {
        List<RopePoint> points = new List<RopePoint>();

        int attachmentIndex = 0;
        Vector3 previousPosition = DEFAULT_POSITION;
        Vector3 nextPosition = attachments.Length > 0 ? attachments[0].Position : DEFAULT_POSITION;
        int currentPoint = 0;
        int pointsInSection = 1;
        int previousPointIndex = 0;
        int nextPointIndex = attachments.Length > 0 ? attachments[0].CorrectedPointIndex(segments + 1) : 0;
        for (int i = 0; i < segments + 1; i++)
        {
            //Debug.Log("i: " + i);
            //Debug.Log("raw index: " + attachments[Mathf.Min(attachmentIndex, attachments.Count - 1)].RawPointIndex);
            //Debug.Log("corrected index: " + attachments[Mathf.Min(attachmentIndex, attachments.Count - 1)].CorrectedPointIndex(points.Count));
            if (attachmentIndex + 1 > attachments.Length)
            {
                points.Add(new RopePoint(previousPosition + new Vector3(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)).normalized));
                //Debug.Log("Add 1: " + previousPosition);
                continue;
            }
            if (nextPointIndex <= i)
            {
                previousPosition = nextPosition;

                attachmentIndex++;
                if (attachmentIndex + 1 > attachments.Length)
                {
                    points.Add(new RopePoint(nextPosition));
                    //Debug.Log("Add 2: " + nextPosition);
                    continue;
                }
                previousPointIndex = nextPointIndex;
                nextPointIndex = attachments[attachmentIndex].CorrectedPointIndex(segments + 1);

                nextPosition = attachments[attachmentIndex].Position;
                points.Add(new RopePoint(previousPosition));
                //Debug.Log("Add 3: " + previousPosition);
                currentPoint = 1;

                pointsInSection = nextPointIndex - previousPointIndex;
                //Debug.Log("points in section: " + pointsInSection);
                continue;
            }
            //Debug.Log("prev: " + previousPosition + " next: " + nextPosition);
            //Debug.Log("Lerp: " + (float)currentPoint / (float)pointsInSection);
            points.Add(new RopePoint(Vector3.Lerp(previousPosition, nextPosition, (float)currentPoint / (float)pointsInSection)));
            //Debug.Log("Add 4: " + Vector3.Lerp(previousPosition, nextPosition, (float)currentPoint / (float)pointsInSection));
            currentPoint++;
        }

        return points;
    }
}


public class RopeBuilderFromTo : RopeBuilder
{
    private Vector3 positionA;
    private Vector3 positionB;
    private int points;

    public RopeBuilderFromTo(
        Vector3 positionA,
        Vector3 positionB,
        int points
        )
    {
        this.positionA = positionA;
        this.positionB = positionB;
        this.points = points;
    }

    public override List<RopePoint> BuildRope()
    {
        List<RopePoint> pointsList = new List<RopePoint>();

        if(points == 1)
        {
            pointsList.Add(new RopePoint(positionA));
            return pointsList;
        }
        for (int i = 0; i < points; i++)
        {
            pointsList.Add(new RopePoint(
                Vector3.Lerp(positionA, positionB, (float)i / points - 1)
                ));
        }
        return pointsList;
    }
}
