using UnityEngine;
using System;
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

    // Private Variables
    private Tile[,] boardArray;
    private List<PlayerHand> handHandlers;
    private int turn = 0;
    [SerializeField] private int curr_moves = 2;
	private int curr_player = 1;
    private bool isPaused = false;

    private List<Tile> tileDeck;
    private int deckSize = 50;

    public IController currentPlayer;


    // Used to read boardlayout txt files
    private readonly FileHandler _fileHandler = FileHandler.Instance;
    public List<string> boardString = new();

    /*
     * Setting up board layout.
     */
     
    void Start()
    {
        // PopulateTileDeck();
        SetupBoard();
        // SetupPlayerHands();
    }

#region Board Setup
    // Board setup from text file
    // b - BLOCK
    // e - EMPTY
    // 1 - BASE1
    // 2 - BASE2
    // w - WALK
    private void SetupBoard()
    {
        boardString = _fileHandler.ReadTextFile("Scripts/Data/boardlayout.txt");
        boardArray = new Tile[BOARD_WIDTH, BOARD_HEIGHT];

        for(int row = 0; row < boardString.Count; row++)
        {
            string[] tempRow = boardString[row].Split(new string[]{","}, StringSplitOptions.None);

            for (int col = 0; col < BOARD_WIDTH; col++)
            {
                Tile tile = null;

                switch(tempRow[col])
                {
                    case "e":
                        //Takes in x value, z value, the offset and the name of the object
                        tile = CreatePlaceTile(col, row, 0, "Place");
                        break;
                    case "b":
                        tile = CreatePlaceTile(col, row, 0, "Block");
                        // Setting the object's state to block
                        tile.SetType(Tile.TileType.BLOCK);
                        break;
                    case "w":
                        tile = CreatePlaceTile(col, row, 0, "Walk");
                        tile.SetType(Tile.TileType.WALK);
                        break;
                    case "1":
                        tile = CreatePlaceTile(col, row, 0, "Base 1");
                        tile.SetType(Tile.TileType.BASE1);
                        // Create attacker for base tile
                        tile.SetAttacker(CreateAttacker(tile.Team, new Vector2(col, row), "Player 1 Attacker", tile));
                        break;
                    case "2":
                        tile = CreatePlaceTile(col, row, 0, "Base 2");
                        tile.SetType(Tile.TileType.BASE2);
                        tile.SetAttacker(CreateAttacker(tile.Team, new Vector2(col, row), "Player 2 Attacker", tile));
                        break;
                    default:
                        Debug.Log(tempRow[col]);
                        break;
                }
                // if a tile doesn't get created, exit out
                if(!tile)
                    return;
                
                // Setting the object's parent to a child of this script's gameobject.
                tile.transform.parent = transform.GetChild(0).transform;
                tile.Position = new Vector2(col, row);
                boardArray[col, row] = tile;
            }
        }
    }
#endregion

#region Tile Creation Helpers
    private Tile CreatePlaceTile(int x, int z, int offset, string name)
    {
        Tile placeTile = Instantiate(placement, new Vector3(x + offset * BOARD_SPACING, 0, z * BOARD_SPACING), Quaternion.identity);
        // placeTile.xPos = x;
        // placeTile.zPos = z;
        string newName = string.Format("{0}: {1}, {2}", name, z, x);
        placeTile.name = newName;
        return placeTile;
    }

    // Creates an attacker and assigns a team to it
    private Attacker CreateAttacker(int team, Vector2 position, string name, Tile originTile)
    {
        Attacker attacker = Instantiate(playerAttacker, new Vector3(position[0], PIECE_OFFSET, position[1]), Quaternion.identity);
        attacker.Team = team;
        attacker.name = name;
        attacker.UpdateHistory(originTile);
        return attacker;
    }
