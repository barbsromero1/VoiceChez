using System.Collections.Generic;
using UnityEngine;

public class ChessPlayer
{
    public List<GameObject> pieces;
    public List<GameObject> capturedPieces;

    public string name;
    public int forward;

    public ChessPlayer(string name)
    {
        this.name = name;
        pieces = new List<GameObject>();
        capturedPieces = new List<GameObject>();
    }
}
