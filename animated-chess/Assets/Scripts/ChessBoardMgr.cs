using UnityEngine;

public class ChessBoardMgr : MonoBehaviour
{
    public Material defaultMaterial;
    public Material selectedMaterial;
    

    public GameObject AddPiece(GameObject piece, int col, int row)
    {
        Vector2Int gridPoint = Grid.GridPoint(col, row);
        GameObject newPiece = Instantiate(piece, Grid.PointFromGrid(gridPoint), Quaternion.identity, gameObject.transform);
        return newPiece;
    }

    public void RemovePiece(GameObject piece)
    {
        Destroy(piece);
    }

    // without animation..
    public void MovePiece(GameObject piece, Vector2Int gridPoint)
    {
        piece.transform.position = Grid.PointFromGrid(gridPoint);
    }

}
