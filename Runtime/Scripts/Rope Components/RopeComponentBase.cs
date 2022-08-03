using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(RopeGameObject))]
public abstract class RopeComponentBase : MonoBehaviour 
{
    protected RopeGameObject ropeGameObject;
    public RopeGameObject RopeGameObject { get { return ropeGameObject; } }

    public abstract RopeActorBase RopeActor { get; }

    //protected List<Type> incompatibleTypes = new List<Type>();
    //public List<Type> IncompatibleTypes { get { return incompatibleTypes; } }

    protected List<Type> requiredActorTypes = new List<Type>();
    public List<Type> RequiredActorTypes { get { return requiredActorTypes; } }

    protected virtual void Reset()
    {
        if(GetComponents(this.GetType()).Length > 1)
        {
            Debug.LogWarning("Don't add two of same type of Rope Components!");
            Debug.LogWarning("Destroying duplicate Component", this.gameObject);
            DestroyImmediate(this);
        }
    }

    protected virtual void OnValidate()
    {

    }

    protected virtual void Start()
    {
        ropeGameObject = GetComponent<RopeGameObject>();
        ropeGameObject.RequestComponentInitialization(this);
    }

    public abstract void Initialize(Rope rope, IEnumerable<RopeActorBase> requiredActors);

    protected virtual void OnDisable()
    {
        ropeGameObject.DisableComponent(this);
    }
}
