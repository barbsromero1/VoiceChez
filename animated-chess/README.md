# Animated-chess

Animated chess is a game from the popular ‘Harry Potter’ book series, which follows all the regular rules of chess. In Animated chess, the chess pieces take the form of humanoid characters, and walk from square to square when moved. Also, if a piece ‘claims’ another piece from the board, an attack animation plays and the opposing piece is destroyed. 

## Credits 
Characters and animations in this Animated Chess are downloaded from [Dungeon Mason](https://assetstore.unity.com/publishers/23554). 

Sound track and audios for animations and game play are downloaded from [Pixabay](https://pixabay.com/sound-effects/) and necessary clipping and re-arranging of sound effects are done with [Audio Cutter](https://mp3cut.net/).

This animated chess game is built with [Unity](https://unity.com/). 

## Environment 

Our characters are placed in the outdoor green battle field where they fight one another with the wits of chess rule. Like any other chess game, there are two players (blue and white) in this Animated Chess. 

Player (you) will assume the position of white. As always, white makes the first move.

## Board 

The board is built with the grid system and every grid (white or green square) has a column and row number assigned for the navigation of the chess piece movement and storing of the position of player’s chess piece. Note: The idea of the chess board grid is inspired by [Chess Game (with Unity) by Brian Broom](https://www.kodeco.com/5441-how-to-make-a-chess-game-with-unity). 

## Piece Selection and movement

When player hovers over the chess piece, it is highlighted with yellow square.
When player selects the chess piece, possible moves are shown with blue squares and possible captured moves are highlighted with red squares. 

For movement of the Chess Piece Character, Unity Software’s concept of NavMesh, NavMeshAgent and RayCast is utilized and programmed to play the respective animated moves.  Below is the code snippet of moving the character to mouse pointed location. 

## Attack and piece destruction 

Every chess piece character has a collider which is used to detect if moving chess piece is in the capacity of claimed piece. OnTriggerEnter function of Unity Software is used to start the attack animation on the chess piece along with the flinch animation (get hurt) on the enemy piece. Every chess piece character has their own set of animator as animated motions are different for different characters but they all follow the same pattern. 

## Game Play Mode
A simple Game Menu is built for game settings. 

Player can play against 
1.	another human opponent or 
2.	the computer (AI) in varying level of difficulty:
User can set the difficulty level of AI by moving the level slider (values from 0 to 10). 
At Level 0, computer makes a random move from all the possible moves.
At Level 1, computer makes a calculated move after thinking one move ahead. 
At Level 2, computer makes a calculated move after thinking two moves ahead. So on and so forth.

Additionally, user can also simulate the AI versus AI Game Play. 

## Logic for Random Moves
For random move, computer player follows the rules of chess but does not look ahead. Instead, it will pick a random piece and make a random move from all the possible move of the piece. 

## Calculated Moves 
For calculated moves (thinking ahead), minmax algorithm with alpha-beta pruning is implemented in the game. 

Notes:
1.	Game Pieces Shuffling: Pieces are shuffled first before evaluating for the best move. This is to prevent from the Knight always coming out first (At the game start, there are no best moves since the scoring is zero for any move. Therefore, the first move is always default to Knight coming out as Knight is the first chess piece (with possible moves) to get evaluated), followed by the rook moving right and left until an opponent’s piece is captured. This is obvious when AI is looking ahead 1 move only. As such, game peices are shuffled to default to a random piece with possible moves for the first move. For subsequent moves, AI will look for the optimal piece with chess move. 
 
2.	Computation Time: Given that there are between 10^111 and 10^123 positions in Chess, running minmax algorithm for the chess is resource intensive, even after AB pruning is incorporated. This Animated Chess can safely support up to 3 moves look ahead. User will experience a delay in the game move if anything greater than 3 move ahead is set for the AI level. Of course, this greatly depends on the processors of the computer from which the game is running. Due to the lack of time, I was not able to allocate the minmax algorithm to a separate thread and have it run in the background. 

### Evaluation Function 

At the end of every look ahead set (i.e. after looking ahead 2 moves for AI level = 2 or after looking ahead 3 moves for AI level = 3), computer will evaluate the board to calculate the score. Based on the score, it will choose the best move. 

For the score calculation, a value is assigned to the chess piece. Total score of the player is the sum of the values of the player’s chess piece on the board subtract by the values from the opponent’s chess pieces. 

This Animated Chess Game utilizes [Larry Kaufman’s suggested values for middle game](https://www.chessfornovices.com/chesspiecevalues.html), multiplied by 100 so as there are no floating numbers. 

| Chess Piece  | Pawn | Rook | Knight | Bishop | Queen | King |
| ------------ | -----| -----| -------| -------| ------| -----| 
| Value        | 100  | 525  |  350   |  350   | 1000  | 1000000 |


This function can be further improved by incorporating the position of the chess piece in calculating the score. For example, Bishop in the middle of the chess board could be valued more. Other considerations such as a penalty for losing two bishops would also enhance the evaluation function. 


## Improvements Consideration

- [ ] This animated chess game can be further improved by looking into the weapon collision to the enemy body with Unity Software and perhaps, display the ragdoll effects for the enemy destruction. 
- [ ] At present, when moving chess piece has neighbouring chess pieces, it's sort of colliding with them and sometimes, moving the neighbouring chess pieces unintentionally. Perhaps, it could be animated for the neighbouring chess pieces to give way for moving chess piece.
- [ ] Minmax algorithm could be allocated to a separate thread to handle the computation. 
- [ ] Evaluation function can be fine-tuned to incorporate the position of the chess piece and penalties for the loss pairs (for example: having an incur on the total score for the loss of two bishops).
- [ ] At present, characters are facing the direction of the mouse pointer. This could be re-assessed to always face forwards to the enemy positions on the chess board. It would appease the visual of the game. 


## Super Note
Please pardon the messy code, I completed this project alone in 3 weeks, including finding animations, audios, learning min-max algorithm, unity physics while at the same time, working on other projects. I am just grateful that I have knowledge of how to play Chess before building this game. 











