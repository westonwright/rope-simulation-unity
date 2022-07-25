using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSimulatorDebug
{
    private Color lineColor;
    public Color LineColor { get => lineColor; set => lineColor = value; }

    public RopeSimulatorDebug(Color lineColor)
    {
        this.lineColor = lineColor;
    }

    public void DrawLines(IEnumerable<RopeStick> sticks)
    {
        foreach(RopeStick s in sticks)
        {
            Debug.DrawLine(s.PointA.Position, s.PointB.Position, LineColor);
        }
    }
}
