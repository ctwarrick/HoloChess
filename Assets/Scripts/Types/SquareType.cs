using System;

/// <summary>
/// Treats different types of squares differently for purposes of threat and movement.
/// </summary>
public enum SquareType
{
    /// <summary>
    /// A square that currently holds a piece (non-king) that is threatened.
    /// </summary>
    Attack,
    /// <summary>
    /// A square that holds a king that is threatened.
    /// </summary>
    Check,
    /// <summary>
    /// A square to which a legal move can be made without capturing a piece, although opposing 
    /// kings may not be able to move through it unless it's from a pawn.
    /// </summary>
    Move,
    /// <summary>
    /// A square to which a king can castle, split out from "move" because you can't capture a 
    /// piece when castling.
    /// </summary>
    Castle,
    /// <summary>
    /// A square with a piece of the same color on it.  Stops movement.
    /// </summary>
    Friendly
}