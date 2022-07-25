using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

// TODO: with only one collision, find a better method for determining which direciton to rotate
// TODO: with two collisions on a sharp surface, determin which direciton to rotate in

public class Rope
{
    protected readonly Vector3 DEFAULT_DIRECTION = Vector3.down;
    protected readonly Vector3 DEFAULT_POSITION = Vector3.zero;

    protected List<RopePoint> points = new List<RopePoint>();
    public List<RopePoint> Points { get { return points; } }

    protected List<RopeStick> sticks = new List<RopeStick>();
    public List<RopeStick> Sticks { get { return sticks; } }

    protected List<RopeAttachment> attachments = new List<RopeAttachment>();
    public List<RopeAttachment> Attachments { get { return attachments; } }

    private GameObject parentGameObject;
    public GameObject ParentGameObject { get { return parentGameObject; } }

    protected float currentTimeStep;
    public float CurrentTimeStep { get { return currentTimeStep; } }

    private List<RopeActorBase> ropeComponents = new List<RopeActorBase>();
    private List<(int, Action)> actionExectuions = new List<(int, Action)>();
    public T GetRopeComponent<T>() where T : RopeActorBase
    {
        return (T)ropeComponents.First(x => x.GetType() == typeof(T));
    }

    public Rope(
        GameObject parentGameObject,
        float totalLength,
        float segmentLength
        )
    {
        this.parentGameObject = parentGameObject;

        BuildRope(Mathf.FloorToInt(totalLength / segmentLength), segmentLength);
    }

    public Rope(
        GameObject parentGameObject,
        RopeAttachment[] attachments,
        float totalLength,
        float segmentLength
        )
    {
        this.parentGameObject = parentGameObject;
        foreach (RopeAttachment a in attachments)
        {
            AddAttachment(a);
        }

        BuildRope(Mathf.FloorToInt(totalLength / segmentLength), segmentLength);
    }
    public void AddRopeComponent(RopeActorBase newComponent)
    {
        if (!ropeComponents.Contains(newComponent))
        {
            ropeComponents.Add(newComponent);
            GetActionExecutions();
        }
    }
    public void RemoveRopeComponent(RopeActorBase oldComponent)
    {
        ropeComponents.Remove(oldComponent);
        GetActionExecutions();
    }

    private void GetActionExecutions()
    {
        actionExectuions.Clear();
        foreach(RopeActorBase component in ropeComponents)
        {
            foreach((int, Action) action in component.ActionExecutions)
            {
                actionExectuions.Add(action);
            }
        }
        actionExectuions.Sort((x, y) => x.Item1.CompareTo(y.Item1));
    }

    public void UpdateRope(float timeStep)
    {
        currentTimeStep = timeStep;
        MovePointsToAttachments();
        // move this into action executions too?

        // execute the actions of each component in order
        foreach((int, Action) action in actionExectuions)
        {
            action.Item2();
        }
    }

