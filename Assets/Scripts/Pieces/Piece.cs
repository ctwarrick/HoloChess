using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Abstract class that holds info about a GameObject that is a chess piece. 
/// </summary>
public abstract class Piece : MonoBehaviour
{
    #region Color
    [SerializeField]
    protected Color _color;
    #endregion

    #region Audio Clip Fields
    [SerializeField]
    protected AudioClip pieceHitsBoard;

    [SerializeField]
    protected AudioClip pieceHitsPiece;

    [SerializeField]
    protected AudioClip error;
    #endregion

    #region Component Fields
    protected AudioSource audioSource;
    protected MeshCollider meshCollider;

    // This is disabled because I'm apparently overwriting some weird deprecated
    // rigidbody object that MonoBehaviour otherwise inherits from UnityEngine.Component
#pragma warning disable 108
    protected Rigidbody rigidbody;
#pragma warning restore 108
    #endregion

    #region Status Fields
    protected bool _isActive;
    protected bool _isCaptured;
    protected bool _isMoveValid;
    protected BoardCoords _location;
    protected Dictionary<BoardCoords, SquareType> _moveSquares;
    protected Dictionary<BoardCoords, SquareType> _threatSquares;
    protected Vector3 upright;
    #endregion

    #region Offboard Controller Fields
    protected BoardController boardController;
    protected GameController gameController;
    #endregion

    #region Properties
    public BoardCoords Location
    {
        get { return _location; }
        set
        {
            if (value.X >= 0 && value.X <= 7 && value.Y >= 0 && value.Y >= 7)
            {
                _location = value;
            }
            else
            {
                throw new ArgumentException("Coordinates must be a valid board square.");
            }
        }
    }

    public bool IsCaptured
    {
        get { return _isCaptured; }
        set { _isCaptured = value; }
    }

    public Color Color
    {
        get { return _color; }
        // Do you really need this?
        set { _color = value; }
    }

    public Dictionary<BoardCoords, SquareType> MoveSquares
    {
        get { return _moveSquares; }
        set { _moveSquares = value; }
    }
    public Dictionary<BoardCoords, SquareType> ThreatSquares
    {
        get { return _threatSquares; }
        set { _threatSquares = value; }
    }

    private GameController GameController
    {
        get { return gameController; }
        set { gameController = value; }
    }
    #endregion

    #region Methods
    /// <summary> 
    /// Directs a piece that it's been captured and to move to the holding bin
    /// </summary>
    protected virtual void GetCaptured()
    {
        audioSource.PlayOneShot(pieceHitsPiece);
        gameController.BoardUpdate(null, _location);
        _location = new BoardCoords(-1, -1);
    }

    protected virtual void FillCapturedSpot(string pieceName)
    {
        gameController.CapturedPieces[pieceName] = true;
        var newPosition = new Vector3(boardController.CapturedCoords[pieceName].X,
                              (boardController.BoardPosition.y +
                              boardController.BoardHeight),
                              boardController.CapturedCoords[pieceName].Y);
        transform.position = newPosition;
    }

    /// <summary>
    /// Centers a piece on its square after it's either been moved or let go of, then
    /// deactivates it.
    /// </summary>
    public void CenterPieceOnSquare()
    {
        SpaceCoords center = boardController.GetCenter(_location.X, _location.Y);
        var newPosition = new Vector3(center.X,
                                      (boardController.BoardPosition.y +
                                      boardController.BoardHeight),
                                      center.Y);
        transform.position = newPosition;

        var rotation = new Quaternion();
        rotation.eulerAngles = upright;
        transform.rotation = rotation;
        Lock();
    }

    /// <summary>
    /// Returns the correct SquareType value for an occupied square.
    /// </summary>
    /// <param name="square"></param>
    /// <returns></returns>
    public virtual SquareType? CheckSquareStatus(BoardCoords square)
    {
        if (square.X < 0 || square.Y < 0)
        {
            return null;
        }

        if (gameController.ReturnPiece(square) != null)
        {
            if (gameController.ReturnPiece(square).Color == _color)
            {
                return SquareType.Friendly;
            }
            else if (gameController.ReturnPiece(square).GetType().Name == "King")
            {
                return SquareType.Check;
            }
            else
            {
                return SquareType.Attack;
            }
        }
        else
        {
            return SquareType.Move;
        }
    }

