using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(RopeGameObject), typeof(RopeComponentLength))]
public class RopeComponentDebug : RopeComponentBase
{
    [SerializeField]
    private Color lineColor = Color.red;

    private RopeActorDebug ropeActorDebug;
    public override RopeActorBase RopeActor { get { return ropeActorDebug; } }

    protected override void Start()
    {
        requiredActorTypes.Add(typeof(RopeActorLength));
        base.Start();
    }

    public override void Initialize(Rope rope, IEnumerable<RopeActorBase> requiredActors)
    {
        RopeActorLength ropeActorLength = requiredActors.FirstOrDefault(x => x.GetType() == typeof(RopeActorLength)) as RopeActorLength;

        ropeActorDebug = new RopeActorDebug(
            rope,
            ropeActorLength,
            lineColor
            );
        ropeActorDebug.EnableActor();
    }

    public Color LineColor { get => ropeActorDebug.LineColor; set => ropeActorDebug.LineColor = value; }
}
