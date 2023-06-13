using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Victory : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterState(){
        ChessPlayer currentPlayer = ChessMgr.instance.currentPlayer;
        this.enabled = true;
        List<GameObject> pieces = currentPlayer.pieces;
        foreach (GameObject piece in pieces) {
            Animator anim = piece.GetComponent<Animator>();
            anim.SetBool("victory", true);
        }
    }
}