    protected virtual void BuildRope(int segments, float segmentLength)
    {
        int attachmentIndex = 0;
        Vector3 previousPosition = DEFAULT_POSITION;
        Vector3 nextPosition = attachments.Count > 0 ? attachments[0].Position : DEFAULT_POSITION;
        int currentPoint = 0;
        int pointsInSection = 1;
        int previousPointIndex = 0;
        int nextPointIndex = attachments.Count > 0 ? attachments[0].CorrectedPointIndex(segments + 1) : 0;
        for (int i = 0; i < segments + 1; i++)
        {
            //Debug.Log("i: " + i);
            //Debug.Log("raw index: " + attachments[Mathf.Min(attachmentIndex, attachments.Count - 1)].RawPointIndex);
            //Debug.Log("corrected index: " + attachments[Mathf.Min(attachmentIndex, attachments.Count - 1)].CorrectedPointIndex(points.Count));
            if (attachmentIndex + 1 > attachments.Count)
            {
                points.Add(new RopePoint(previousPosition + new Vector3(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)).normalized));
                //Debug.Log("Add 1: " + previousPosition);
                continue;
            }
            if (nextPointIndex <= i)
            {
                previousPosition = nextPosition;

                attachmentIndex++;
                if (attachmentIndex + 1 > attachments.Count)
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
        // create sticks
        for(int i = 0; i < points.Count - 1; i++)
        {
            sticks.Add(new RopeStick(points[i], points[i + 1], segmentLength));
        }

        ResolveAttachments();
    }

    private void MovePointsToAttachments()
    {
        //Debug.Log("point count: " + points.Count);
        foreach (RopePoint point in points)
        {
            point.IsLocked = false;
        }
        foreach (RopeAttachment attachment in attachments)
        {
            points[attachment.CorrectedPointIndex(points.Count)].Position = attachment.Position;
            points[attachment.CorrectedPointIndex(points.Count)].IsLocked = true;
        }
    }

    public void AddAttachmentPoint(int pointIndex, Vector3 position)
    {
        AddAttachment(new RopeAttachmentPoint(pointIndex, position));
        ResolveAttachments();
    }

    public void AddAttachmentTransform(int pointIndex, Transform transform, Vector3 offset)
    {
        AddAttachment(new RopeAttachmentTransform(pointIndex, transform, offset));
        ResolveAttachments();
    }

    public void AddAttachmentRigidbody(int pointIndex, Transform transform, Vector3 offset, Rigidbody rigidbody)
    {
        AddAttachment(new RopeAttachmentRigidbody(pointIndex, transform, offset, rigidbody));
        ResolveAttachments();
    }

    private void AddAttachment(RopeAttachment attachment)
    {
        // check if something is already attached to that point and overwrite it
        // then sort the list of attachments

        //add contact
        if (attachments.Count == 0)
        {
            attachments.Add(attachment);
            return;
        }

        if (!attachments.Any(x => x.RawPointIndex == attachment.RawPointIndex))
        {
            if (attachment.RawPointIndex >= 0)
            {
                int aIndex = 0;
                float otherRaw = -1;
                while (true)
                {
                    otherRaw = attachments[aIndex].RawPointIndex;
                    if (otherRaw < 0)
                    {
                        attachments.Insert(aIndex, attachment);
                        return;
                    }
                    if (otherRaw > attachment.RawPointIndex)
                    {
                        attachments.Insert(aIndex, attachment);
                        return;
                    }
                    aIndex++;
                    if (aIndex >= attachments.Count)
                    {
                        attachments.Add(attachment);
                        return;
                    }
                }
            }
            else
            {
                int aIndex = attachments.Count - 1;
                float otherRaw = 0;
                while (true)
                {
                    otherRaw = attachments[aIndex].RawPointIndex;
                    if (otherRaw >= 0)
                    {
                        attachments.Insert(aIndex + 1, attachment);
                        return;
                    }
                    if (otherRaw < attachment.RawPointIndex)
                    {
                        attachments.Insert(aIndex + 1, attachment);
                        return;
                    }
                    aIndex--;
                    if (aIndex < 0)
                    {
                        attachments.Insert(0, attachment);
                        return;
                    }
                }
            }
        }
        //replace contact
        else
        {
            attachments[attachments.FindIndex(x => x.RawPointIndex == attachment.RawPointIndex)] = attachment;
        }
    }

    /// <summary>
    /// Removes any attachments that are connected to the same point index
    /// </summary>
    protected void ResolveAttachments()
    {
        // index then count
        Dictionary<int, int> repeatedIndexes = new Dictionary<int, int>();
        foreach (RopeAttachment a in attachments)
        {
            int correctedIndex = a.CorrectedPointIndex(points.Count);
            if (repeatedIndexes.ContainsKey(correctedIndex))
            {
                repeatedIndexes[correctedIndex]++;
            }
            else
            {
                repeatedIndexes[correctedIndex] = 1;
            }
        }

        foreach (KeyValuePair<int, int> indexCount in repeatedIndexes)
        {
            int count = indexCount.Value;
            while (count > 1)
            {
                int removeIndex = attachments.FindLastIndex(x => x.CorrectedPointIndex(points.Count) == indexCount.Key);
                if (removeIndex < 0) break;
                attachments.RemoveAt(removeIndex);
                count--;
            }
        }
    }

    /// <summary>
    /// creates a new point for you from the provided position
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pointPosition"></param>
    /// <param name="segmentLength"></param>
    public void InsertPoint(int index, Vector3 pointPosition, float segmentLength)
    {
        InsertPoint(index, new RopePoint(pointPosition), segmentLength);
    }

    public void InsertPoint(int index, RopePoint point, float segmentLength)
    {
        if ((points.Count == 0) || (index < 0) || (index > points.Count))
        {
            ResolveAttachments(); 
            return;
        }
        if (index == -1) index = points.Count;

        if (index == 0)
        {
            points.Insert(index, point);
            sticks.Insert(0, new RopeStick(points[0], points[1], segmentLength));
        }
        else if(index == points.Count)
        {
            points.Add(point);
            sticks.Add(new RopeStick(points[index - 1], points[index], segmentLength));
        }
        else
        {
            points.Insert(index, point);
            if (sticks.Count > 0)
            {
                RopeStick oldstick = sticks[index - 1];
                sticks[index - 1] = new RopeStick(points[index], oldstick.PointB, segmentLength);
                sticks.Insert(index - 1, new RopeStick(oldstick.PointA, points[index], segmentLength));
            }
        }
        ResolveAttachments();
    }

    public void RemovePointAt(int index)
    {
        if ((points.Count == 0) || (index < 0) || (index >= points.Count))
        {
            ResolveAttachments();
            return;
        }
        if (index == -1) index = points.Count - 1;

        if (index == 0)
        {
            if (sticks.Count > 0) sticks.RemoveAt(0);
        }
        else if (index == points.Count - 1)
        {
            if(sticks.Count > 0) sticks.RemoveAt(index - 1);
        }
        else
        {
            //if (!(sticks.Count > 0 && sticks.Count > index + 1)) return; 
            if (sticks.Count > 0)
            {
                RopeStick oldStick = sticks[index - 1];
                sticks[index - 1] = new RopeStick(oldStick.PointA, points[index + 1], oldStick.Length);
                sticks.RemoveAt(index);
            }

        }
        points.RemoveAt(index);
        ResolveAttachments();
    }

    #region helper methods
    /// <summary>
    /// Returns the index provided, or counts backwards if less than 0
    /// </summary>
    /// <param name="index">the index desired</param>
    /// <param name="arrayCount">How many items are in the array</param>
    /// <returns></returns>
    private int GetArrayIndex(int index, int arrayCount)
    {
        return index < 0 ? (arrayCount + index) : index;
    }
    #endregion
}
