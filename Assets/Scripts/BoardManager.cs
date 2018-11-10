using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {

    // Public Variables for the Inspector
    public PlaceScript placement;
    public AttackerScript playerAttacker;

    // Constants
    private const int BOARD_WIDTH = 6;
    private const int BOARD_HEIGHT = 6;
    private const float boardSpacing = 1f; // the distance between the tiles
    private const float pieceOffset = 0.1f; // height above the tile that pieces rest
    private const int max_moves = 2;

    private readonly int[,] PLAYER_ONE_COORDS = { { 0, 0 }, { 0, 1 }, { 0, 2 } };
    private readonly int[,] PLAYER_TWO_COORDS = { { 5, 3 }, { 5, 4 }, { 5, 5 } };

    // Private Variables
    private PlaceScript[,] boardArray;
    private List<PlayerHandHandler> handHandlers;
    private int turn = 0;
    private int curr_moves = 2;
	private int curr_player = 1;

    /*
     * Setting up board layout.
     */
    void Awake () {
		boardArray = new PlaceScript[BOARD_WIDTH,BOARD_HEIGHT];

        //B1 - first 3, B2 - next 3, B - remaining 4
        int[,] layoutV1 = {{0,0},{1,0},{2,2},{3,3},{4,5},{5,5},{4,1},{3,2},{2,3},{1,4}};
        SetupBoard();
        SetupPlayerHands();
        BoardLayout(layoutV1);
    }

    // Sets up empty places on the board.
    private void SetupBoard()
    {
        for (int x = 0; x < BOARD_HEIGHT; x++)
        {
            for (int z = 0; z < BOARD_WIDTH; z++)
            {
                PlaceScript placeTile = CreatePlaceTile(x, z, 0, "Place");
                placeTile.transform.parent = transform.GetChild(0).transform;
                placeTile.xPos = x;
                placeTile.zPos = z;
                boardArray[x, z] = placeTile;
            }
        }
    }

    // Sets up the positions of the players hands.
    private void SetupPlayerHands()
    {
        handHandlers = new List<PlayerHandHandler>
        {
            gameObject.AddComponent<PlayerHandHandler>(),
            gameObject.AddComponent<PlayerHandHandler>()
        };

        handHandlers[0].PopulatePlayerHand(PLAYER_ONE_COORDS, placement, -2, boardSpacing, "PLAYER 1 HAND", 1);
        handHandlers[0].FillHand();
        handHandlers[1].PopulatePlayerHand(PLAYER_TWO_COORDS, placement, 2, boardSpacing, "PLAYER 2 HAND", 2);
        handHandlers[1].FillHand();
    }

    //applies a preset layout to the board
    private void BoardLayout(int[,] layout)
    {
        for (int i = 0; i < layout.GetLength(0); i++)
        {
            foreach (PlaceScript cell in boardArray)
            {
                if (cell.zPos == layout[i, 0] && cell.xPos == layout[i, 1])
                {
                    if (i < 3)
                    {
                        cell.SetState("BASE1");
                        cell.team = 1;
                    }
         
                    if (i > 2)
                    {
                        cell.SetState("BASE2");
                        cell.team = 2;
                    } 
                    if (i > 5)
                    {
                        cell.SetState("BLOCK");
                        cell.team = 0;
                    }


                    if (cell.GetState("BASE1"))
                        cell.SetAttacker(CreateAttacker(cell.team, cell.xPos, cell.zPos, "Player 1 Attacker"));
                    if (cell.GetState("BASE2"))
                        cell.SetAttacker(CreateAttacker(cell.team, cell.xPos, cell.zPos, "Player 2 Attacker"));
                }

            }
        }
    }

    private PlaceScript CreatePlaceTile(int x, int z, int offset, string name)
    {
        PlaceScript placeTile = Instantiate(placement, new Vector3(x + offset * boardSpacing, 0, z * boardSpacing), Quaternion.identity);
        placeTile.SetState("EMPTY");
        placeTile.name = name + ": " + z + ", " + x;
        return placeTile;
    }

    // Creates an attacker and assigns a team to it
    private AttackerScript CreateAttacker(int team, int x, int z, string name)
    {
        AttackerScript attacker = Instantiate(playerAttacker, new Vector3(x, pieceOffset, z), Quaternion.identity);
        attacker.team = team;
        attacker.name = name;
        return attacker;
    }

     /*
     * Public methods called from other classes.
     */
    // Swap player when the moves get below 1. Then reset the moves to max.
    public void SwitchPlayer()
	{
		if(curr_moves < 1)
		{
			turn++;
			if(turn % 2 == 0)
				curr_player = 1;
			else
				curr_player = 2;
			curr_moves = max_moves;
		}
	}

    // Check to see if the attacker is on the opposing player's base. Increase the counter.
    public bool CheckForWinner(int playerID)
    {
        int counter = 0;
        foreach(PlaceScript cell in boardArray)
        {
            if (cell.team != playerID && cell.team != 0)
            {
                if(cell.GetAttacker() && cell.GetAttacker().team == playerID)
                {
                    counter++;
                }
            }
        }
        if(counter > 2)
        {
            return true;
        }
        return false;
    }

    // Move Attacker from origin tile to target tile.
    public void SetAttacker(PlaceScript origin, PlaceScript target)
    {
        target.SetAttacker(origin.GetAttacker());
        origin.GetAttacker().transform.position = new Vector3(target.transform.position.x, pieceOffset, target.transform.position.z);
        origin.GetAttacker().ToggleSelect();
        origin.SetAttacker(null);
    }
    
    // Does checks to see if the value to be subtracted from the moves is correct.
    public bool SubtractMove(int subtract)
    {
        if(subtract > max_moves)
        {
            //Debug.LogError("Move is greater than the max_moves.");
            // PLAY ERROR SOUND
            return false;
        }
        else if(subtract > curr_moves)
        {
            //Debug.LogError("Move is greater than the currently available moves.");
            // PLAY ERROR SOUND
            return false;
        }
        else
        {
            curr_moves -= subtract;
        }
        return true;
    }

    // Checks tile that are walkable around the attacker tiles and makes them selectable.
    public bool PossibleAttackerMoves (PlaceScript place)
    {
        bool canMove = false;
        
        foreach(PlaceScript cell in boardArray)
        {
            if(Mathf.Abs(cell.xPos - place.xPos) + Mathf.Abs(cell.zPos - place.zPos) <= 1)
            {
                if((cell.GetState("WALK") || cell.GetState("BASE1") || cell.GetState("BASE2")) && !cell.GetAttacker())
                {
                    cell.ToggleSelectable();
                    canMove = true;
                }
            }
        }
        return canMove;
    }

    // Clears the board, toggles off all selected tiles and sets breakable tiles to non-breakable.
    public void ClearBoard()
    {
        foreach(PlaceScript place in boardArray){
            if (place.isSelected)
                place.ToggleSelectable();
            if (place.isBreakable)
                place.isBreakable = false;
        }
    }

    // Makes cells around the attackers selectable for a tile to be placed on them.
    public void PossibleTilePlacements()
    {
        foreach(PlaceScript cell in boardArray)
        {
            if(cell.GetAttacker() && cell.GetAttacker().team == curr_player)
            {
                PlaceScript currCell = cell;
                foreach(PlaceScript adjacentCell in boardArray)
                {
                    if (adjacentCell.GetState("EMPTY"))
                    {
                        if (adjacentCell.xPos > -1 && adjacentCell.xPos < BOARD_WIDTH)
                        {
                            if (adjacentCell.xPos == currCell.xPos + 1 && adjacentCell.zPos == currCell.zPos && !adjacentCell.isSelected)
                                adjacentCell.ToggleSelectable();

                            if (adjacentCell.xPos == currCell.xPos - 1 && adjacentCell.zPos == currCell.zPos && !adjacentCell.isSelected)
                                adjacentCell.ToggleSelectable();
                        }

                        if (adjacentCell.zPos > -1 && adjacentCell.zPos < BOARD_HEIGHT)
                        {
                            if (adjacentCell.zPos == currCell.zPos + 1 && adjacentCell.xPos == currCell.xPos && !adjacentCell.isSelected)
                                adjacentCell.ToggleSelectable();

                            if (adjacentCell.zPos == currCell.zPos - 1 && adjacentCell.xPos == currCell.xPos && !adjacentCell.isSelected)
                                adjacentCell.ToggleSelectable();
                        }
                    }
                }
            }
        }
    }

    public void ShowBreakableTiles()
    {
        for (int x = 0; x < BOARD_WIDTH; x++)
        {
            for (int z = 0; z < BOARD_HEIGHT; z++)
            {
                if (boardArray[x, z].GetAttacker())
                {
                    if (curr_player == boardArray[x, z].GetAttacker().team)
                    {
                        if (x < BOARD_WIDTH - 1)
                        {
                            if (boardArray[x + 1, z].GetState("WALK") || boardArray[x + 1, z].GetState("BLOCK"))
                                boardArray[x + 1, z].isBreakable = true;
                        }
                        if (x > 0)
                        {
                            if (boardArray[x - 1, z].GetState("WALK") || boardArray[x - 1, z].GetState("BLOCK"))
                                boardArray[x - 1, z].isBreakable = true; 
                        }
                        if (z < BOARD_HEIGHT - 1)
                        {
                            if (boardArray[x, z + 1].GetState("WALK") || boardArray[x, z + 1].GetState("BLOCK"))
                                boardArray[x, z + 1].isBreakable = true;
                        }
                        if (z > 0)
                        {
                            if (boardArray[x, z - 1].GetState("WALK") || boardArray[x, z - 1].GetState("BLOCK"))
                                boardArray[x, z - 1].isBreakable = true;
                        }
                    }
                }
            }
        }
    }

     /*
     * Getters
     */
    public PlaceScript[,] GetBoardArray()
    {
        return boardArray;
    }

    public List<PlayerHandHandler> GetHands()
    {
        return handHandlers;
    }

    public int GetCurrPlayer()
    {
        return curr_player;
    }

}
