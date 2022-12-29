
public class Rook : PieceThatCastles
{
    private StraightLineMover _straightLineMover;

    protected override void Awake()
    {
        base.Awake();
        _straightLineMover = new StraightLineMover(this);
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
