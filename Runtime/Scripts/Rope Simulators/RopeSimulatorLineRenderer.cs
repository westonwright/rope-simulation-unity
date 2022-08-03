using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// with help from https://www.habrador.com/tutorials/interpolation/1-catmull-rom-splines/

public class RopeSimulatorLineRenderer
{
    private LineRenderer lineRenderer;

    private float smoothingResolution;
    public float SmoothingResolution { get { return smoothingResolution; } set { smoothingResolution = Mathf.Max(value, 0); } }


    public RopeSimulatorLineRenderer(LineRenderer lineRenderer, float smoothingResolution)
    {
        this.lineRenderer = lineRenderer;
        this.smoothingResolution = smoothingResolution;
    }

    public void UpdateLineRenderer(IEnumerable<IRopePointPosition> ropePoints)
    {
        List<Vector3> basePoints = new List<Vector3>();
        foreach (IRopePointPosition ropePoint in ropePoints) basePoints.Add(ropePoint.Position);

        Vector3[] newPoints = SmoothedPositions(basePoints);
        lineRenderer.positionCount = newPoints.Length;
        lineRenderer.SetPositions(newPoints);

        //foreach(Vector3 v3 in newPoints) Debug.DrawRay(v3, Vector3.up, Color.magenta);
        //for(int i = 0; i < newPoints.Length; i++) Debug.DrawRay(newPoints[i], Vector3.up, Color.Lerp(Color.black, Color.magenta, (float) i / newPoints.Length));
    }

    private Vector3[] SmoothedPositions(IList<Vector3> basePoints)
    {
        if (smoothingResolution == 1 || basePoints.Count <= 2)
        {
            Vector3[] output = new Vector3[basePoints.Count];
            for (int i = 0; i < basePoints.Count; i++) output[i] = basePoints[i];
            // just return the unaltered list
            return output;
        }
        /*
        if (basePoints.Count == 3)
        {
            Vector3 middlePoint = basePoints[1];
            basePoints[1] = Vector3.Lerp(basePoints[0], middlePoint, .5f);
            basePoints.Insert(2, Vector3.Lerp(middlePoint, basePoints[2], .5f));
        }
        */

        // simulates points added to the beginning and end of the rope
        // so that smoothing can be applied to the whole rope
        basePoints.Insert(0, basePoints[0] + (basePoints[0] - basePoints[1]).normalized);
        basePoints.Add(basePoints[basePoints.Count - 1] + (basePoints[basePoints.Count - 1] - basePoints[basePoints.Count - 2]).normalized);

        List<Vector3> newPoints = new List<Vector3>();
        newPoints.Add(basePoints[1]); // need to add this here or it will get skipped later

        for (int i = 0; i < basePoints.Count; i++)
        {
            if (i == 0 || i == basePoints.Count - 2 || i == basePoints.Count - 1)
            {
                continue;
            }
            CalculatePoints(basePoints, newPoints, i);
        }

        return newPoints.ToArray();
    }

    private void CalculatePoints(IList<Vector3> basePoints, IList<Vector3> newPoints, int index)
    {
        Vector3 p0 = basePoints[index - 1];
        Vector3 p1 = basePoints[index];
        Vector3 p2 = basePoints[index + 1];
        Vector3 p3 = basePoints[index + 2];

        Vector3 lastPos = p1;

        int loops = Mathf.FloorToInt(1f / smoothingResolution);
        float resolution = 1f / loops;

        for(int i = 1; i <= loops; i++)
        {
            float t = i * resolution;
            newPoints.Add(GetCatmullRomPosition(t, p0, p1, p2, p3));
        }
    }

    private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        //The cubic polynomial: a + b * t + c * t^2 + d * t^3
        Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

        return pos;
    }
}
