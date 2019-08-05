using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {

    // Public Variables for the Inspector
    public Tile placement;
    public Attacker playerAttacker;

    // Constants
    private const int BOARD_WIDTH = 6, BOARD_HEIGHT = 6; // Size of the board
    private const float BOARD_SPACING = 1f; // Distance between the tiles
    private const float PIECE_OFFSET = 0.1f; // Distance above the tile that pieces rest
    private const int max_moves = 2; // Total moves each player can do

    private readonly int[,] PLAYER_ONE_COORDS = { { 0, 0 }, { 0, 1 }, { 0, 2 } };
    private readonly int[,] PLAYER_TWO_COORDS = { { 5, 3 }, { 5, 4 }, { 5, 5 } };

    // TileInfo(string name, int team, int z, int x) 
    private readonly TileInfo[] STANDARD_BOARD = {
        new TileInfo("BASE1", 1, 0, 0),
        new TileInfo("BASE1", 1, 1, 0),
        new TileInfo("BASE1", 1, 2, 2),
        new TileInfo("BASE2", 2, 3, 3),
        new TileInfo("BASE2", 2, 4, 5),
        new TileInfo("BASE2", 2, 5, 5),
        new TileInfo("BLOCK", 0, 4, 1),
        new TileInfo("BLOCK", 0, 3, 2),
        new TileInfo("BLOCK", 0, 2, 3),
        new TileInfo("BLOCK", 0, 1, 4)
    };

    // Private Variables
    private Tile[,] boardArray;
    private List<PlayerHand> handHandlers;
    private int turn = 0;
    [SerializeField]
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
                // Takes in x value, z value, the offset and the name of the object
                Tile placeTile = CreatePlaceTile(x, z, 0, "Place");
                // Setting the object's parent to a child of this script's gameobject.
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
        // Holds the player hands.
        handHandlers = new List<PlayerHand>
        {
            gameObject.AddComponent<PlayerHand>(),
            gameObject.AddComponent<PlayerHand>()
        };

        // Takes in the hand coords, the tile object, x offset, offset is multiplied by this float, the name of the objects, the player id.
        handHandlers[0].PopulatePlayerHand(PLAYER_ONE_COORDS, placement, -2, BOARD_SPACING, "PLAYER 1 HAND", 1);
        // Fills the player's hand with random tiles.
        handHandlers[0].FillHand();
        handHandlers[1].PopulatePlayerHand(PLAYER_TWO_COORDS, placement, 2, BOARD_SPACING, "PLAYER 2 HAND", 2);
        handHandlers[1].FillHand();
    }

    // Applies a preset layout to the board
    private void SetupBoardLayout()
    {
        foreach(Tile cell in boardArray)
        {
            foreach(TileInfo tile in STANDARD_BOARD)
            {
                if(cell.zPos == tile.GetZPos() && cell.xPos == tile.GetXPos())
                {
                    cell.SetState(tile.GetState());
                    cell.team = tile.GetTeam();

                    if (cell.GetState("BASE1"))
                        cell.SetAttacker(CreateAttacker(cell.team, cell.xPos, cell.zPos, "Player 1 Attacker", cell));
                    if (cell.GetState("BASE2"))
                        cell.SetAttacker(CreateAttacker(cell.team, cell.xPos, cell.zPos, "Player 2 Attacker", cell));
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
    private Attacker CreateAttacker(int team, int x, int z, string name, Tile originTile)
    {
        Attacker attacker = Instantiate(playerAttacker, new Vector3(x, PIECE_OFFSET, z), Quaternion.identity);
        attacker.Team = team;
        attacker.name = name;
        attacker.UpdateHistory(originTile);
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
        origin.GetAttacker().UpdateHistory(target);
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
                if((cell.GetState("WALK") || cell.GetState("BASE1") || cell.GetState("BASE2")) && !cell.GetAttacker() && !cell.isSelected)
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

    public int GetCurrMoveAmount()
    {
        return curr_moves;
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void IsPaused(bool isPaused)
    {
        this.isPaused = isPaused;
    }

}
