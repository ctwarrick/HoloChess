using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

/// <summary>
/// The BoardController class is used to create and manage the chess board.  
/// </summary>
public class BoardController : MonoBehaviour
{
    #region Board Object Fields
    [SerializeField]
    GameObject mockBoard;

    [SerializeField]
    GameObject realBoard;
    #endregion

    #region Piece Object Fields
    [SerializeField]
    GameObject whitePawn;

    [SerializeField]
    GameObject whiteRook;

    [SerializeField]
    GameObject whiteKnight;

    [SerializeField]
    GameObject whiteBishop;

    [SerializeField]
    GameObject whiteQueen;

    [SerializeField]
    GameObject whiteKing;

    [SerializeField]
    GameObject blackPawn;

    [SerializeField]
    GameObject blackRook;

    [SerializeField]
    GameObject blackKnight;

    [SerializeField]
    GameObject blackBishop;

    [SerializeField]
    GameObject blackQueen;

    [SerializeField]
    GameObject blackKing;

    #endregion

    #region Board Square Object Fields
    [SerializeField]
    GameObject attackSquare;

    [SerializeField]
    GameObject moveSquare;

    [SerializeField]
    GameObject checkSquare;
    #endregion

    #region Debug Console Field
    private DebugConsole _debugConsole;
    #endregion

    #region Board Array and Value Fields
    // This holds the spatial coordinates each square
    private SquareBounds[,] _boardSquares;
    // Coordinates of the board in Unity space
    private Vector3 _boardPosition;
    // The width of a square in Unity space
    private float _squareWidth;
    // The height of a square in Unity space
    private float _squareHeight;
    // This holds the Y value of the base of a piece.  Will change as you scale the board.
    private float _boardHeight = 0.005f;

    // Additional spacing for square highlights so they don't act weird
    private const float ObjectSpacing = 0.005f;

    #endregion

    #region Properties

    public float BoardHeight
    {
        get { return _boardHeight; }
    }
    public Vector3 BoardPosition
    {
        get { return _boardPosition; }
    }

    public DebugConsole DebugConsole
    {
        get { return _debugConsole; }
    }
    #endregion

