
public abstract class Mover
{
    protected Piece _piece;
    

    protected Mover(Piece piece)
    {
        _piece = piece;
    }

    public abstract void AddActionSquares();
}
