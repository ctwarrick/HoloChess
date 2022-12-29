
public class DiagonalMover : Mover
{
    public DiagonalMover(Piece piece) : base(piece) { }

    #region Methods
    // Start is called before the first frame update
    public override void AddActionSquares()
    {
        if (_piece.Location.X <= 6 && _piece.Location.Y <= 6)
        {
            UpAndRight();
        }
        
        if (_piece.Location.X >= 1 && _piece.Location.Y <= 6)
        {
            UpAndLeft();
        }

        if (_piece.Location.X >= 1 && _piece.Location.Y >= 1)
        {
            DownAndLeft();
        }

        if (_piece.Location.X <= 6 && _piece.Location.Y >= 1)
        {
            DownAndRight();
        }
    }
    private void UpAndRight()
    {
        for (var i = new BoardCoords(_piece.Location.X + 1, _piece.Location.Y + 1);
             i.X != -1 && i.Y != -1;
             i = i.UpAndRight())
        {
            bool result = _piece.AnalyzeSquare(i);

            if (result == false)
            {
                break;
            }
        }
    }
    private void UpAndLeft()
    {
        for (var i = _piece.Location.UpAndLeft();
             i.X != -1 && i.Y != -1;
             i = i.UpAndLeft())
        {
            bool result = _piece.AnalyzeSquare(i);

            if (result == false)
            {
                break;
            }
        }
        
    }
    private void DownAndRight()
    {
        for (var i = new BoardCoords(_piece.Location.X + 1, _piece.Location.Y - 1);
             i.X != -1 && i.Y != -1;
             i = i.DownAndRight())
        {
            bool result = _piece.AnalyzeSquare(i);

            if (result == false)
            {
                break;
            }
        }
    }
    private void DownAndLeft()
    {
        for (var i = new BoardCoords(_piece.Location.X - 1, _piece.Location.Y - 1);
             i.X != -1 && i.Y != -1;
             i = i.DownAndLeft())
        {
            bool result = _piece.AnalyzeSquare(i);

            if (result == false)
            {
                break;
            }
        }
    }
    #endregion
}