using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An abstract class for pawns, kings, and rooks that extends Piece, because these pieces
/// care whether or not it's their first move.
/// </summary>
public abstract class PieceThatCastles : Piece
{
    #region Fields
    protected bool _isFirstMove;
    #endregion

    #region Properties
    public bool IsFirstMove
    {
        get { return _isFirstMove; }
    }
    #endregion

    #region Methods
    protected override void Awake()
    {
        base.Awake();
        // Call this on Awake otherwise pawns will not recognize it's their first move.
        _isFirstMove = true;
    }

    protected override void GetCaptured()
    {
        base.GetCaptured();
    }

    protected override void OnCollisionEnter(Collision coll)
    {
        base.OnCollisionEnter(coll);

        if (_isFirstMove == true && _isMoveValid == true)
        {
            _isFirstMove = false;
        }
    }
    #endregion
}
