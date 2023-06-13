using System.Collections.Generic;
using UnityEngine;

public class PawnMove : ChessPiece
{
    
    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
    {
        
        List<Vector2Int> locations = new List<Vector2Int>();
        
        int forwardDirection = 1;
        if (this.color == "Black")
            forwardDirection = -1;

        Vector2Int forwardOne = new Vector2Int(gridPoint.x, gridPoint.y + forwardDirection);
        if (ChessMgr.instance.PieceAtGrid(forwardOne) == false)
        {
            locations.Add(forwardOne);
        }

        Vector2Int forwardTwo = new Vector2Int(gridPoint.x, gridPoint.y + 2 * forwardDirection);
        
        if (ChessMgr.instance.HasPawnMoved(gameObject) == false && ChessMgr.instance.PieceAtGrid(forwardTwo) == false && ChessMgr.instance.PieceAtGrid(forwardOne) == false)
        {
            if (ChessMgr.instance.IsFakeMovedPawnsEmpty() || ChessMgr.instance.HasFakePawnMoved(gameObject) == false)
                locations.Add(forwardTwo);
        }

        // When there are pieces at one step left and right of the pawn
        Vector2Int forwardRight = new Vector2Int(gridPoint.x + 1, gridPoint.y + forwardDirection);
        if (ChessMgr.instance.PieceAtGrid(forwardRight))
        {
            locations.Add(forwardRight);
        }

        Vector2Int forwardLeft = new Vector2Int(gridPoint.x - 1, gridPoint.y + forwardDirection);
        if (ChessMgr.instance.PieceAtGrid(forwardLeft))
        {
            locations.Add(forwardLeft);
        }
        
        return locations;
        
    }

}
