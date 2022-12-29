using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    protected override void Start()
    {
        base.Start();

        if (_color == Color.Black)
        {
            upright = new Vector3(0, 0, 0);
        }
        else
        {
            upright = new Vector3(0, 180, 0);
        }
    }
    public override void UpdateActionSquares()
    {
        base.UpdateActionSquares();

        var checkSquares = new List<BoardCoords>();

        // Hardcode the squares, because knights do the wonky hoppy thing
        if (_location.X < 7 && _location.Y < 6)
        {
            checkSquares.Add(new BoardCoords(_location.X + 1, _location.Y + 2));
        }
        
        if (_location.X > 0 && _location.Y < 6)
        {
            checkSquares.Add(new BoardCoords(_location.X - 1, _location.Y + 2));
        }

        if (_location.X > 0 && _location.Y > 1)
        {
            checkSquares.Add(new BoardCoords(_location.X - 1, _location.Y - 2));
        }

        if (_location.X < 7 && _location.Y > 1)
        {
            checkSquares.Add(new BoardCoords(_location.X + 1, _location.Y - 2));
        }

        if (_location.X < 6 && _location.Y < 7)
        {
            checkSquares.Add(new BoardCoords(_location.X + 2, _location.Y + 1));
        }

        if (_location.X > 1 && _location.Y < 7)
        {
            checkSquares.Add(new BoardCoords(_location.X - 2, _location.Y + 1));
        }

        if (_location.X > 1 && _location.Y > 0)
        {
            checkSquares.Add(new BoardCoords(_location.X - 2, _location.Y - 1));
        }

        if (_location.X < 6 && _location.Y > 0)
        {
            checkSquares.Add(new BoardCoords(_location.X + 2, _location.Y - 1));
        }

        // Doesnt use AnalyzeSquare because knights check all squares always
        foreach (BoardCoords square in checkSquares)
        {
            SquareType result = (SquareType)CheckSquareStatus(square);

            if (result == SquareType.Move)
            {
                _moveSquares.Add(square, result);
            }
            else if (result == SquareType.Attack || result == SquareType.Check)
            {
                _threatSquares.Add(square, result);
            }
        }
    }
}
