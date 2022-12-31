using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that is a pawn-specific implementation of Piece.  Also implements isFirstMove a la
/// PieceThatCastles, but differently since pawns can be captured en passant by other pawns.
/// </summary>
public class Pawn : Piece
{
    #region Fields
    private bool _isFirstMove;
    private bool _isEnPassant;
    #endregion

    #region Properties
    public bool IsEnPassant
    {
        get { return _isEnPassant; }
        set { _isEnPassant = value; }
    }

    public bool IsFirstMove
    {
        get { return _isFirstMove; }
    }
    #endregion

    #region Methods
    // Start is called before the first frame update
    protected override void Awake()
    {
        _isEnPassant = false;
        _isFirstMove = true;

        base.Awake();
    }

#nullable enable
    protected override bool CheckLegalMove(BoardCoords moveSquare)
    {
        Piece? squareContents = gameController.ReturnPiece(moveSquare);

        foreach (BoardCoords square in _moveSquares.Keys)
        {
            if (square.X == moveSquare.X && square.Y == moveSquare.Y && squareContents == null)
            {
                return true;
            }
        }

        foreach (BoardCoords square in _threatSquares.Keys)
        {
            if (square.X == moveSquare.X && square.Y == moveSquare.Y && squareContents != null)
            {
                return true;
            }
        }

        return false;
    }
#nullable disable

    protected override void GetCaptured()
    {
        base.GetCaptured();

        if (_color == Color.Black)
        {
            // Move to the slot in the array corresponding to the captured black pawns
            SpaceCoords newCoords = boardController.CapturedBlackPawnCoords[gameController.CapturedBlackPawnCount];
            var newPosition = new Vector3(newCoords.X,
                                          (boardController.BoardPosition.y + 
                                           boardController.BoardHeight),
                                          newCoords.Y);
            transform.position = newPosition;

            gameController.CapturedBlackPawnCount++;
        }
        else
        {
            // Move to the slot in the array corresponding to the captured white pawns
            SpaceCoords newCoords = boardController.CapturedWhitePawnCoords[gameController.CapturedWhitePawnCount];
            var newPosition = new Vector3(newCoords.X,
                                          (boardController.BoardPosition.y +
                                           boardController.BoardHeight),
                                          newCoords.Y);
            transform.position = newPosition;

            gameController.CapturedWhitePawnCount++;
        }
    }

#nullable enable
    public override void UpdateActionSquares()
    {
        base.UpdateActionSquares();

        CheckMoveSquares();
        CheckThreatSquares();
        CheckEnPassantSquares();
    }
#nullable disable
    protected override void OnCollisionEnter(Collision coll)
    {
        base.OnCollisionEnter(coll);

        if (_isFirstMove == true && _isMoveValid == true)
        {
            // don't look for the second move anymore
            _isFirstMove = false;

            // Nothing else can be en passant after this pawn moves
            gameController.PurgeEnPassant();

            // Set this pawn as capturable via en passant
            _isEnPassant = true;
        }
        else if (_isEnPassant == true && _isMoveValid == true)
        {
            _isEnPassant = false;
        }
    }
#nullable enable
    private void CheckThreatSquares()
    {
        var candidateSquares = new List<BoardCoords>();
        if (_color == Color.Black)
        {
            candidateSquares.Add(_location.DownAndLeft());
            candidateSquares.Add(_location.DownAndRight());
        }
        else
        {
            candidateSquares.Add(_location.UpAndLeft());
            candidateSquares.Add(_location.UpAndRight());
        }

        foreach(BoardCoords square in candidateSquares)
        {
            Piece? testPiece = gameController.ReturnPiece(square);
            
            if (testPiece == null)
            {
                continue;
            }

            if (testPiece.Color != _color &&
                testPiece.GetType().ToString() == "King")
            {
                AddThreatSquare(square, SquareType.Check);
            }
            else if (testPiece.Color != _color)
            {
                AddThreatSquare(square, SquareType.Attack);
            }
        }
    }

    private void CheckEnPassantSquares()
    {
        var candidateSquares = new List<BoardCoords>();
        candidateSquares.Add(_location.Left());
        candidateSquares.Add(_location.Right());

        foreach (BoardCoords square in candidateSquares)
        {
            Piece? testPiece = gameController.ReturnPiece(square);

            if (testPiece == null || testPiece.GetType().ToString() != "Pawn")
            {
                continue;
            }

            Pawn testPawn = (Pawn)testPiece;

            if (testPawn.Color != _color && testPawn.IsEnPassant == true)
            {
                AddThreatSquare(square, SquareType.Attack);
            }
        }
    }
#nullable disable

    private void CheckMoveSquares()
    {
        var moveSquare = ShiftMoveSquare(_location);

        AddMoveSquare(moveSquare);

        if (IsFirstMove == true)
        {
            moveSquare = ShiftMoveSquare(moveSquare);

            AddMoveSquare(moveSquare);
        }
    }

    private BoardCoords ShiftMoveSquare(BoardCoords square)
    {
        if (_color == Color.Black)
        {
            return square.Down();
        }
        else
        {
            return square.Up();
        }
    }
#nullable enable
    private bool IsSquareOccupied(BoardCoords square)
    {
        Piece? testPiece = gameController.ReturnPiece(square);

        if (testPiece == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
#nullable disable
    private void AddMoveSquare(BoardCoords square)
    {
        if (IsSquareOccupied(square) == false)
        {
            AddMoveSquare(square, SquareType.Move);
        }
    }
    #endregion
}
