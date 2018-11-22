using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {

    // Public Variables for the Inspector
    public Tile placement;
    public Attacker playerAttacker;

    // Constants
    private const int BOARD_WIDTH = 6, BOARD_HEIGHT = 6;
    private const float BOARD_SPACING = 1f; // the distance between the tiles
    private const float PIECE_OFFSET = 0.1f; // height above the tile that pieces rest
    private const int max_moves = 2;

    private readonly int[,] PLAYER_ONE_COORDS = { { 0, 0 }, { 0, 1 }, { 0, 2 } };
    private readonly int[,] PLAYER_TWO_COORDS = { { 5, 3 }, { 5, 4 }, { 5, 5 } };
    //B1 - first 3, B2 - next 3, B - remaining 4
    private readonly int[,] BOARD_LAYOUT = { { 0, 0 }, { 1, 0 }, { 2, 2 }, { 3, 3 }, { 4, 5 }, { 5, 5 }, { 4, 1 }, { 3, 2 }, { 2, 3 }, { 1, 4 } };

    // Private Variables
    private Tile[,] boardArray;
    private List<PlayerHand> handHandlers;
    private int turn = 0;
    private int curr_moves = 2;
	private int curr_player = 1;
    private bool isPaused = false;

    /*
     * Setting up board layout.
     */
    void Awake () {
        SetupBoard();
        SetupPlayerHands();
        SetupBoardLayout();
    }

    // Sets up empty places on the board.
    private void SetupBoard()
    {
        boardArray = new Tile[BOARD_WIDTH, BOARD_HEIGHT];

        for (int x = 0; x < BOARD_HEIGHT; x++)
        {
            for (int z = 0; z < BOARD_WIDTH; z++)
            {
                Tile placeTile = CreatePlaceTile(x, z, 0, "Place");
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
        handHandlers = new List<PlayerHand>
        {
            gameObject.AddComponent<PlayerHand>(),
            gameObject.AddComponent<PlayerHand>()
        };

        handHandlers[0].PopulatePlayerHand(PLAYER_ONE_COORDS, placement, -2, BOARD_SPACING, "PLAYER 1 HAND", 1);
        handHandlers[0].FillHand();
        handHandlers[1].PopulatePlayerHand(PLAYER_TWO_COORDS, placement, 2, BOARD_SPACING, "PLAYER 2 HAND", 2);
        handHandlers[1].FillHand();
    }

    //applies a preset layout to the board
    private void SetupBoardLayout()
    {
        for (int i = 0; i < BOARD_LAYOUT.GetLength(0); i++)
        {
            foreach (Tile cell in boardArray)
            {
                if (cell.zPos == BOARD_LAYOUT[i, 0] && cell.xPos == BOARD_LAYOUT[i, 1])
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

    private Tile CreatePlaceTile(int x, int z, int offset, string name)
    {
        Tile placeTile = Instantiate(placement, new Vector3(x + offset * BOARD_SPACING, 0, z * BOARD_SPACING), Quaternion.identity);
        placeTile.SetState("EMPTY");
        placeTile.name = name + ": " + z + ", " + x;
        return placeTile;
    }

    // Creates an attacker and assigns a team to it
    private Attacker CreateAttacker(int team, int x, int z, string name)
    {
        Attacker attacker = Instantiate(playerAttacker, new Vector3(x, PIECE_OFFSET, z), Quaternion.identity);
        attacker.Team = team;
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

    // Move Attacker from origin tile to target tile.
    public void SetAttacker(Tile origin, Tile target)
    {
        target.SetAttacker(origin.GetAttacker());
        origin.GetAttacker().transform.position = new Vector3(target.transform.position.x, PIECE_OFFSET, target.transform.position.z);
        origin.GetAttacker().ToggleSelect();
        origin.SetAttacker(null);
    }
    
    // Does checks to see if the value to be subtracted from the moves is correct.
    public bool SubtractMove(int subtract)
    {
        if(subtract > max_moves)
            return false;
        else if(subtract > curr_moves)
            return false;
        else
            curr_moves -= subtract;
        return true;
    }

    // Checks tile that are walkable around the attacker tiles and makes them selectable.
    public bool PossibleAttackerMoves (Tile place)
    {
        bool canMove = false;
        
        foreach(Tile cell in boardArray)
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
        foreach(Tile place in boardArray){
            if (place.isSelected)
                place.ToggleSelectable();
            if (place.isBreakable)
                place.isBreakable = false;
        }
    }

    // Makes cells around the attackers selectable for a tile to be placed on them.
    public void PossibleTilePlacements()
    {
        foreach(Tile cell in boardArray)
        {
            if(cell.GetAttacker() && cell.GetAttacker().Team == curr_player)
            {
                Tile currCell = cell;
                foreach(Tile adjacentCell in boardArray)
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
                    if (curr_player == boardArray[x, z].GetAttacker().Team)
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

    public void RestartBoard()
    {
        foreach(Tile cell in boardArray)
        {
            if (cell.GetAttacker())
            {
                Destroy(cell.GetAttacker().gameObject);
                cell.SetAttacker(null);
            }
            if (!cell.GetState("EMPTY"))
            {
                cell.SetState("EMPTY");
                cell.team = 0;
            }
        }
        SetupBoardLayout();
        turn = 0;
        curr_player = 1;
    }

     /*
     * Getters, Setters
     */
    public Tile[,] GetBoardArray()
    {
        return boardArray;
    }

    public List<PlayerHand> GetHands()
    {
        return handHandlers;
    }

    public int GetCurrPlayer()
    {
        return curr_player;
    }

    public bool IsPaused
    {
        get
        {
            return isPaused;
        }

        set
        {
            isPaused = value;
        }
    }

}
