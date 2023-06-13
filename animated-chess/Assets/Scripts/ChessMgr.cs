using System.Collections.Generic;
using UnityEngine;

public class ChessMgr : MonoBehaviour
{
    public static ChessMgr instance;

    public GameObject board;

    public GameObject whiteKing;
    public GameObject whiteQueen;
    public GameObject whiteBishop;
    public GameObject whiteKnight;
    public GameObject whiteRook;
    public GameObject whitePawn;

    public GameObject blackKing;
    public GameObject blackQueen;
    public GameObject blackBishop;
    public GameObject blackKnight;
    public GameObject blackRook;
    public GameObject blackPawn;

    private GameObject[,] pieces;
    private List<GameObject> movedPawns;
    private List<GameObject> fakeMovedPawns;

    private ChessPlayer white;
    private ChessPlayer black;
    public ChessPlayer currentPlayer;
    public ChessPlayer otherPlayer;

    public int playMode = 0;
    public int AILevel = 0;
    public int SecondAILevel = 0;

    // this should be in animate/move.cs though
    private Animator anim;

    void Awake()
    {
        instance = this;
    }

    public char GetCurrentPlayer(){
        if(currentPlayer == white)
        {
            return 'w';
        }
        return 'b';
    }

    void Start ()
    {
        pieces = new GameObject[8, 8];
        movedPawns = new List<GameObject>();
        fakeMovedPawns = new List<GameObject>();

        white = new ChessPlayer("White");
        black = new ChessPlayer("Black");

        currentPlayer = white;
        otherPlayer = black;

        //InitialSetup();
    }

    public void InitialSetup(int mode)
    {
        playMode = mode;
        AddPiece(whiteRook, white, 0, 0);
        AddPiece(whiteKnight, white, 1, 0);
        AddPiece(whiteBishop, white, 2, 0);
        
        AddPiece(whiteQueen, white, 3, 0);
        AddPiece(whiteKing, white, 4, 0);
        AddPiece(whiteBishop, white, 5, 0);
        AddPiece(whiteKnight, white, 6, 0);
        AddPiece(whiteRook, white, 7, 0);
        
        for (int i = 0; i < 8; i++)
        {   
            AddPiece(whitePawn, white, i, 1);
        }
        
        AddPiece(blackRook, black, 0, 7);
        AddPiece(blackKnight, black, 1, 7);
        AddPiece(blackBishop, black, 2, 7);
        
        AddPiece(blackQueen, black, 3, 7);
        AddPiece(blackKing, black, 4, 7);
        AddPiece(blackBishop, black, 5, 7);
        AddPiece(blackKnight, black, 6, 7);  
        AddPiece(blackRook, black, 7, 7);     
        
        
        for (int i = 0; i < 8; i++)
        {
            AddPiece(blackPawn, black, i, 6);
        }
        
    }

