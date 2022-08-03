using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

// TODO: with only one collision, find a better method for determining which direciton to rotate
// TODO: with two collisions on a sharp surface, determin which direciton to rotate in

public class Rope
{
    private RopeGameObject ropeGameObject;
    public RopeGameObject RopeGameObject { get { return ropeGameObject; } }

    private RopeBody ropeBody;
    public RopeBody RopeBody { get { return ropeBody; } }

    private RopeUpdater ropeUpdater;
    public RopeUpdater RopeUpdater { get { return ropeUpdater; } }

    public Rope(
        RopeGameObject ropeGameObject,
        RopeBuilder ropeBuilder
        )
    {
        this.ropeGameObject = ropeGameObject;
        this.ropeBody = new RopeBody(ropeBuilder);
        this.ropeUpdater = new RopeUpdater();
    }
}