    #region Methods
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnBoard()
    {
        // First get position of mock board, destroy it, instantiate real board
        GameObject instantiatedMockBoard = GameObject.FindGameObjectWithTag("MockBoard");
        _boardPosition = instantiatedMockBoard.transform.position;
        Destroy(instantiatedMockBoard);
        Instantiate(realBoard, _boardPosition, Quaternion.identity);
        GameObject console = GameObject.FindGameObjectWithTag("Console");
        _debugConsole = console.GetComponent<DebugConsole>();

        // First create a board; [0,0] is bottom left [x,y]
        _boardSquares = new SquareBounds[8, 8];
        SpaceCoords[,] boardCenters = new SpaceCoords[8, 8];

        // Import coords from CSV
        string filePath = Path.Combine(Application.streamingAssetsPath, "BoardCoords.csv");
        var sr = new StreamReader(filePath);

        // i is the row; split each row into strings of x,y coordinates
        for (int i = 0; i <= 7; i++)
        {
            string line = sr.ReadLine();
            string[] coordText = line.Split(',');

            // j is the column; take each x,y coordinate in the row and stuff into temp array
            for (int j = 0; j <= 7; j++)
            {
                string[] coords = coordText[j].Split(' ');
                float x = float.Parse(coords[0]);
                float y = float.Parse(coords[1]);
                boardCenters[j, i] = new SpaceCoords((x + _boardPosition.x),
                                                 (y + _boardPosition.z));
            }
        }

        // Get the difference between two centers for width and height of a square
        _squareWidth = boardCenters[1, 0].X - boardCenters[0, 0].X;
        _squareHeight = boardCenters[0, 1].Y - boardCenters[0,0].Y;

        // Populate the _boardSquares array with the center of each array and its boundaries
        for (int x = 0; x<=7; x++)
        {
            for (int y=0; y<=7; y++)
            {
                var square = new SquareBounds(boardCenters[x, y].X,
                                              boardCenters[x, y].Y,
                                              boardCenters[x, y].X - (_squareWidth / 2),
                                              boardCenters[x, y].Y - (_squareHeight / 2),
                                              boardCenters[x, y].X + (_squareWidth / 2),
                                              boardCenters[x, y].Y + (_squareHeight / 2));
                _boardSquares[x, y] = square;
            }
        }

        // Hardcode how to assemble the board because it's chess; it doesn't change
        // Instantiate pieces.  Board X == Unity X; Board Y == Unity Z
        var whiteStandardRotationAngles = new Vector3(0, 180, 90);
        Quaternion whitestandardRotation = AnglesToQuaternion(whiteStandardRotationAngles);

        var blackStandardRotationAngles = new Vector3(0, 0, 90);
        Quaternion blackStandardRotation = AnglesToQuaternion(blackStandardRotationAngles);

        var whiteKnightRotationAngles = new Vector3(0, 180, 0);
        Quaternion whiteKnightRotation = AnglesToQuaternion(whiteKnightRotationAngles);

        var blackKnightRotationAngles = new Vector3(0, 0, 0);
        Quaternion blackKnightRotation = AnglesToQuaternion(blackKnightRotationAngles);

        var whiteKingRotationAngles = new Vector3(0, -90, 90);
        Quaternion whiteKingRotation = AnglesToQuaternion(whiteKingRotationAngles);

        var blackKingRotationAngles = new Vector3(0, -90, 90);
        Quaternion blackKingRotation = AnglesToQuaternion(blackKingRotationAngles);

        // Populate pawns
        for (int i = 0; i <= 7; i++)
        {
            // Assign positions in space
            var whitePosition = new Vector3(_boardSquares[i, 1].XCenter,
                                            (_boardHeight + _boardPosition.y),
                                            _boardSquares[i, 1].YCenter);

            var blackPosition = new Vector3(_boardSquares[i, 6].XCenter,
                                            (_boardHeight + _boardPosition.y),
                                            _boardSquares[i, 6].YCenter);

            // Instantiate GameObjects and pull their scripts
            GameObject newWhitePawn = Instantiate(whitePawn, whitePosition, whitestandardRotation);
            GameObject newBlackPawn = Instantiate(blackPawn, blackPosition, blackStandardRotation);
        }

        // For main game pieces, first create Vector3s of their positions in space
        // Rooks
        var queensWhiteRookPos = new Vector3(_boardSquares[0, 0].XCenter,
                                             (_boardHeight + _boardPosition.y),
                                             _boardSquares[0, 0].YCenter);
        var kingsWhiteRookPos = new Vector3(_boardSquares[7, 0].XCenter,
                                            (_boardHeight + _boardPosition.y),
                                            _boardSquares[7, 0].YCenter);
        var queensBlackRookPos = new Vector3(_boardSquares[0, 7].XCenter,
                                             (_boardHeight + _boardPosition.y),
                                             _boardSquares[0, 7].YCenter);
        var kingsBlackRookPos = new Vector3(_boardSquares[7, 7].XCenter,
                                            (_boardHeight + _boardPosition.y),
                                            _boardSquares[7, 7].YCenter);
        // Knights
        var queensWhiteKnightPos = new Vector3(_boardSquares[1, 0].XCenter,
                                               (_boardHeight + _boardPosition.y),
                                               _boardSquares[1, 0].YCenter);
        var kingsWhiteKnightPos = new Vector3(_boardSquares[6, 0].XCenter,
                                              (_boardHeight + _boardPosition.y),
                                              _boardSquares[6, 0].YCenter);
        var queensBlackKnightPos = new Vector3(_boardSquares[1, 7].XCenter,
                                               (_boardHeight + _boardPosition.y),
                                               _boardSquares[1, 7].YCenter);
        var kingsBlackKnightPos = new Vector3(_boardSquares[6, 7].XCenter,
                                              (_boardHeight + _boardPosition.y),
                                              _boardSquares[6, 7].YCenter);
        // Bishops
        var queensWhiteBishopPos = new Vector3(_boardSquares[2, 0].XCenter,
                                               (_boardHeight + _boardPosition.y),
                                               _boardSquares[2, 0].YCenter);
        var kingsWhiteBishopPos = new Vector3(_boardSquares[5, 0].XCenter,
                                              (_boardHeight + _boardPosition.y),
                                              _boardSquares[5, 0].YCenter);
        var queensBlackBishopPos = new Vector3(_boardSquares[2, 7].XCenter,
                                               (_boardHeight + _boardPosition.y),
                                               _boardSquares[2, 7].YCenter);
        var kingsBlackBishopPos = new Vector3(_boardSquares[5, 7].XCenter,
                                              (_boardHeight + _boardPosition.y),
                                              _boardSquares[5, 7].YCenter);
        // Queens
        var whiteQueenPos = new Vector3(_boardSquares[3, 0].XCenter,
                                        (_boardHeight + _boardPosition.y),
                                        _boardSquares[3, 0].YCenter);
        var blackQueenPos = new Vector3(_boardSquares[3, 7].XCenter,
                                        (_boardHeight + _boardPosition.y),
                                        _boardSquares[3, 7].YCenter);
        // Kings
        var whiteKingPos = new Vector3(_boardSquares[4, 0].XCenter,
                                       (_boardHeight + _boardPosition.y),
                                       _boardSquares[4, 0].YCenter);
        var blackKingPos = new Vector3(_boardSquares[4, 7].XCenter,
                                       (_boardHeight + _boardPosition.y),
                                       _boardSquares[4, 7].YCenter);

        // Then instantiate the objects at those positions
        // Rooks
        GameObject queensWhiteRook = Instantiate(whiteRook, queensWhiteRookPos, whitestandardRotation);
        GameObject kingsWhiteRook = Instantiate(whiteRook, kingsWhiteRookPos, whitestandardRotation);
        GameObject queensBlackRook = Instantiate(blackRook, queensBlackRookPos, blackStandardRotation);
        GameObject kingsBlackRook = Instantiate(blackRook, kingsBlackRookPos, blackStandardRotation);

        // Knights
        GameObject queensWhiteKnight = Instantiate(whiteKnight,
                                                   queensWhiteKnightPos,
                                                   whiteKnightRotation);
        GameObject kingsWhiteKnight = Instantiate(whiteKnight,
                                                  kingsWhiteKnightPos,
                                                  whiteKnightRotation);
        GameObject queensBlackKnight = Instantiate(blackKnight,
                                                   queensBlackKnightPos,
                                                   blackKnightRotation);
        GameObject kingsBlackKnight = Instantiate(blackKnight,
                                                  kingsBlackKnightPos,
                                                  blackKnightRotation);
        // Bishops 
        GameObject queensWhiteBishop = Instantiate(whiteBishop,
                                                   queensWhiteBishopPos,
                                                   whitestandardRotation);
        GameObject kingsWhiteBishop = Instantiate(whiteBishop,
                                                  kingsWhiteBishopPos,
                                                  whitestandardRotation);
        GameObject queensBlackBishop = Instantiate(blackBishop,
                                                   queensBlackBishopPos,
                                                   blackStandardRotation);
        GameObject kingsBlackBishop = Instantiate(blackBishop,
                                                  kingsBlackBishopPos,
                                                  blackStandardRotation);
        // Queens and Kings
        GameObject realWhiteQueen = Instantiate(whiteQueen, whiteQueenPos, whitestandardRotation);
        GameObject realBlackQueen = Instantiate(blackQueen, blackQueenPos, blackStandardRotation);

        GameObject realWhiteKing = Instantiate(whiteKing, whiteKingPos, whiteKingRotation);
        GameObject realBlackKing = Instantiate(blackKing, blackKingPos, blackKingRotation);

        // Once all pieces are there, get game controller and sync action squares once
        var gameController = gameObject.GetComponent<GameController>();
        gameController.ReSync();
    }

