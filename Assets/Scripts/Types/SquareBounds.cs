using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This struct is used for marking the x and y coordinates of the center of a square
/// in Unity space, and also its maximum and minimum x and y values.  "x" and "y" are
/// from the perspective of the board, not the perspective of Unity transforms.
/// </summary>
public struct SquareBounds
{
    public SquareBounds(float xCenter, float yCenter, float xMin, float yMin, float xMax, float yMax)
    {
        XCenter = xCenter;
        YCenter = yCenter;
        XMin = xMin;
        YMin = yMin;
        XMax = xMax;
        YMax = yMax;
    }

    public float XCenter { get; }
    public float YCenter { get; }
    public float XMin { get; }
    public float YMin { get; }
    public float XMax { get; }
    public float YMax { get; }

}