    /// <summary>
    /// Spawns colored prefabs on the board to illustrate all the potential moves a touched piece
    /// can legally make
    /// </summary>
    public void DisplayActionSquares()
    {
        foreach (KeyValuePair<BoardCoords, SquareType> item in _moveSquares)
        {
            boardController.SpawnSquare(item.Key.X, item.Key.Y, item.Value);
        }
        foreach (KeyValuePair<BoardCoords, SquareType> item in _threatSquares)
        {
            boardController.SpawnSquare(item.Key.X, item.Key.Y, item.Value);
        }
    }

    /// <summary>
    /// Hides prefabs when you stop touching a Piece GameObject.  This is a wrapper so it can be
    /// tied to a Piece prefab in the hierarchy.
    /// </summary>
    public void HideActionSquares()
    {
        boardController.HideSquares();
    }

    /// <summary>
    /// Freezes a piece, deactivates it, and turns off physics
    /// </summary>
    public void Lock()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        rigidbody.useGravity = false;
        _isActive = false;
    }

    /// <summary>
    /// Activates a piece, engages physics, and allows it to be manipulated after it is picked up.
    /// </summary>
    /// 
    public void Unlock()
    {
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.useGravity = true;
        _isActive = true;
    }

    /// <summary>
    /// Updates the dictionary of legal moves for a given piece when required
    /// </summary>
    public virtual void UpdateActionSquares()
    {
        // Clear out what's there if it exists
        _moveSquares.Clear();
        _threatSquares.Clear();

        // Then do the piece-specific stuff
    }

    protected virtual void Awake()
    {
        // Set up controllers to understand where you are and update the board
        boardController = Camera.main.GetComponent<BoardController>();
        gameController = Camera.main.GetComponent<GameController>();

        // Get initial location on board, and check in on virtual board
        _location = boardController.GetSquareForSpace(transform.position.x, transform.position.z);
        gameController.BoardUpdate(this, _location);

        // Instantiate action squares Dictionaries
        _moveSquares = new Dictionary<BoardCoords, SquareType>();
        _threatSquares = new Dictionary<BoardCoords, SquareType>();


        // Obviously you're not captured or active to start out
        _isCaptured = false;
        _isActive = false;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Set audio source to be able to use it for multiple sounds
        audioSource = GetComponent<AudioSource>();

        // Get rigidbody and mesh collider to lock
        rigidbody = GetComponent<Rigidbody>();
        meshCollider = GetComponent<MeshCollider>();

        // Disable on spawn so pieces don't go crazy sitting on the board
        Lock();

        // Set upright to what it is for everything but kings and bishops, who override
        if (_color == Color.Black)
        {
            upright = new Vector3(0, 0, 90);
        }
        else
        {
            upright = new Vector3(0, 180, 90);
        }
    }

    protected virtual void Update()
    {
        // This resets the location/rotation of a piece if it gets dropped off the board or flung 
        if (transform.position.y < (boardController.BoardPosition.y - 0.01) ||
            Math.Abs(transform.position.x) > (boardController.BoardPosition.x + 0.5) ||
            Math.Abs(transform.position.z) > (boardController.BoardPosition.z + 0.5))
        {
            CenterPieceOnSquare();
        }
    }

    // This takes care of any moves as well, because every move ends with a collision with
    // the board or another piece.
    protected virtual void OnCollisionEnter(Collision coll)
    {
        // if piece isn't active, bail because it's just sitting there touching the board
        if (_isActive == false)
        {
            return;
        }

        // set move to be invalid by default
        _isMoveValid = false;

        // see what you hit
        string tag = coll.gameObject.tag;

        if (tag == "Board")
        {
            HandleBoardCollision(coll);
        }
        else
        {
            HandlePieceCollision(coll);
        }
    }

    /// <summary>
    /// For a given square, checks whether it is in the dictionary of permissible moves for 
    /// that piece.
    /// </summary>
    /// <param name="moveSquare"></param>
    /// <returns></returns>
    protected virtual bool CheckLegalMove(BoardCoords moveSquare)
    {

        foreach (BoardCoords square in _moveSquares.Keys)
        {
            if (square.X == moveSquare.X && square.Y == moveSquare.Y)
            {
                return true;
            }
        }

        return false;
    }

    protected virtual bool CheckThreat(BoardCoords moveSquare)
    {

        foreach (BoardCoords square in _threatSquares.Keys)
        {
            if (square.X == moveSquare.X && square.Y == moveSquare.Y)
            {
                return true;
            }
        }

        return false;
    }

    // Debug method to write to the console
    public void ReportMoves()
    {
        var sb = new StringBuilder();
        // Doing this to keep from reimplementing Color as a class for debugging
        if (this.Color == Color.Black)
        {
            sb.Append("Black ");
        }
        else
        {
            sb.Append("White ");
        }
        sb.Append(this.GetType().ToString() + " ");
        sb.Append("Move Squares:  ");
        foreach (KeyValuePair<BoardCoords, SquareType> actionSquare in MoveSquares)
        {
            sb.Append("[" +
                       actionSquare.Key.X.ToString() +
                       "," +
                       actionSquare.Key.Y.ToString() +
                       "], ");
        }

        string consoleInput = sb.ToString();
        boardController.DebugConsole.Write(consoleInput);
    }
    protected void IllegalMove(BoardCoords boardCoords)
    {
        audioSource.PlayOneShot(error);
        CenterPieceOnSquare();
        boardController.DebugConsole.Write(this.GetType().ToString() +
                                           " tried to move to " +
                                           boardCoords.X.ToString() + 
                                           ", " +
                                           boardCoords.Y.ToString());
    }
    protected void AddMoveSquare(BoardCoords boardCoords, SquareType? squareType)
    {
        if (boardCoords.X >= 0 && 
            boardCoords.Y >= 0 &&
            (squareType == SquareType.Move || squareType == SquareType.Castle))
        {
            _moveSquares.Add(boardCoords, (SquareType)squareType);
        }
    }

    protected void AddThreatSquare(BoardCoords boardCoords, SquareType? squareType)
    {
        if (boardCoords.X >= 0 && 
            boardCoords.Y >= 0 &&
            (squareType == SquareType.Attack || squareType == SquareType.Check))
        {
            _threatSquares.Add(boardCoords, (SquareType)squareType);
        }
    }

    protected virtual void HandleBoardCollision(Collision coll)
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
            MovePiece(landingSquare);
        }
        // if it's illegal, error and reset
        else
        {
            IllegalMove(landingSquare);
        }
    }
    protected virtual void HandlePieceCollision(Collision coll)
    {
        // Get the game object you hit and its position
        Piece pieceYouHit = coll.gameObject.GetComponent<Piece>();
        Vector3 attackPosition = coll.gameObject.transform.position;

        // Figure out what square the attacked piece is on
        var attackedSquare = new BoardCoords();

        // stop compiler from squawking about unused exception
#pragma warning disable 0168
        try
        {
            attackedSquare = boardController.GetSquareForSpace(attackPosition.x, attackPosition.z);
        }
        catch (ArgumentException e)
        {
            IllegalMove(boardController.GetSquareForSpace(attackPosition.x, attackPosition.z));
        }
#pragma warning restore 0168

        // if it's the opposite color and legal, capture, otherwise ignore because it's your own
        if (CheckThreat(attackedSquare) == true)
        {
            pieceYouHit.GetCaptured();
            MovePiece(attackedSquare);
        }
    }

    private void MovePiece(BoardCoords landingSquare)
    {
        gameController.BoardUpdate(null, _location);
        gameController.BoardUpdate(this, landingSquare);
        _location = landingSquare;
        CenterPieceOnSquare();
        gameController.SwitchTurn();
    }
    public virtual bool AnalyzeSquare(BoardCoords square)
    {
        SquareType result = (SquareType)CheckSquareStatus(square);

        if (result == SquareType.Move)
        {
            _moveSquares.Add(square, result);
            return true;
        }
        else if (result == SquareType.Attack || result == SquareType.Check)
        {
            _threatSquares.Add(square, result);
        }
        return false;
    }
    #endregion
}
