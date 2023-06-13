using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelect : MonoBehaviour
{
    public GameObject tileHighlightPrefab;
    private GameObject tileHighlight;

    void Start ()
    {
        Vector2Int gridPoint = Grid.GridPoint(0, 0);
        Vector3 point = Grid.PointFromGrid(gridPoint);
        tileHighlight = Instantiate(tileHighlightPrefab, point, Quaternion.identity, gameObject.transform);
        tileHighlight.SetActive(false);
    }

    void Update ()
    {
        switch (ChessMgr.instance.playMode) {
            case 0: // Player vs AI
                ChessPlayer currentPlayer = ChessMgr.instance.currentPlayer;
                if (currentPlayer.name == "Black") {
                    var results = AutoMode(ChessMgr.instance.AILevel);
                    GameObject bestMovePiece = results.bestMovePiece;
                    Vector2Int bestMoveLoc = results.bestMoveLoc;
                    ExitState(bestMovePiece, bestMoveLoc);
                }  else {
                    ManualMode();
                }
                break;
            case 1: // Two Players
                ManualMode();
                break;
            case 2: // AI vs AI
                int depth = 0;
                if (ChessMgr.instance.currentPlayer.name == "Black") {
                    depth = ChessMgr.instance.AILevel;
                } else {
                    depth = ChessMgr.instance.SecondAILevel;
                }
                var results2 = AutoMode(depth);
                GameObject bestMovePiece2 = results2.bestMovePiece;
                Vector2Int bestMoveLoc2 = results2.bestMoveLoc;
                ExitState(bestMovePiece2, bestMoveLoc2);
                break;
        }
    }

    public (int score, GameObject bestMovePiece, Vector2Int bestMoveLoc) AutoMode(int depth) {
        if (depth == 0) {
            return RandomMove();
        }
        ChessPlayer currentPlayer = ChessMgr.instance.currentPlayer;
        return MinimaxWithABPruning(depth, currentPlayer.name);
    }

    public void ManualMode() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            Vector3 point = hit.point;
            Vector2Int gridPoint = Grid.GridFromPoint(point);

            tileHighlight.SetActive(true);
            tileHighlight.transform.position = Grid.PointFromGrid(gridPoint);
            if (Input.GetMouseButtonDown(0)) {
                GameObject selectedPiece = ChessMgr.instance.PieceAtGrid(gridPoint);
                if (ChessMgr.instance.DoesPieceBelongToCurrentPlayer(selectedPiece))
                {
                    ExitState(selectedPiece);
                }
                
            }
        } else {
            tileHighlight.SetActive(false);
        }        
    }

    public void EnterState()
    {
        enabled = true;
    }

    // AI Move straight to Animation 
    private void ExitState(GameObject movingPiece, Vector2Int moveLoc) {
        this.enabled = false;
        if (ChessMgr.instance.PieceAtGrid(moveLoc) == null) {
            ChessMgr.instance.Move(movingPiece, moveLoc);
            Move move = GetComponent<Move>();
            move.EnterState(movingPiece, Grid.PointFromGrid(moveLoc));
        } else {
            GameObject pieceToCapture = ChessMgr.instance.CapturePieceAt(moveLoc);
            ChessMgr.instance.Move(movingPiece, moveLoc);
            Move move = GetComponent<Move>();
            move.EnterState(movingPiece, Grid.PointFromGrid(moveLoc), pieceToCapture);

        }
    }

    private void ExitState(GameObject movingPiece)
    {
        this.enabled = false;
        tileHighlight.SetActive(false);
        MoveSelect move = GetComponent<MoveSelect>();
        move.EnterState(movingPiece);
    }

    // Random Piece & move for AI 
    private (int score, GameObject bestMovePiece, Vector2Int bestMoveLoc) RandomMove() {
        System.Random rnd = new System.Random();
        List<Vector2Int> moveLocations;
        GameObject rndPiece; //randomly choose a piece that can move
        ChessPlayer currentPlayer = ChessMgr.instance.currentPlayer;
        do {
            rndPiece = currentPlayer.pieces[rnd.Next(currentPlayer.pieces.Count)];
            moveLocations = ChessMgr.instance.MovesForPiece(rndPiece);
            //Debug.Log("Random Piece Name: " + rndPiece.name);
            //Debug.Log("Random Move Location: " + moveLocations.Count);
        } while (moveLocations.Count == 0);
        // randomly choose a location to move
        Vector2Int rndMoveLoc = moveLocations[rnd.Next(moveLocations.Count)];
        return (0, rndPiece, rndMoveLoc); 
    }

    private (int bestScore, GameObject bestMovePiece, Vector2Int bestMoveLoc) LookOneMoveAhead(string playerColor) {
        // looks all the possible moves for this turn 
        List<GameObject> pieces = ChessMgr.instance.currentPlayer.pieces;
        pieces = ShuffleList(pieces);
        int bestMoveScore = int.MinValue;
        GameObject bestMovePiece = null;
        Vector2Int bestMoveLoc = new Vector2Int(0,0);

        foreach (GameObject piece in pieces) {// Scenario play for each piece
            Vector2Int origGridPoint = ChessMgr.instance.GridForPiece(piece);
            // possible moves from piece 
            List<Vector2Int> moveLocations = ChessMgr.instance.MovesForPiece(piece);
            foreach (Vector2Int location in moveLocations) {
                // play out the scenario
                GameObject capturedPiece = ScenarioPlay(piece, location);
                // evaluate the position after each move scenario
                int currentScore = EvaluateBoard(playerColor);                 
                if (currentScore > bestMoveScore) {
                    bestMoveScore = currentScore; 
                    bestMovePiece = piece;
                    bestMoveLoc = location;
                }
                // revert the scenario 
                ScenarioRevert(piece, capturedPiece, origGridPoint, location);
            }
        }
        // pick the move which leads to maximum gain 
        return (bestMoveScore, bestMovePiece, bestMoveLoc); 
    }

    private (int bestScore, GameObject bestMovePiece, Vector2Int bestMoveLoc) Minimax(int depth, string playerColor, bool isMaximizingPlayer = true) {
        // base case 
        if (depth == 0) {
            // always evaluate for our player
            return (EvaluateBoard(playerColor), null, new Vector2Int(0,0));
        }
        // recursive case
        List<GameObject> pieces = new List<GameObject> (ChessMgr.instance.currentPlayer.pieces);
        pieces = ShuffleList(pieces);
        int bestMoveScore = int.MinValue; 
        if (!isMaximizingPlayer)
            bestMoveScore = int.MaxValue; 
        GameObject bestMovePiece = null;
        Vector2Int bestMoveLoc = new Vector2Int(0,0);

        foreach (GameObject piece in pieces) {
            Vector2Int origGridPoint = ChessMgr.instance.GridForPiece(piece);
            List<Vector2Int> moveLocations = new List<Vector2Int> (ChessMgr.instance.MovesForPiece(piece));
            foreach (Vector2Int location in moveLocations) {
                GameObject capturedPiece = ScenarioPlay(piece, location);
                // we evaluate the position if it's a base case
                // otherwise we switch player 
                int currentScore = Minimax(depth-1, playerColor, !isMaximizingPlayer).bestScore;
                if (isMaximizingPlayer) { // this will be processed only after the evaluation... at which point player is already switched back
                    if (currentScore > bestMoveScore) {
                        bestMoveScore = currentScore;
                        bestMovePiece = piece; //original piece 
                        bestMoveLoc = location; //original location
                    }
                } else {
                    if (currentScore < bestMoveScore) {
                        bestMoveScore = currentScore;
                        bestMovePiece = piece;  //original piece 
                        bestMoveLoc = location; //original location
                    }
                }
                ScenarioRevert(piece, capturedPiece, origGridPoint, location);
            }
        }
        return (bestMoveScore, bestMovePiece, bestMoveLoc); //give the minimax result
    }

    private (int bestScore, GameObject bestMovePiece, Vector2Int bestMoveLoc) MinimaxWithABPruning(int depth, string playerColor, bool isMaximizingPlayer = true, int alpha = int.MinValue, int beta = int.MaxValue) {
        if (depth == 0) {         // base case 
            return (EvaluateBoard(playerColor), null, new Vector2Int(0,0)); // always evaluate for our player
        }
        // recurisve case
        List<GameObject> pieces = new List<GameObject> (ChessMgr.instance.currentPlayer.pieces);
        pieces = ShuffleList(pieces);
        int bestMoveScore = int.MinValue; 
        if (!isMaximizingPlayer)
            bestMoveScore = int.MaxValue; 
        GameObject bestMovePiece = null;
        Vector2Int bestMoveLoc = new Vector2Int(0,0);

        foreach (GameObject piece in pieces) {
            Vector2Int origGridPoint = ChessMgr.instance.GridForPiece(piece);
            List<Vector2Int> moveLocations = new List<Vector2Int> (ChessMgr.instance.MovesForPiece(piece));
            foreach (Vector2Int location in moveLocations) {
                GameObject capturedPiece = ScenarioPlay(piece, location);
                // we evaluate the position if it's a base case, otherwise we switch to another player perspective
                int currentScore = MinimaxWithABPruning(depth-1, playerColor, !isMaximizingPlayer, alpha, beta).bestScore;
                if (isMaximizingPlayer) {
                    if (currentScore > bestMoveScore) {
                        bestMoveScore = currentScore;
                        bestMovePiece = piece; //original piece 
                        bestMoveLoc = location; //original location
                    }
                    if (currentScore > alpha)
                        alpha = currentScore;
                } else {
                    if (currentScore < bestMoveScore) {
                        bestMoveScore = currentScore;
                        bestMovePiece = piece;  //original piece 
                        bestMoveLoc = location; //original location
                    }
                    if (currentScore < beta)
                        beta = currentScore;
                }
                ScenarioRevert(piece, capturedPiece, origGridPoint, location);
                // check for alpha beta pruning 
                if (beta <= alpha) 
                    break;
            }
        }
        return (bestMoveScore, bestMovePiece, bestMoveLoc); //give the minimax result
    }

    // Relative to the player for whom we want to evaluate the position, any of their own pieces will add to player's total score, and their opponent’s pieces will subtract from player's total score
    private int EvaluateBoard(string playerColor) {
        List<GameObject> pieces = null;
        List<GameObject> oPieces = null;
        if (playerColor != ChessMgr.instance.currentPlayer.name) {
            pieces = ChessMgr.instance.otherPlayer.pieces;
            oPieces = ChessMgr.instance.currentPlayer.pieces; 
        } else {
            pieces = ChessMgr.instance.currentPlayer.pieces; 
            oPieces = ChessMgr.instance.otherPlayer.pieces; 
        }
        int playerScore = 0;
        foreach (GameObject piece in pieces) {
            ChessPiece pieceC = piece.GetComponent<ChessPiece>();
            playerScore += pieceC.pieceValue;
        }
        foreach (GameObject piece in oPieces) {
            ChessPiece pieceC = piece.GetComponent<ChessPiece>();
            playerScore -= pieceC.pieceValue;
        }
        return playerScore;
    }

    // returns the captured piece if any
    private GameObject ScenarioPlay(GameObject movePiece, Vector2Int moveLoc) {
        GameObject capturedPiece = null;
        if (ChessMgr.instance.PieceAtGrid(moveLoc) != null) {
            capturedPiece = ChessMgr.instance.FakeCapturePieceAt(moveLoc);
        }
        ChessMgr.instance.FakeMove(movePiece, moveLoc);
        ChessMgr.instance.NextPlayer(); //next player's turn
        return capturedPiece;
    }
    
    private void ScenarioRevert(GameObject movePiece, GameObject capturedPiece, Vector2Int origLoc, Vector2Int moveLoc) {
        ChessMgr.instance.NextPlayer(); //previous's player
        ChessMgr.instance.UndoFakeMove(movePiece, origLoc); //revert the movement
        ChessMgr.instance.UndoFakeCapturePieceAt(capturedPiece, moveLoc); //reactivate the capture
    }

    private void MakeMove(GameObject movePiece, Vector2Int moveLoc) {
        if (ChessMgr.instance.PieceAtGrid(moveLoc) == null) {
            ChessMgr.instance.Move(movePiece, moveLoc);
        } else {
            ChessMgr.instance.CapturePieceAt(moveLoc);
            ChessMgr.instance.Move(movePiece, moveLoc);
        }
        // next player's turn 
        ChessMgr.instance.NextPlayer();
    }

    private List<GameObject> ShuffleList(List<GameObject>list)  
    {  
        System.Random rnd = new System.Random();
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rnd.Next(n + 1);  
            GameObject value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
        return list;
    }

}
