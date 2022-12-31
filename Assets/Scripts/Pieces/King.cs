using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : PieceThatCastles
{
    protected override void Start()
    {
        base.Start();
        if (_color == Color.Black)
        {
            upright = new Vector3(0,90, 90);
        }
        else
        {
            upright = new Vector3(0, -90, 90);
        }
    }

    /// <summary>
    /// This shouldn't actually get invoked; it's only here so the other pieces can be captured.
    /// </summary>
    protected override void GetCaptured()
    {
        throw new InvalidOperationException("Kings cannot be captured, only checkmated!");
    }

    /// <summary>
    /// This shouldn't actually get invoked; it's only here so the other pieces can be captured.
    /// </summary>
    protected override void FillCapturedSpot(string pieceName)
    {
        throw new InvalidOperationException("Kings cannot be captured, only checkmated!");
    }

    public override void UpdateActionSquares()
    {
        // Clear out what's there
        base.UpdateActionSquares();

        // Search around the king one square
        var checkSquares = new List<BoardCoords>();

        if (_location.Up().X >=0 && _location.Up().Y >=0)
        {
            checkSquares.Add(_location.Up());
        }

        if (_location.UpAndRight().X >= 0 && _location.UpAndRight().Y >= 0)
        {
            checkSquares.Add(_location.UpAndRight());
        }

        if (_location.Right().X >= 0 && _location.Right().Y >= 0)
        {
            checkSquares.Add(_location.Right());
        }

        if (_location.DownAndRight().X >= 0 && _location.DownAndRight().Y >= 0)
        {
            checkSquares.Add(_location.DownAndRight());
        }

        if (_location.Down().X >= 0 && _location.Down().Y >= 0)
        {
            checkSquares.Add(_location.Down());
        }

        if (_location.DownAndLeft().X >= 0 && _location.DownAndLeft().Y >= 0)
        {
            checkSquares.Add(_location.DownAndLeft());
        }

        if (_location.Left().X >= 0 && _location.Left().Y >= 0)
        {
            checkSquares.Add(_location.Left());
        }

        if (_location.UpAndLeft().X >= 0 && _location.UpAndLeft().Y >= 0)
        {
            checkSquares.Add(_location.UpAndLeft());
        }

        foreach(BoardCoords checkSquare in checkSquares)
        {
            SquareType? squareType = CheckSquareStatus(checkSquare);

            if (squareType == null)
            {
                continue;
            }
            else if (squareType.Value == SquareType.Move)
            {
                AddMoveSquare(checkSquare, SquareType.Move);
            }
            else if (squareType.Value == SquareType.Attack)
            {
                AddThreatSquare(checkSquare, SquareType.Attack);
            }
        }
        
        // If it's a first move, check castling to king and queen side if not in check
        if (_isFirstMove == true && gameController.IsKingChecked(_color) == false)
        {
            // Hardcode the squares to check because they're always the same
            CheckAndAddCastleSquares();            
        }
    }

    // Override because kings can't move into check
#nullable enable
    public override SquareType? CheckSquareStatus(BoardCoords square)
    {
        Piece? squareContents = gameController.ReturnPiece(square);

        var checkColor = new Color();

        if (this.Color == Color.Black)
        {
            checkColor = Color.White;
        }    
        else
        {
            checkColor = Color.Black;
        }

        bool isSquareChecked = gameController.IsSquareChecked(square, checkColor);

        if (square.X < 0 || square.Y < 0)
        {
            return null;
        }

        if (squareContents != null)
        {
            Piece squareOccupant = squareContents as Piece;

            if (squareOccupant.Color != this.Color && isSquareChecked == false)
            {
                return SquareType.Attack;
            }
            else if (squareOccupant.Color == this.Color)
            {
                return SquareType.Friendly;
            }
        }
        else if (isSquareChecked == true)
        {
            return SquareType.Check;
        }
        return SquareType.Move;
    }
#nullable disable
    private void CheckAndAddCastleSquares()
    {
        if (gameController.CanCastleQueenside(_color) == true)
        {
            AddMoveSquare(new BoardCoords(2, _location.Y), SquareType.Castle);
        }
        
        if (gameController.CanCastleKingside(_color) == true)
        {
            AddMoveSquare(new BoardCoords(6, _location.Y), SquareType.Castle);
        }
    }

    protected override void HandleBoardCollision(Collision coll)
    {
        // find where you landed and check if it's legal
        ContactPoint contact = coll.GetContact(0);
        Vector3 position = contact.point;
        var landingSquare = new BoardCoords();

        // stop compiler from squawking about unused exception
#pragma warning disable 0168
        try
        {
            landingSquare = boardController.GetSquareForSpace(position.x, position.z);
        }
        catch (ArgumentException e)
        {
            IllegalMove(boardController.GetSquareForSpace(position.x, position.z));
        }
#pragma warning restore 0168

        // if it's a legal move, move the piece
        if (CheckLegalMove(landingSquare) == true)
        {
            _isMoveValid = true;

            // Play the sound for hitting the board
            audioSource.PlayOneShot(pieceHitsBoard);

            // Update the virtual board and the piece's own awareness
            gameController.BoardUpdate(null, _location);
            gameController.BoardUpdate(this, landingSquare);
            _location = landingSquare;
            CenterPieceOnSquare();
            
            if (_moveSquares[landingSquare] == SquareType.Castle &&
                landingSquare.X == gameController.QueensideCastleColumn)
            {
                gameController.CastleQueenside(_color);
            }
            else if (CheckLegalMove(landingSquare) == true &&
                 _moveSquares[landingSquare] == SquareType.Castle &&
                 landingSquare.X == gameController.KingsideCastleColumn)
            {
                gameController.CastleKingside(_color);
            }
            
            gameController.SwitchTurn();
        }
        else
        {
            IllegalMove(landingSquare);
        }
    }
}