    public List<GameObject> FindPiecesOfType(ChessPieceType chessType)
    {
        List<GameObject> piecesList = new List<GameObject>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                GameObject p = pieces[i, j];
                if(p == null) {
                    continue;
                }
                ChessPiece ePiece = p.GetComponent<ChessPiece>(); // activate Chess Piece
                if(ePiece == null){
                    continue;
                }
                if (ePiece.type == chessType)
                {
                    piecesList.Add(p);
                }
            }
        }
        return piecesList;
    }
    
    public void AddPiece(GameObject prefab, ChessPlayer player, int col, int row)
    {
        GameObject pieceObject = board.GetComponent<ChessBoardMgr>().AddPiece(prefab, col, row);
        if (player.name == "Black")
            pieceObject.transform.Rotate(0.0f,180.0f,0.0f);
        player.pieces.Add(pieceObject); // differentiating white or black
        pieces[col, row] = pieceObject;
    }

    public void ClearBoard() {
        for (int i = 0; i < 8; i++) 
        {
            for (int j = 0; j < 8; j++)
            {
                GameObject p = pieces[i, j];
                if (p != null) 
                    Destroy(p);
            }
        }
        pieces = new GameObject[8, 8];
        movedPawns = new List<GameObject>();
        fakeMovedPawns = new List<GameObject>();
        white = new ChessPlayer("White");
        black = new ChessPlayer("Black");

        currentPlayer = white;
        otherPlayer = black;
    }

    public List<Vector2Int> MovesForPiece(GameObject pieceObject)
    {
        ChessPiece piece = pieceObject.GetComponent<ChessPiece>();
        Vector2Int gridPoint = GridForPiece(pieceObject);
        List<Vector2Int> locations = piece.MoveLocations(gridPoint);

        // filter out offboard locations
        locations.RemoveAll(gp => gp.x < 0 || gp.x > 7 || gp.y < 0 || gp.y > 7);

        // filter out locations with friendly piece
        locations.RemoveAll(gp => FriendlyPieceAt(gp));

        return locations;
    }
    
    public void Move(GameObject piece, Vector2Int gridPoint)
    {
        ChessPiece pieceComponent = piece.GetComponent<ChessPiece>();
        if (pieceComponent.type == ChessPieceType.Pawn && !HasPawnMoved(piece))
        {
            movedPawns.Add(piece);
        }

        Vector2Int startGridPoint = GridForPiece(piece);
        pieces[startGridPoint.x, startGridPoint.y] = null;
        pieces[gridPoint.x, gridPoint.y] = piece;
        //board.GetComponent<ChessBoardMgr>().MovePiece(piece, gridPoint);
    }

    public void FakeMove(GameObject piece, Vector2Int gridPoint) {
        ChessPiece pieceComponent = piece.GetComponent<ChessPiece>();
        if (pieceComponent.type == ChessPieceType.Pawn && !HasPawnMoved(piece) && !HasFakePawnMoved(piece))
        {
            fakeMovedPawns.Add(piece);
        }
        Vector2Int startGridPoint = GridForPiece(piece);
        pieces[startGridPoint.x, startGridPoint.y] = null;
        pieces[gridPoint.x, gridPoint.y] = piece;
        //board.MovePiece(piece, gridPoint);
    }

    public void UndoFakeMove(GameObject piece, Vector2Int gridPoint) {
        ChessPiece pieceComponent = piece.GetComponent<ChessPiece>();
        if (pieceComponent.type == ChessPieceType.Pawn && HasFakePawnMoved(piece))
        {
            fakeMovedPawns.Remove(piece);
        }
        Vector2Int startGridPoint = GridForPiece(piece);
        pieces[startGridPoint.x, startGridPoint.y] = null;
        pieces[gridPoint.x, gridPoint.y] = piece;
    }
    
    public bool HasPawnMoved(GameObject pawn)
    {
        return movedPawns.Contains(pawn);
    }

    public bool HasFakePawnMoved(GameObject pawn)
    {
        return fakeMovedPawns.Contains(pawn);
    }    

    public void FakePawnUnMoved(GameObject pawn) {
        if (HasFakePawnMoved(pawn))
            fakeMovedPawns.Remove(pawn);
    }

    public bool IsFakeMovedPawnsEmpty() {
        if (fakeMovedPawns.Count == 0) {
            return true;
        }
        return false; 
    }
    
    public GameObject CapturePieceAt(Vector2Int gridPoint)
    {
        GameObject pieceToCapture = PieceAtGrid(gridPoint);
        if (pieceToCapture.GetComponent<ChessPiece>().type == ChessPieceType.King)
        {   /* to un-comment for non-animation play
            Debug.Log(currentPlayer.name + " wins!");
            Destroy(board.GetComponent<TileSelector>());
            Destroy(board.GetComponent<MoveSelector>());
            Destroy(board.GetComponent<Move>());
            */
        }
        currentPlayer.capturedPieces.Add(pieceToCapture);
        pieces[gridPoint.x, gridPoint.y] = null;
        // Destroy(pieceToCapture);
        otherPlayer.pieces.Remove(pieceToCapture);
        return pieceToCapture;
    }

    public GameObject FakeCapturePieceAt(Vector2Int gridPoint)
    {
        GameObject pieceToCapture = PieceAtGrid(gridPoint);
        currentPlayer.capturedPieces.Add(pieceToCapture);
        pieces[gridPoint.x, gridPoint.y] = null;
        //Destroy(pieceToCapture);
        // added by me 
        otherPlayer.pieces.Remove(pieceToCapture);
        return pieceToCapture;
    }

    public void UndoFakeCapturePieceAt(GameObject capturedPiece, Vector2Int gridPoint)
    {
        if (capturedPiece != null) {
            currentPlayer.capturedPieces.Remove(capturedPiece);
            pieces[gridPoint.x, gridPoint.y] = capturedPiece;
            //Destroy(pieceToCapture);
            // added by me 
            otherPlayer.pieces.Add(capturedPiece);
        }
    }
    
    public bool DoesPieceBelongToCurrentPlayer(GameObject piece)
    {
        return currentPlayer.pieces.Contains(piece);
    }
    

    public GameObject PieceAtGrid(Vector2Int gridPoint)
    {
        if (gridPoint.x > 7 || gridPoint.y > 7 || gridPoint.x < 0 || gridPoint.y < 0)
        {
            return null;
        }
        return pieces[gridPoint.x, gridPoint.y];
    }
    
    public Vector2Int GridForPiece(GameObject piece)
    {
        for (int i = 0; i < 8; i++) 
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] == piece)
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    public bool FriendlyPieceAt(Vector2Int gridPoint)
    {
        GameObject piece = PieceAtGrid(gridPoint);

        if (piece == null) {
            return false;
        }

        if (otherPlayer.pieces.Contains(piece))
        {
            return false;
        }

        return true;
    }

    public void NextPlayer()
    {
        ChessPlayer tempPlayer = currentPlayer;
        currentPlayer = otherPlayer;
        otherPlayer = tempPlayer;
    }

    public string ReturnChessMap() {
        string str = "";
        for (int j = 7; j >= 0; j--) {
            for (int i = 0; i < 8; i++) {
                if (pieces[i,j] != null) {
                    str += " [ " + pieces[i,j].name + " ]";
                } else {
                    str += " [ " + i + "," + j + " ]";
                }
            }
            str += System.Environment.NewLine;
        }
        return str;
    }

}
