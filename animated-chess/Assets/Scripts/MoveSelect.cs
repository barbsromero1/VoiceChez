using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelect : MonoBehaviour
{
    public GameObject moveLocationPrefab;
    public GameObject tileHighlightPrefab;
    public GameObject attackLocationPrefab;
    

    private GameObject tileHighlight;
    private GameObject movingPiece;
    private List<Vector2Int> moveLocations;
    private List<GameObject> locationHighlights;

    void Start ()
    {
        //this.enabled = false;
        tileHighlight = Instantiate(tileHighlightPrefab, Grid.PointFromGrid(new Vector2Int(0, 0)),
            Quaternion.identity, gameObject.transform);
        tileHighlight.SetActive(false);
    }

    void Update ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)){
            Vector3 point = hit.point;
            Vector2Int gridPoint = Grid.GridFromPoint(point);
            tileHighlight.SetActive(true);
            tileHighlight.transform.position = Grid.PointFromGrid(gridPoint);
            if (Input.GetMouseButtonDown(0))
            {
                // Reference Point 2: check for valid move location
                if (!moveLocations.Contains(gridPoint)) {
                    CancelMove();
                    return;
                } else {
                    if (ChessMgr.instance.PieceAtGrid(gridPoint) == null) {
                        ChessMgr.instance.Move(movingPiece, gridPoint);
                        //ExitState(movingPiece, gridPoint); //with cc
                        ExitState(movingPiece, Grid.PointFromGrid(gridPoint)); //with nma
                    } else {
                        Debug.Log("in Move Select: ");
                        GameObject pieceToCapture = ChessMgr.instance.CapturePieceAt(gridPoint);
                        Debug.Log("Claim Piece: " + pieceToCapture.name);
                        ChessMgr.instance.Move(movingPiece, gridPoint);
                        ExitState(movingPiece, Grid.PointFromGrid(gridPoint),pieceToCapture); //to handle animation
                    }
                }
                // Reference Point 3: capture enemy piece here later
                ExitState();
            }
        } else{
            //Debug.Log("deactivate");
            tileHighlight.SetActive(false);
        }
    }

    
    private void CancelMove()
    {
        this.enabled = false;

        foreach (GameObject highlight in locationHighlights)
        {
            Destroy(highlight);
        }

        //ChessMgr.instance.DeselectPiece(movingPiece);
        TileSelect selector = GetComponent<TileSelect>();
        selector.EnterState();
    }
    
    // enter from TileSelection for selected piece 
    public void EnterState(GameObject piece)
    {
        movingPiece = piece;
        this.enabled = true;
        
        moveLocations = ChessMgr.instance.MovesForPiece(movingPiece);
        locationHighlights = new List<GameObject>();
        
        if (moveLocations.Count == 0)
        {
            CancelMove();
        }

        foreach (Vector2Int loc in moveLocations)
        {
            GameObject highlight;
            if (ChessMgr.instance.PieceAtGrid(loc))
            {
                highlight = Instantiate(attackLocationPrefab, Grid.PointFromGrid(loc), Quaternion.identity, gameObject.transform);
            }
            else
            {
                highlight = Instantiate(moveLocationPrefab, Grid.PointFromGrid(loc), Quaternion.identity, gameObject.transform);
            }
            locationHighlights.Add(highlight);
        }
        
    }

    private void ExitState()
    {
        this.enabled = false;
        TileSelect selector = GetComponent<TileSelect>();
        tileHighlight.SetActive(false);
        //ChessMgr.instance.DeselectPiece(movingPiece);
        movingPiece = null;
        //ChessMgr.instance.NextPlayer();
        selector.EnterState();
        foreach (GameObject highlight in locationHighlights)
        {
            Destroy(highlight);
        }
    }

    private void ExitState(GameObject movingPiece, Vector2Int gridPoint) {
        this.enabled = false;
        Move move = GetComponent<Move>();
        move.EnterState(movingPiece, gridPoint);
    }

    private void ExitState(GameObject movingPiece, Vector3 point) {
        this.enabled = false;
        Move move = GetComponent<Move>();
        move.EnterState(movingPiece, point);
    }

    private void ExitState(GameObject movingPiece, Vector3 point, GameObject enemyPiece) {
        this.enabled = false; 
        Move move = GetComponent<Move>();
        move.EnterState(movingPiece, point, enemyPiece);
    }
}
