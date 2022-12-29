using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Constants
    public const int BlackBackRow = 7;
    public const int WhiteBackRow = 0;
    public const int KingsideCastle = 5;
    public const int QueensideCastle = 3;
    public const int KingsideRookColumn = 7;
    public const int QueensideRookColumn = 0;
    #endregion

    #region Fields
    // Board Controller has the debug log
    private BoardController _boardController;
    // This holds Piece objects to show where the pieces are on the board
    private Piece[,] _virtualBoard;

    // These two lists hold all the attacked squares for each side so kings can't move into check
    List<BoardCoords> _whiteCheckSquares;
    List<BoardCoords> _blackCheckSquares;

    private Color _activeTurn;
    private bool _isWhiteKingChecked;
    private bool _isBlackKingChecked;
    #endregion

    #region Properties
    public List<BoardCoords> WhiteCheckSquares
    {
        get { return _whiteCheckSquares; }
        set { _whiteCheckSquares = value; }
    }
    public List<BoardCoords> BlackCheckSquares
    {
        get { return _blackCheckSquares; }
        set { _blackCheckSquares = value; }
    }

    public int QueensideCastleColumn
    {
        get { return QueensideCastle; }
    }

    public int KingsideCastleColumn
    {
        get { return KingsideCastle; }
    }

    public Color ActiveTurn
    {
        get { return _activeTurn; }
    }
    #endregion

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        _activeTurn = Color.White;
        _virtualBoard = new Piece[8, 8];
        _whiteCheckSquares = new List<BoardCoords>();
        _blackCheckSquares = new List<BoardCoords>();
        _boardController = gameObject.GetComponent<BoardController>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SwitchTurn()
    {
        ReSync();

        // Clear debug console
        _boardController.DebugConsole.Clear();

        // Switch active side
        if (_activeTurn == Color.Black)
        {
            _activeTurn = Color.White;
            _boardController.DebugConsole.Write("White's Turn");
        }
        else
        {
            _activeTurn = Color.Black;
            _boardController.DebugConsole.Write("Black's Turn");
        }

#nullable enable
        foreach (Piece? piece in _virtualBoard)
        {
            if (piece != null)
            {
                piece.ReportMoves();
            }
            
        }
    }
