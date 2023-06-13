using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType {King, Queen, Bishop, Knight, Rook, Pawn};

public abstract class ChessPiece : MonoBehaviour
{
    public ChessPieceType type;
    public int pieceValue;
    public string color;
    private GameObject claimPiece;
    private bool attackNow = false;

    protected Vector2Int[] RookDirections = {new Vector2Int(0,1), new Vector2Int(1, 0),
        new Vector2Int(0, -1), new Vector2Int(-1, 0)};
    protected Vector2Int[] BishopDirections = {new Vector2Int(1,1), new Vector2Int(1, -1),
        new Vector2Int(-1, -1), new Vector2Int(-1, 1)};

    public abstract List<Vector2Int> MoveLocations(Vector2Int gridPoint);

    // animations events

    public void StartWalk() {
        AudioSource[] audioSource = GetComponents<AudioSource>();
        audioSource[0].Play();
    }

    public void StopWalk() {
        AudioSource[] audioSource = GetComponents<AudioSource>();
        audioSource[0].Stop();
    }

    public void StartAttack() {
        AudioSource[] audioSource = GetComponents<AudioSource>();
        audioSource[0].Stop();
        audioSource[1].Play();
    }

    public void StartGetHurt() {
        AudioSource[] audioSource = GetComponents<AudioSource>();
        audioSource[2].Play();
    }

    public void CompleteGetHurt() {
        AudioSource[] audioSource = GetComponents<AudioSource>();
        audioSource[2].Stop();
    }

    public void CompleteAttack(){
        Animator anim = GetComponent<Animator>();
        anim.SetBool("attack",false);
        AudioSource[] audioSource = GetComponents<AudioSource>();
        audioSource[1].Stop();
    }

    public void CompleteDead() {
        Animator anim = GetComponent<Animator>();
        AudioSource[] audioSource = GetComponents<AudioSource>();
        audioSource[3].Play();
        Destroy(anim.gameObject);
    }

    public void SetClaimPiece(GameObject enemyPiece) {
        claimPiece = enemyPiece;
    }

    // on trigger enter 
    public void OnTriggerEnter(Collider other)
    {
        GameObject collidePiece = other.gameObject;
        if(claimPiece != null && GameObject.ReferenceEquals(claimPiece, collidePiece)) {
            SetAttackNow(); // attack now!
        }
    }

    public void SetAttackNow() {
        attackNow = !attackNow;
    }

    public bool GetAttackNow() {
        return attackNow;
    }

    

}


