

public class StraightLineMover : Mover
{
    public StraightLineMover(Piece piece) : base(piece) { }

    #region Methods
    public override void AddActionSquares()
    {
        if (_piece.Location.X > 0)
        {
            Left();
        }
            if (_piece.Location.Y > 0)
        {
            Down();
        }

        if (_piece.Location.X < 7)
        {
            Right();
        }

        if (_piece.Location.Y < 7)
        {
            Up();
        }
    }

    private void Up()
    {
        for (var i = new BoardCoords(_piece.Location.X, _piece.Location.Y + 1);
             i.Y != -1;
             i = i.Up())
        {
            bool result = _piece.AnalyzeSquare(i);

            if (result == false)
            {
                break;
            }
        }
    }

    private void Down()
    {
        for (var i = new BoardCoords(_piece.Location.X, _piece.Location.Y - 1);
             i.Y != -1;
             i = i.Down())
        {
            bool result = _piece.AnalyzeSquare(i);

            if (result == false)
            {
                break;
            }
        }
    }
    private void Left()
    {
        
        for (var i = new BoardCoords(_piece.Location.X - 1, _piece.Location.Y);
            i.X != -1;
            i = i.Left())
        {
            bool result = _piece.AnalyzeSquare(i);

            if (result == false)
            {
                break;
            }
        }
    }
    private void Right()
    {
        for (var i = new BoardCoords(_piece.Location.X + 1, _piece.Location.Y);
                 i.X != -1;
                 i = i.Right())
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
