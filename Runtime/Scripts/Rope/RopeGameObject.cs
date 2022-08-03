using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class RopeGameObject : MonoBehaviour
{
    [SerializeField]
    private int initialSegmentCount = 20;

    private Rope rope;

    private List<RopeComponentBase> activeComponents = new List<RopeComponentBase>();
    private List<RopeComponentBase> waitingComponents = new List<RopeComponentBase>();

    /*
    public T GetRopeComponent<T>() where T : RopeActorBase
    {
        return (T)ropeComponents.FirstOrDefault(x => x.GetType() == typeof(T));
    }
    */

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

    private bool InitializeComponent(RopeComponentBase addedComponent, IEnumerable<RopeActorBase> requiredActors)
    {
        // make sure it cant add twice
        if (activeComponents.Find(x => x.GetType() == addedComponent.GetType()) == null)
        {
            addedComponent.Initialize(rope, requiredActors);
            activeComponents.Add(addedComponent);
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
        IEnumerable<RopeActorBase> requiredActors = new List<RopeActorBase>();
        foreach (RopeComponentBase waitingComponent in waitingComponents)
        {
            if(CheckAvailability(waitingComponent, out requiredActors)) componentsToSwap.Add(waitingComponent);
        }
        foreach(RopeComponentBase swapComponent in componentsToSwap)
        {
            waitingComponents.Remove(swapComponent);
            InitializeComponent(swapComponent, requiredActors);
        }
        if(componentsToSwap.Count > 0) CheckWaitingList();
    }

    private bool CheckAvailability(RopeComponentBase checkingComponent, out IEnumerable<RopeActorBase> requiredActors)
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
                requiredActors = requiredList;
                return false;
            }
            else
            {
                requiredList.Add(match.RopeActor);
            }
        }
        requiredActors = requiredList;
        return true;
    }


    public void DisableComponent(RopeComponentBase ropeComponent)
    {
        activeComponents.Remove(ropeComponent);
        // TODO: create a safe solution for disabling components

    }

    // Start is called before the first frame update
    // TODO: Move this off of this gameobject?
    void Awake()
    {
        RopeComponentAttachments attachmentComponent = GetComponent<RopeComponentAttachments>();
        RopeBuilder ropeBuilder;
        if (attachmentComponent != null)
        {
            ropeBuilder = new RopeBuilderAttachments(attachmentComponent.InitialAttachments, initialSegmentCount);
        }
        else
        {
            ropeBuilder = new RopeBuilderFromTo(Vector3.one * -5, Vector3.one * 5, initialSegmentCount);
        }

        rope = new Rope(
            this,
            ropeBuilder
            );
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rope.RopeUpdater.UpdateRope(Time.fixedDeltaTime);
    }

}
