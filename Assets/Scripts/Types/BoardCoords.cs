using System;

/// <summary>
/// This class is used for expressing coordinates of chessboard squares from [0,0] (bottom left)
/// to [7,7] (top right).  Methods are used to pass coordinates surrounding the square, with -1
/// values indicating that movement in that direction is off the board.
/// </summary>

public class BoardCoords
{
    #region Fields
    private int _x;
    private int _y;
    #endregion

    #region Constructors
    public BoardCoords(int x, int y)
    {
        if (x >= 0 && x <= 7)
        {
            _x = x;
        }
        else
        {
            _x = -1;
        }

        if (y >= 0 && y <= 7)
        {
            _y = y;
        }
        else
        {
            _y = -1;
        }
    }
    public BoardCoords()
    {
        _x = -1;
        _y = -1;
    }
    #endregion

    #region Properties
    public int X
    {
        get { return _x; }
        set
        {
            if (value >= 0 && value <= 7)
            {
                _x = value;
            }
            else
            {
                _x = -1;
            }
        }
    }
    public int Y
    {
        get { return _y; }
        set
        {
            if (value >= 0 && value <= 7)
            {
                _y = value;
            }
            else
            {
                _y = -1;
            }
        }
    }
    #endregion

    #region Methods
    public BoardCoords Up()
    {
        return new BoardCoords(_x, _y + 1);
    }

    public BoardCoords Down()
    {
        return new BoardCoords(_x, _y - 1);
    }

    public BoardCoords Left()
    {
        return new BoardCoords(_x - 1, _y);
    }

    public BoardCoords Right()
    {
        return new BoardCoords(_x + 1, _y);
    }

    public BoardCoords UpAndLeft()
    {
        return new BoardCoords(_x - 1, _y + 1);
    }

    public BoardCoords UpAndRight()
    {
        return new BoardCoords(_x + 1, _y + 1);
    }

    public BoardCoords DownAndLeft()
    {
        return new BoardCoords(_x - 1, _y - 1);
    }

    public BoardCoords DownAndRight()
    {
        return new BoardCoords(_x + 1, _y - 1);
    }
    #endregion
    #region Equality Operators
    // Reimplementing these to allow for value equality not reference equality
    public static bool operator ==(BoardCoords bc1, BoardCoords bc2)
    {
        return (bc1.X == bc2.X && bc1.Y == bc2.Y);
    }

    public static bool operator !=(BoardCoords bc1, BoardCoords bc2)
    {
        return (bc1.X != bc2.X || bc1.Y != bc2.Y);
    }
    public override bool Equals(object obj)
    {
        return obj is BoardCoords && this == (BoardCoords)obj;
    }

    // Not changing this because BoardCoords objects are not immutable
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}
