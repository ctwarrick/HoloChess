using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This struct is used for expressing coordinates in Unity space relative to the chessboard's
/// X and Y axes.
/// </summary>
public struct SpaceCoords
{
    public SpaceCoords(float x, float y)
    {
        X = x;
        Y = y;
    }

    public float X { get; }
    public float Y { get; }

}