#endregion

    private void PopulateTileDeck()
    {
        tileDeck = new List<Tile>();

        for(int i  = 0; i < deckSize; i++)
        {
            // TODO variable for tiledeck position
            tileDeck.Add(Instantiate(placement, new Vector3(-20,0), Quaternion.identity));
            if (i < (deckSize/3))
            {

                tileDeck[i].SetType(Tile.TileType.BLOCK);
            }
            else
            {
                tileDeck[i].SetType(Tile.TileType.WALK);
            }
        }
        // shuffle here
        tileDeck.Shuffle<Tile>();
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
        handHandlers[0].PopulatePlayerHand(ref tileDeck, PLAYER_ONE_COORDS, placement, -2, 1);
        // Fills the player's hand with random tiles.
        handHandlers[0].FillHand();
        handHandlers[1].PopulatePlayerHand(ref tileDeck, PLAYER_TWO_COORDS, placement, 2, 2);
        handHandlers[1].FillHand();
    }

    #region Public Variables
    // Move Attacker from origin tile to target tile.
    public void SetAttacker(Tile origin, Tile target)
    {
        target.SetAttacker(origin.Attacker);
        origin.Attacker.UpdateHistory(target);
        origin.Attacker.transform.position = new Vector3(target.transform.position.x, PIECE_OFFSET, target.transform.position.z);
        origin.Attacker.ToggleSelect();
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
    public bool PossibleAttackerMoves (Tile currTile)
    {
        bool canMove = false;
        
        foreach(Tile tile in boardArray)
        {
            if(Mathf.Abs(tile.Position[0] - currTile.Position[0]) + Mathf.Abs(tile.Position[1] - currTile.Position[1]) <= 1)
            {
                if ((tile.Type == Tile.TileType.WALK || tile.Type == Tile.TileType.BASE1 || tile.Type == Tile.TileType.BASE2)
                && !tile.Attacker && !tile.IsSelected)
                {
                    tile.ToggleSelectable();
                    canMove = true;
                }
            }
        }
        return canMove;
    }

    // Clears the board, toggles off all selected tiles and sets breakable tiles to non-breakable.
    public void ClearBoard()
    {
        foreach(Tile tile in boardArray)
        {
            if (tile.IsSelected)
                tile.ToggleSelectable();
            if (tile.isBreakable)
                tile.isBreakable = false;
        }
    }

    // Makes cells around the attackers selectable for a tile to be placed on them.
    public void PossibleTilePlacements(int playerID)
    {
        Debug.Log("PossibleTilePlacements");
        foreach(Tile tile in boardArray)
        {
            if(tile.Attacker && tile.Attacker.team == playerID)
            {
                // Tile currTile = tile;

                foreach(Tile adjacentTile in boardArray)
                {
                    // Tile currAdjacentTile = adjacentTile;
                    if (adjacentTile.Type == Tile.TileType.EMPTY)
                    {
                        if (adjacentTile.Position[0] > -1 && adjacentTile.Position[0] < BOARD_WIDTH)
                        {
                            if (adjacentTile.Position[0] == tile.Position[0] + 1 && adjacentTile.Position[1] == tile.Position[1] && !adjacentTile.IsSelected)
                                adjacentTile.ToggleSelectable();

                            if (adjacentTile.Position[0] == tile.Position[0] - 1 && adjacentTile.Position[1] == tile.Position[1] && !adjacentTile.IsSelected)
                                adjacentTile.ToggleSelectable();
                        }

                        // if (adjacentCell.GetPosition()[1] > -1 && adjacentCell.GetPosition()[1] < BOARD_HEIGHT)
                        // {
                        //     if (adjacentCell.GetPosition()[1] == currCell.GetPosition()[1] + 1 && adjacentCell.GetPosition()[0] == currCell.GetPosition()[0] && !adjacentCell.TileIsSelected())
                        //         adjacentCell.ToggleSelectable();

                        //     if (adjacentCell.GetPosition()[1] == currCell.GetPosition()[1] - 1 && adjacentCell.GetPosition()[0] == currCell.GetPosition()[0] && !adjacentCell.TileIsSelected())
                        //         adjacentCell.ToggleSelectable();
                        // }
                    }
                }
            }
        }
    }

    public void ShowBreakableTiles()
    {
        // for (int x = 0; x < BOARD_WIDTH; x++)
        // {
        //     for (int z = 0; z < BOARD_HEIGHT; z++)
        //     {
        //         if (boardArray[x, z].GetAttacker())
        //         {
        //             if (curr_player == boardArray[x, z].GetAttacker().Team)
        //             {
        //                 if (x < BOARD_WIDTH - 1)
        //                 {
        //                     if (boardArray[x + 1, z].GetTileInfo().CheckState(TileInfo.TileState.WALK) || boardArray[x + 1, z].GetTileInfo().CheckState(TileInfo.TileState.BLOCK))
        //                         boardArray[x + 1, z].isBreakable = true;
        //                 }
        //                 if (x > 0)
        //                 {
        //                     if (boardArray[x - 1, z].GetTileInfo().CheckState(TileInfo.TileState.WALK) || boardArray[x - 1, z].GetTileInfo().CheckState(TileInfo.TileState.BLOCK))
        //                         boardArray[x - 1, z].isBreakable = true; 
        //                 }
        //                 if (z < BOARD_HEIGHT - 1)
        //                 {
        //                     if (boardArray[x, z + 1].GetTileInfo().CheckState(TileInfo.TileState.WALK) || boardArray[x, z + 1].GetTileInfo().CheckState(TileInfo.TileState.BLOCK))
        //                         boardArray[x, z + 1].isBreakable = true;
        //                 }
        //                 if (z > 0)
        //                 {
        //                     if (boardArray[x, z - 1].GetTileInfo().CheckState(TileInfo.TileState.WALK) || boardArray[x, z - 1].GetTileInfo().CheckState(TileInfo.TileState.BLOCK))
        //                         boardArray[x, z - 1].isBreakable = true;
        //                 }
        //             }
        //         }
        //     }
        // }
    }

    public void RestartBoard()
    {
        foreach(Tile cell in boardArray)
        {
            if (cell.Attacker)
            {
                Destroy(cell.Attacker.gameObject);
                cell.SetAttacker(null);
            }
            if (cell.Type != Tile.TileType.EMPTY)
            {
                cell.SetType(Tile.TileType.EMPTY);
                // cell.team = 0;
            }
        }
        // SetupBoardLayout();
        turn = 0;
        curr_player = 1;
    }

    #region Getters Setters
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
    #endregion

}
#endregion