using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Move : MonoBehaviour
{
    public float chessPieceSpeed;
    private Vector3 moveLocation; 
    private GameObject movingPiece;
    private ChessPiece cPiece;
    private GameObject enemyPiece;
    private ChessPiece ePiece;
    private CharacterController cc;
    private NavMeshAgent nma; 
    private Animator anim;
    private Animator enemyAnim;
    private CapsuleCollider capsuleCollider;
    private bool found = false;
    private bool confetti = false; 

    void Start(){
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsTarget();
    }

    
    // Character Controller Move 
    public void EnterState(GameObject piece, Vector2Int gridPoint){
        movingPiece = piece;
        moveLocation = Grid.PointFromGrid(gridPoint);
        cc = movingPiece.GetComponent<CharacterController>();
        anim = piece.GetComponent<Animator>();
        this.enabled = true;
    }

    // Nav Mesh Agent Move
    public void EnterState(GameObject piece, Vector3 targetPoint){
        movingPiece = piece;
        moveLocation = targetPoint;
        nma = piece.GetComponent<NavMeshAgent>();
        anim = piece.GetComponent<Animator>();
        this.enabled = true;
    }

    // Nav Mesh Agent Move + Claimed Piece
    public void EnterState(GameObject piece, Vector3 targetPoint, GameObject enemy) {
        movingPiece = piece; // which piece to animate 
        moveLocation = targetPoint; // where to go 
        enemyPiece = enemy; // which piece to capture 
        confetti = false; // set confettie to false first
        nma = piece.GetComponent<NavMeshAgent>(); // Get Nav Mesh Agent 
        anim = piece.GetComponent<Animator>(); // Get Animator 
        capsuleCollider = piece.GetComponent<CapsuleCollider>(); // Get the collider
        if (capsuleCollider == null) {
            SphereCollider scollider = piece.GetComponent<SphereCollider>();
            scollider.isTrigger = true;
        } else {
            capsuleCollider.isTrigger = true;
        }


        cPiece = movingPiece.GetComponent<ChessPiece>(); // activate Chess Piece 
        cPiece.SetClaimPiece(enemyPiece); // Set the capture piece 

        ePiece = enemy.GetComponent<ChessPiece>(); // activate Chess Piece 
        enemyAnim = enemy.GetComponent<Animator>();

        this.enabled = true;
    }

    // animations (move, attack, get hit, die) are done
    private void ExitState() {
        //Debug.Log(ChessMgr.instance.ReturnChessMap());
        if (capsuleCollider == null) {
            SphereCollider scollider = movingPiece.GetComponent<SphereCollider>();
            if (scollider != null) {
                scollider.isTrigger = false;
            }
        } else {
            capsuleCollider.isTrigger = false;
        }
        this.enabled = false;
        movingPiece = null;
        enemyPiece = null;
        found = false;
        ChessMgr.instance.NextPlayer();
        TileSelect selector = GetComponent<TileSelect>();
        selector.EnterState();
    }

    // animations (victory)
    private void ExitState(bool confetti) {
        this.enabled = false;
        Victory victory = GetComponent<Victory>();
        victory.EnterState();
    }

    /* working with cc 
    private void MoveTowardsTarget(Vector3 target) {
        Vector3 offset = target - movingPiece.transform.position; 
        offset.y = 0;
        if (offset.magnitude > 0.1f) {
            offset = offset.normalized * chessPieceSpeed;
            cc.Move(offset * Time.deltaTime);
            anim.SetBool("walk",true);
        } else {
            movingPiece.transform.position= target;
            anim.SetBool("walk",false);
            ExitState();
        }        
    }
    */

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
            if(movablePieces.Count == 1)
            {
                ChessMgr.instance.Move(movablePieces[0], coordsDest);
            }else{
                Debug.Log("No se puede realizar el movimiento especificado.");
            }
        }
        else {
            Vector2Int coordsOrigin = ParseCoords(coords0);
            GameObject piece = ChessMgr.instance.PieceAtGrid(coordsOrigin);
            if( piece == false)
            {
                Debug.Log("No se encuentra la pieza en esa posición.");
            }
            else
            {
                ChessMgr.instance.Move(piece, coordsDest);
            }
        }

    }

    private void MoveTowardsTarget() {
        anim.SetFloat("speed", nma.velocity.magnitude);
        
        // need to re-do this with checking of claimed piece  = hitInfo
        // check if game piece is in a close vicinity to the claimed piece 

        /* working code 
        if (!found && enemyPiece != null) {
            RaycastHit hitInfo;
            Vector3 rayCastPos=movingPiece.transform.position;
            rayCastPos.y+=0.2f; //Look up a litte bit 
            Debug.DrawRay(rayCastPos, movingPiece.transform.forward* 1000, Color.white);
            if (Physics.Raycast(rayCastPos, movingPiece.transform.forward, out hitInfo, 0.5f)) {
                GameObject foundPiece = hitInfo.transform.gameObject;
                Debug.Log("HitInfo: " + foundPiece); 
                if(GameObject.ReferenceEquals(enemyPiece, foundPiece)) {
                    found = true;       
                    StartCoroutine(Attack());                
                }
            } 
        }
        */

        /* testing code with onTriggerEnter */



        if (enemyPiece != null && cPiece.GetAttackNow()) {
            found = true;
        }

        if (enemyPiece !=null && found) {
            StartCoroutine(Attack()); 
        }
   
        if (cPiece != null && enemyPiece == null) {
            cPiece.CompleteAttack();
            if (confetti) { // The king is captured!!
                anim.SetFloat("speed",0);
                ExitState(confetti);
            }
        }

        // The King is captured!!! 
        if (ePiece != null && ePiece.type == ChessPieceType.King) {
            confetti = true;
        }

        
        
        nma.destination = moveLocation;    
        Vector3 offset = moveLocation - movingPiece.transform.position; 
        offset.y = 0;
        if (offset.magnitude < 0.1f) {
            movingPiece.transform.position = moveLocation;
            anim.SetFloat("speed", 0);
            ExitState();
        }
    }

    IEnumerator Attack() {
        anim.SetFloat("speed", 0);
        anim.SetBool("attack", true);   
        while (anim.GetBool("attack") && enemyPiece != null) {
            enemyAnim.SetBool("getHit", true);
            yield return null;
        }
    }


    
}
