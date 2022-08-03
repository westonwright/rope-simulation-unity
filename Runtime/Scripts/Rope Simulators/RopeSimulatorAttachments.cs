using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSimulatorAttachments : IList<RopeAttachment>, IRopeBodyListner
{
    private RopeBody ropeBody;

    private List<RopeAttachment> ropeAttachments = new List<RopeAttachment>();

    public int Count => ropeAttachments.Count;

    public bool IsReadOnly => true;

    public RopeAttachment this[int index] { get => ropeAttachments[index]; set => ropeAttachments[index] = value; }

    public RopeSimulatorAttachments(RopeBody ropeBody, IEnumerable<RopeAttachment> initialAttachments)
    {
        this.ropeBody = ropeBody;
        // TODO: add automatic removal of listner when disabled
        this.ropeBody.AddListner(this);
        foreach (RopeAttachment attachment in initialAttachments)
        {
            AddAttachment(attachment);
        }
    }

    public void MovePointsToAttachments()
    {
        //Debug.Log("point count: " + points.Count);
        foreach (IRopePointIsAttached point in ropeBody)
        {
            point.IsAttached = false;
        }
        foreach (RopeAttachment attachment in ropeAttachments)
        {
            ropeBody[attachment.CorrectedPointIndex(ropeBody.Count)].Position = attachment.Position;
            ropeBody[attachment.CorrectedPointIndex(ropeBody.Count)].IsAttached = true;
        }
    }

    //TODO: Fill out these directional functions

    public void RopePointAdded(bool direction)
    {
        ValidateAttachments();
    }

    public void RopePointRemoved(bool driection)
    {
        ValidateAttachments();
    }

    // TODO: Make this direction dependent
    public void AddAttachment(RopeAttachment attachment)
    {
        // check if something is already attached to that point and overwrite it
        // then sort the list of attachments

        //add contact
        if (ropeAttachments.Count == 0)
        {
            ropeAttachments.Add(attachment);
            return;
        }

        int matchingIndex = ropeAttachments.FindIndex(x => x.RawPointIndex == attachment.RawPointIndex);
        if (matchingIndex == -1)
        {
            if (attachment.RawPointIndex >= 0)
            {
                int aIndex = 0;
                float otherRaw = -1;
                while (true)
                {
                    otherRaw = ropeAttachments[aIndex].RawPointIndex;
                    if (otherRaw < 0)
                    {
                        ropeAttachments.Insert(aIndex, attachment);
                        break;
                    }
                    if (otherRaw > attachment.RawPointIndex)
                    {
                        ropeAttachments.Insert(aIndex, attachment);
                        break;
                    }
                    aIndex++;
                    if (aIndex >= ropeAttachments.Count)
                    {
                        ropeAttachments.Add(attachment);
                        break;
                    }
                }
            }
            else
            {
                int aIndex = ropeAttachments.Count - 1;
                float otherRaw = 0;
                while (true)
                {
                    otherRaw = ropeAttachments[aIndex].RawPointIndex;
                    if (otherRaw >= 0)
                    {
                        ropeAttachments.Insert(aIndex + 1, attachment);
                        break;
                    }
                    if (otherRaw < attachment.RawPointIndex)
                    {
                        ropeAttachments.Insert(aIndex + 1, attachment);
                        break;
                    }
                    aIndex--;
                    if (aIndex < 0)
                    {
                        ropeAttachments.Insert(0, attachment);
                        break;
                    }
                }
            }
            ValidateAttachments();
        }
        //replace contact
        else
        {
            ropeAttachments[matchingIndex] = attachment;
        }
    }

    public void RemoveAttachment(int index)
    {
        if (index > 0 && index < ropeAttachments.Count) ropeAttachments.RemoveAt(index);
    }

    /// <summary>
    /// Removes any attachments that are connected to the same point index
    /// </summary>
    protected void ValidateAttachments()
    {
        // index then count
        Dictionary<int, int> repeatedIndexes = new Dictionary<int, int>();
        foreach (RopeAttachment a in ropeAttachments)
        {
            int correctedIndex = a.CorrectedPointIndex(ropeBody.Count);
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
                int removeIndex = ropeAttachments.FindLastIndex(x => x.CorrectedPointIndex(ropeBody.Count) == indexCount.Key);
                if (removeIndex < 0) break;
                ropeAttachments.RemoveAt(removeIndex);
                count--;
            }
        }
    }

    public int IndexOf(RopeAttachment item) => ropeAttachments.IndexOf(item);

    public void Insert(int index, RopeAttachment item) => ropeAttachments.Insert(index, item);

    public void RemoveAt(int index) => ropeAttachments.RemoveAt(index);

    public void Add(RopeAttachment item) => ropeAttachments.Add(item);

    public void Clear() => ropeAttachments.Clear();

    public bool Contains(RopeAttachment item) => ropeAttachments.Contains(item);

    public void CopyTo(RopeAttachment[] array, int arrayIndex) => ropeAttachments.CopyTo(array, arrayIndex);

    public bool Remove(RopeAttachment item) => ropeAttachments.Remove(item);

    public IEnumerator<RopeAttachment> GetEnumerator()
    {
        foreach(RopeAttachment ropeAttachment in ropeAttachments)
        {
            yield return ropeAttachment;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