    private Quaternion AnglesToQuaternion(Vector3 vector3)
    {
        var quaternion = new Quaternion();
        quaternion.eulerAngles = vector3;
        return quaternion;
    }

    // Spawns squares which illustrate possible moves
    public void SpawnSquare(int x, int y, SquareType type)
    {
        var squareLocation = new Vector3(_boardSquares[x, y].XCenter,
                                         (_boardHeight + _boardPosition.y + ObjectSpacing),
                                         _boardSquares[x, y].YCenter);
        
        var squareRotationangles = new Vector3(90, 0, 0);
        var squareRotation = new Quaternion();
        squareRotation.eulerAngles = squareRotationangles;

        switch (type)
        {
            case SquareType.Attack:
                Instantiate(attackSquare, squareLocation, squareRotation);
                break;
            case SquareType.Check:
                Instantiate(checkSquare, squareLocation, squareRotation);
                break;
            default:
                Instantiate(moveSquare, squareLocation, squareRotation);
                break;
        }
    }

    // Gets the square associated with a point in Unity Space or throws an exception
    // if it's not on the board
    public void HideSquares()
    {
        GameObject[] squares = GameObject.FindGameObjectsWithTag("Square");

        foreach(GameObject obj in squares)
        {
            GameObject.Destroy(obj);
        }
    }

    public BoardCoords GetSquareForSpace(float xCoord, float yCoord)
    {
        // If it's outside the board, throw an exception
        if (xCoord < _boardSquares[0,0].XMin ||
            xCoord > _boardSquares[7,0].XMax ||
            yCoord < _boardSquares[0,0].YMin ||
            yCoord > _boardSquares[0,7].YMax)
        {
            throw new ArgumentException("Coordinates are not on the board.");
        }
        else
        {
            for (int x = 0; x <= 7; x++)
            {
                // if you've found the x, dive in and find the y, else continue
                if (xCoord >= _boardSquares[x,0].XMin &&
                    xCoord <= _boardSquares[x,0].XMax)
                {
                    for (int y = 0; y <= 7; y++)
                    {
                        // if you've found the y, assign the values, else continue
                        if (yCoord >= _boardSquares[x, y].YMin &&
                            yCoord <= _boardSquares[x, y].YMax)
                        {
                            var boardCoords = new BoardCoords(x,y);
                            return boardCoords;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        // If you fall out here, something really weird happened, because you're neither on nor
        // off the board.  Stop to pet Shroedinger's Cat, then troubleshoot.
        throw new Exception("Square coordinates could not be found.");
    }

    // Gets the Unity space coordinates of the center of a given square or throws an 
    // exception if it's not a valid square
    public SpaceCoords GetCenter(int xCoord, int yCoord)
    {
        if (xCoord >= 0 && xCoord <= 7 && yCoord >= 0 && yCoord <= 7)
        {
            var center = new SpaceCoords(_boardSquares[xCoord, yCoord].XCenter,
                                                 _boardSquares[xCoord, yCoord].YCenter);
            return center;
        }
        else
        {
            throw new ArgumentException("Argument must be a valid board square.");
        }
    }
    #endregion
}
