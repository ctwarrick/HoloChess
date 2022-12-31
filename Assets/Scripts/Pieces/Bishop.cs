using UnityEngine;

public class Bishop : Piece
{
    #region Fields
    private DiagonalMover _diagonalMover;
    #endregion

    #region Methods
    protected override void Awake()
    {
        _diagonalMover = new DiagonalMover(this);
        base.Awake();
    }

    protected override void GetCaptured()
    {
        base.GetCaptured();

        if (_color == Color.Black)
        {
            if (gameController.CapturedPieces["BlackBishop1"] == false)
            {
                gameController.CapturedPieces["BlackBishop1"] = true;
                FillCapturedSpot("BlackBishop1");
            }
            else
            {
                gameController.CapturedPieces["BlackBishop2"] = true;
                FillCapturedSpot("BlackBishop2");
            }
        }
        else
        {
            if (gameController.CapturedPieces["WhiteBishop1"] == false)
            {
                gameController.CapturedPieces["WhiteBishop1"] = true;
                FillCapturedSpot("WhiteBishop1");
            }
            else
            {
                gameController.CapturedPieces["WhiteBishop2"] = true;
                FillCapturedSpot("WhiteBishop2");
            }
        }
    }

    public override void UpdateActionSquares()
    {
        base.UpdateActionSquares();
        _diagonalMover.AddActionSquares();
    }
    #endregion
}
