using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceControlledMoveHandler : MonoBehaviour
{
    public ChessPieceType ParsePiece(char id)
    {
        switch (id)
        {
            case 'P':
                return ChessPieceType.Pawn;
            case 'R':
                return ChessPieceType.Rook;
            case 'N':
                return ChessPieceType.Knight;
            case 'B':
                return ChessPieceType.Bishop;
            case 'Q':
                return ChessPieceType.Queen;
            case 'K':
                return ChessPieceType.King;
            default:
                return ChessPieceType.Pawn;
        }
    }

    public Vector2Int ParseCoords(string coords)
    {
        int x = coords[0] - 'a';
        int y = int.Parse(coords[1] + "");
        return new Vector2Int(x, y);
    }

    public bool CanMoveToPosition(GameObject piece, Vector2Int coords)
    {
        List<Vector2Int> locations = ChessMgr.instance.MovesForPiece(piece);
        for (int j = 0; j < locations.Count; j++)
        {
            if (locations[j].x == coords.x && locations[j].y == coords.y)
            {
                return true;
            }
        }
        return false;
    }

    public void MovePiece(string coords0, string coords1)
    {
        Vector2Int coordsDest = ParseCoords(coords1);

        if (char.IsUpper(coords0[0]) && coords0[1] == ' ')
        {
            ChessPieceType chessType = ParsePiece(coords0[0]);
            List<GameObject> possiblePieces = ChessMgr.instance.FindPiecesOfType(chessType);
            List<GameObject> movablePieces = new List<GameObject>();
            for (int i = 0; i < possiblePieces.Count; i++)
            {
                if (CanMoveToPosition(possiblePieces[i], coordsDest))
                {
                    movablePieces.Add(possiblePieces[i]);
                }
            }
            if (movablePieces.Count == 1)
            {
                ChessMgr.instance.Move(movablePieces[0], coordsDest);
            }
            else
            {
                Debug.LogWarning("No se puede realizar el movimiento especificado.");
            }
        }
        else
        {
            Vector2Int coordsOrigin = ParseCoords(coords0);
            GameObject piece = ChessMgr.instance.PieceAtGrid(coordsOrigin);
            if (piece == false)
            {
                Debug.LogWarning("No se encuentra la pieza en esa posición.");
            }
            else
            {
                ChessMgr.instance.Move(piece, coordsDest);
            }
        }
    }

    public void OnServerResponse(string response){
        string[] parts = response.Split(',');
        if(parts.Length != 3){
            Debug.LogError("Respuesta del servidor incorrecta");
        }
        if (parts[2][0] != '1') {
            Debug.LogWarning("No es tu turno");
        }
        MovePiece(parts[0], parts[1]);
    }
}
