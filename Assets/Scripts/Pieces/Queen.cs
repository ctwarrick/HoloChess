using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    private DiagonalMover _diagonalMover;
    private StraightLineMover _straightLineMover;
    protected override void Awake()
    {
        _diagonalMover = new DiagonalMover(this);
        _straightLineMover = new StraightLineMover(this);
        base.Awake();
    }
    protected override void GetCaptured()
    {
        base.GetCaptured();

        if (_color == Color.Black)
        {
            gameController.CapturedPieces["BlackQueen"] = true;
            FillCapturedSpot("BlackQueen");
        }
        else
        {
            gameController.CapturedPieces["WhiteQueen"] = true;
            FillCapturedSpot("WhiteQueen");
        }
    }

    public override void UpdateActionSquares()
    {
        base.UpdateActionSquares();
        _diagonalMover.AddActionSquares();
        _straightLineMover.AddActionSquares();
    }
}
