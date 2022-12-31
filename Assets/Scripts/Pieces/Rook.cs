
public class Rook : PieceThatCastles
{
    private StraightLineMover _straightLineMover;

    protected override void Awake()
    {
        base.Awake();
        _straightLineMover = new StraightLineMover(this);
    }

    protected override void GetCaptured()
    {
        base.GetCaptured();

        if (_color == Color.Black)
        {
            if (gameController.CapturedPieces["BlackRook1"] == false)
            {
                gameController.CapturedPieces["BlackRook1"] = true;
                FillCapturedSpot("BlackRook1");
            }
            else
            {
                gameController.CapturedPieces["BlackRook2"] = true;
                FillCapturedSpot("BlackRook2");
            }
        }
        else
        {
            if (gameController.CapturedPieces["WhiteRook1"] == false)
            {
                gameController.CapturedPieces["WhiteRook1"] = true;
                FillCapturedSpot("WhiteRook1");
            }
            else
            {
                gameController.CapturedPieces["WhiteRook2"] = true;
                FillCapturedSpot("WhiteRook2");
            }
        }
    }
    public override void UpdateActionSquares()
    {
        base.UpdateActionSquares();
        _straightLineMover.AddActionSquares();
    }
    /// <summary>
    /// This method allows the game controller to have the rook move itself after its king castles
    /// </summary>
    /// <param name="square"></param>
    public void MoveAfterCastle(BoardCoords square)
    {
        _location = square;
        CenterPieceOnSquare();
    }
}
