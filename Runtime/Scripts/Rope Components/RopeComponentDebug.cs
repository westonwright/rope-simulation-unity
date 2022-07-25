using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RopeGameObject))]
public class RopeComponentDebug : RopeComponentBase
{
    [SerializeField]
    private Color lineColor = Color.red;

    private RopeActorDebug ropeActorDebug;
    public override RopeActorBase RopeActor { get { return ropeActorDebug; } }

    public override void Initialize(Rope rope, IEnumerable<RopeActorBase> requiredActors)
    {
        ropeActorDebug = new RopeActorDebug(
            rope,
            lineColor
            );
    }

    public Color LineColor { get => ropeActorDebug.LineColor; set => ropeActorDebug.LineColor = value; }
}
