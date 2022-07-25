using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class RopeGameObject : MonoBehaviour
{
    private Rope rope;

    public Transform[] attachmentBodies;
    public int[] attachmentIndexes;

    private List<RopeComponentBase> activeComponents = new List<RopeComponentBase>();
    private List<RopeComponentBase> waitingComponents = new List<RopeComponentBase>();

    public void RequestComponentInitialization(RopeComponentBase ropeComponent)
    {
        // check availability will always be true if there are 0
        // required components
        if(CheckAvailability(ropeComponent, out IEnumerable<RopeActorBase> requiredComponents))
        {
            if(InitializeComponent(ropeComponent, requiredComponents)) CheckWaitingList();
        }
        else
        {
            AddToWaitingList(ropeComponent);
        }
    }

    private bool InitializeComponent(RopeComponentBase addedComponent, IEnumerable<RopeActorBase> requiredComponents)
    {
        // make sure it cant add twice
        if (activeComponents.Find(x => x.GetType() == addedComponent.GetType()) == null)
        {
            addedComponent.Initialize(rope, requiredComponents);
            activeComponents.Add(addedComponent);
            rope.AddRopeComponent(addedComponent.RopeActor);
            return true;
        }
        else return false;
        // log error for trying to add multiple of the same?
    }

    private bool AddToWaitingList(RopeComponentBase waitingComponent)
    {
        // make sure it cant add twice
        if (waitingComponents.Find(x => x.GetType() == waitingComponent.GetType()) == null)
        {
            waitingComponents.Add(waitingComponent);
            return true;
        }
        // log error for trying to add multiple of the same?
        else return false;
    }

    private void CheckWaitingList()
    {
        List<RopeComponentBase> componentsToSwap = new List<RopeComponentBase>();
        IEnumerable<RopeActorBase> requiredComponents = new List<RopeActorBase>();
        foreach (RopeComponentBase waitingComponent in waitingComponents)
        {
            if(CheckAvailability(waitingComponent, out requiredComponents)) componentsToSwap.Add(waitingComponent);
        }
        foreach(RopeComponentBase swapComponent in componentsToSwap)
        {
            waitingComponents.Remove(swapComponent);
            InitializeComponent(swapComponent, requiredComponents);
        }
        if(componentsToSwap.Count > 0) CheckWaitingList();
    }

    private bool CheckAvailability(RopeComponentBase checkingComponent, out IEnumerable<RopeActorBase> requiredComponents)
    {
        List<RopeActorBase> requiredList = new List<RopeActorBase>();

        foreach (Type t in checkingComponent.RequiredActorTypes)
        {
            /*
            foreach(RopeComponentBase rcb in activeComponents)
            {
                Debug.Log("Goal type: " + t);
                Debug.Log("Actual type: " + rcb.RopeActor.GetType());
                Debug.Log("match? " + (rcb.RopeActor.GetType() == t));
            }
            Debug.Log(activeComponents.FirstOrDefault(x => x.RopeActor.GetType() == t));
            */
            RopeComponentBase match = activeComponents.FirstOrDefault(x => x.RopeActor.GetType() == t);
            if(match == null)
            {
                requiredComponents = requiredList;
                return false;
            }
            else
            {
                requiredList.Add(match.RopeActor);
            }
        }
        requiredComponents = requiredList;
        return true;
    }


    public void DisableComponent(RopeComponentBase ropeComponent)
    {
        activeComponents.Remove(ropeComponent);
        rope.RemoveRopeComponent(ropeComponent.RopeActor);

        // TODO: create a safe solution for disabling components

    }

    // Start is called before the first frame update
    void Awake()
    {
        RopeAttachment[] attachments = new RopeAttachment[attachmentBodies.Length];
        for(int i = 0; i < attachmentBodies.Length; i++)
        {
            if (attachmentBodies[i].GetComponent<Rigidbody>() != null)
            {
                attachments[i] = new RopeAttachmentRigidbody(
                    attachmentIndexes[i],
                    attachmentBodies[i].GetComponent<Rigidbody>().transform,
                    Vector3.zero,
                    attachmentBodies[i].GetComponent<Rigidbody>()
                    );
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

        rope = new Rope(
            gameObject,
            attachments,
            15,
            .5f
            );
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rope.UpdateRope(Time.fixedDeltaTime);
    }

}
