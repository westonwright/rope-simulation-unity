using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RopeGameObject))]
public class RopeComponentLineRenderer : RopeComponentBase
{
    #region Constant properties
    const string _linePrefabName = "DefaultLineRenderer";
    const string _lineMaterialName = "DefaultLineMaterial";
    #endregion
    [SerializeField]
    private LineRenderer lineRendererPrefab;
    [SerializeField]
    private Material lineMaterial;
    [SerializeField]
    [Range(.05f, 1f)]
    [Tooltip("Lower is higher quality")]
    private float smoothingResolution = .5f;
    [SerializeField]
    [Tooltip("This is optional")]
    private Transform parentTransform = null;

    private LineRenderer lineRenderer;

    private RopeActorLineRenderer ropeActorLineRenderer;
    public override RopeActorBase RopeActor { get { return ropeActorLineRenderer; } }

    public override void Initialize(Rope rope, IEnumerable<RopeActorBase> requiredActors)
    {
        lineRenderer = Instantiate(lineRendererPrefab, parentTransform ?? transform);
        lineRenderer.material = lineMaterial;
        ropeActorLineRenderer = new RopeActorLineRenderer(
            rope,
            lineRenderer,
            smoothingResolution
            );
        ropeActorLineRenderer.EnableActor();
    }

    protected override void Reset()
    {
        base.Reset();
        lineRendererPrefab = Resources.Load<LineRenderer>(_linePrefabName);
        lineMaterial = Resources.Load<Material>(_lineMaterialName);
        // set default value for line renderer here!
    }

    public float SmoothingResolution { get => ropeActorLineRenderer.SmoothingResolution; set => ropeActorLineRenderer.SmoothingResolution = value; }
}