#nullable disable

    public void ReSync()
    {
        _whiteCheckSquares.Clear();
        _blackCheckSquares.Clear();

        // Re-sync piece awareness
        for (int x = 0; x <= 7; x++)
        {
            for (int y = 0; y <= 7; y++)
            {
                if (_virtualBoard[x, y] != null)
                {
                    _virtualBoard[x, y].UpdateActionSquares();

                    // Add attacked squares to list of checked squares for each side
                    foreach (KeyValuePair<BoardCoords, SquareType> kvp in
                                     _virtualBoard[x, y].MoveSquares)
                    {
                        if (_virtualBoard[x, y] is Pawn pawn)
                        {
                            if (kvp.Value == SquareType.Attack || kvp.Value == SquareType.Check)
                            {
                                if (pawn.Color == Color.Black &&
                                    _blackCheckSquares.Contains(kvp.Key) == false) 
                                {
                                    _blackCheckSquares.Add(kvp.Key);
                                }
                                else if (pawn.Color == Color.White &&
                                         _whiteCheckSquares.Contains(kvp.Key) == false)
                                {
                                    _whiteCheckSquares.Add(kvp.Key);
                                }
                            }
                        }
                        else
                        {
                            if (kvp.Value == SquareType.Move || kvp.Value == SquareType.Attack)
                            {
                                if (_virtualBoard[x, y].Color == Color.Black)
                                {
                                    _blackCheckSquares.Add(kvp.Key);
                                }
                                else
                                {
                                    _whiteCheckSquares.Add(kvp.Key);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

#nullable enable
    public Piece? ReturnPiece(BoardCoords coords)
    {
        if (coords.X < 0 || coords.Y < 0)
        {
            return null;
        }
        else if (_virtualBoard[coords.X, coords.Y] == null)
        {
            return null;
        }
        else
        {
            return _virtualBoard[coords.X, coords.Y];
        }
    }
#nullable disable

    public bool IsSquareChecked(BoardCoords coords, Color color)
    {
        if (color == Color.Black)
        {
            if (_blackCheckSquares.Contains(coords))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (_whiteCheckSquares.Contains(coords))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public bool IsKingChecked(Color color)
    {
        if (color == Color.Black)
        {
            if (_isBlackKingChecked == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (_isWhiteKingChecked == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

#nullable enable
    public void BoardUpdate(Piece? piece, BoardCoords coords)
    {
        _virtualBoard[coords.X, coords.Y] = piece;
    }

    public void PurgeEnPassant()
    {
        for (int x = 0; x <= 7; x++)
        {
            for (int y = 0; y <= 7; y++)
            {
                if (_virtualBoard[x,y] is Pawn pawn)
                {
                    pawn.IsEnPassant = false;
                }
            }
        }
    }
#nullable disable
    public bool CanCastleQueenside(Color color)
    {
        int y = FindBackRow(color);

        if (ReturnPiece(new BoardCoords(0, y)).GetType().Name == "Rook" &&
            ReturnPiece(new BoardCoords(1, y)) == null &&
            ReturnPiece(new BoardCoords(2, y)) == null &&
            ReturnPiece(new BoardCoords(3, y)) == null &&
            IsSquareChecked(new BoardCoords(1, y), color) == false &&
            IsSquareChecked(new BoardCoords(2, y), color) == false &&
            IsSquareChecked(new BoardCoords(3, y), color) == false)
        {
            Rook testRook = ReturnPiece(new BoardCoords(0, y)) as Rook;
            if (testRook.IsFirstMove == true)
            {
                return true;
            }
        }
        return false;
    }

    public bool CanCastleKingside(Color color)
    {
        int y = FindBackRow(color);

        if (ReturnPiece(new BoardCoords(5, y)) == null &&
            ReturnPiece(new BoardCoords(6, y)) == null &&
            IsSquareChecked(new BoardCoords(5, y), color) == false &&
            IsSquareChecked(new BoardCoords(6, y), color) == false &&
            ReturnPiece(new BoardCoords(7, y)).GetType().Name == "Rook")
        {
            Rook testRook = ReturnPiece(new BoardCoords(7, y)) as Rook;
            if (testRook.IsFirstMove == true)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Hardcodes the logic for moving the queenside rook to cover the king when
    /// castling queenside
    /// </summary>
    /// <param name="color"></param>
    public void CastleKingside(Color color)
    {
        if (CanCastleKingside(color) == false)
        {
            throw new InvalidOperationException(color.ToString() +
                                                "Cannot legally castle to kingside.");
        }

        BoardCoords oldSquare = new BoardCoords();
        BoardCoords newSquare = new BoardCoords();
        oldSquare.X = KingsideRookColumn;
        newSquare.X = KingsideCastle;

        if (color == Color.Black)
        {
            oldSquare.Y = BlackBackRow;
            newSquare.Y = BlackBackRow;
        }
        else
        {
            oldSquare.Y = WhiteBackRow;
            newSquare.Y = WhiteBackRow;
        }

        Rook castleRook = (Rook)ReturnPiece(oldSquare);

        castleRook.MoveAfterCastle(newSquare);
    }

    public void CastleQueenside(Color color)
    {
        if (CanCastleQueenside(color) == false)
        {
            throw new InvalidOperationException(color.ToString() +
                                                "Cannot legally castle to queenside.");
        }

        BoardCoords oldSquare = new BoardCoords();
        BoardCoords newSquare = new BoardCoords();
        oldSquare.X = QueensideRookColumn;
        newSquare.X = QueensideCastleColumn;

        if (color == Color.Black)
        {
            oldSquare.Y = BlackBackRow;
            newSquare.Y = BlackBackRow;
        }
        else
        {
            oldSquare.Y = WhiteBackRow;
            newSquare.Y = WhiteBackRow;
        }

        Rook castleRook = (Rook)ReturnPiece(oldSquare);

        castleRook.MoveAfterCastle(newSquare);
    }

    private int FindBackRow(Color color)
    {
        var y = new int();

        if (color == Color.Black)
        {
            y = BlackBackRow;
        }
        else
        {
            y = WhiteBackRow;
        }

        return y;
    }
    #endregion
}

