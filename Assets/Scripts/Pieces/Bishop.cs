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
    public override void UpdateActionSquares()
    {
        base.UpdateActionSquares();
        _diagonalMover.AddActionSquares();
    }
    #endregion
}
