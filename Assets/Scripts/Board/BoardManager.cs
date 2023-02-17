using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {

    // ** Inspector Variables **
    public Tile placement;
    public Attacker playerAttacker;

    // ** Constants **
    private const int BOARD_WIDTH = 6, BOARD_HEIGHT = 6; // Size of the board
    private const float BOARD_SPACING = 1f; // Distance between the tiles
    private const float PIECE_OFFSET = 0.1f; // Distance above the tile that pieces rest
    private const int TILE_DECK_SIZE = 50; // Total number of tiles in the tile deck
    private readonly Vector2 TILE_DECK_COORDS = new Vector2(-20, 0); // Coords of Deck

    private readonly int[,] PLAYER_ONE_COORDS = { { 0, 0 }, { 0, 1 }, { 0, 2 } };
    private readonly int[,] PLAYER_TWO_COORDS = { { 5, 3 }, { 5, 4 }, { 5, 5 } };

    // Private Variables
    private List<PlayerHand> handHandlers;
    // private bool isPaused = false;

    private List<Tile> tileDeck;

    public int CurrMovesLeft { get; set; }
    public Tile[,] BoardArray { get; set; }
    public List<PlayerHand> ControllerHandsList { get; set; }
    public List<Tile> TileDeck { get; set; }

    /*
     * Setting up board layout.
     */
     
    void Start()
    {
        PopulateTileDeck();
        SetupBoard();
        SetupPlayerHands();
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
        // Reads the board layout from text file
        FileHandler _fileHandler = FileHandler.Instance;
        List<string> boardString = _fileHandler.ReadTextFile("Scripts/Data/boardlayout.txt");

        BoardArray = new Tile[BOARD_WIDTH, BOARD_HEIGHT];

        for(int row = 0; row < boardString.Count; row++)
        {
            string[] tempRow = boardString[row].Split(new string[]{","}, StringSplitOptions.None);

            for (int col = 0; col < BOARD_WIDTH; col++)
            {
                switch(tempRow[col])
                {
                    case "e":
                        //Takes in x value, z value, the offset and the name of the object
                        Tile emptyTile = CreatePlaceTile(col, row, 0, "Empty");
                        emptyTile.SetStatus(Tile.TileStatus.EMPTY);
                        BoardArray[col, row] = emptyTile;
                        break;
                    case "b":
                        Tile blockTile = CreatePlaceTile(col, row, 0, "Block");
                        // Setting the object's state to block
                        blockTile.SetStatus(Tile.TileStatus.BLOCK);
                        BoardArray[col, row] = blockTile;
                        break;
                    case "w":
                        Tile walkTile = CreatePlaceTile(col, row, 0, "Walk");
                        walkTile.SetStatus(Tile.TileStatus.WALK);
                        BoardArray[col, row] = walkTile;
                        break;
                    case "1":
                        Tile base1Tile = CreatePlaceTile(col, row, 0, "Base 1");
                        base1Tile.SetStatus(Tile.TileStatus.BASE1);
                        BoardArray[col, row] = base1Tile;
                        // Create attacker for base tile
                        base1Tile.SetAttacker(CreateAttacker(base1Tile.Team, new Vector2(col, row), "Player 1 Attacker", base1Tile));
                        break;
                    case "2":
                        Tile base2Tile = CreatePlaceTile(col, row, 0, "Base 2");
                        base2Tile.SetStatus(Tile.TileStatus.BASE2);
                        BoardArray[col, row] = base2Tile;
                        base2Tile.SetAttacker(CreateAttacker(base2Tile.Team, new Vector2(col, row), "Player 2 Attacker", base2Tile));
                        break;
                    default:
                        Debug.Log(tempRow[col]);
                        break;
                }
            }
        }
    }
#endregion

#region Board Tile Creation Helpers
    private Tile CreatePlaceTile(int x, int z, int offset, string name)
    {
        Tile placeTile = Instantiate(placement, new Vector3(x + offset * BOARD_SPACING, 0, z * BOARD_SPACING), Quaternion.identity);
        // Setting the object's parent to a child of this script's gameobject.
        placeTile.transform.parent = transform.GetChild(0).transform;
        placeTile.Position = new Vector2(x, z);
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

#region Hand Tile Setup
    private void PopulateTileDeck()
    {
        TileDeck = new();

        for (int i = 0; i < TILE_DECK_SIZE; i++)
        {
            TileDeck.Add(Instantiate(placement, TILE_DECK_COORDS, Quaternion.identity));
            if(i < (TILE_DECK_SIZE/3)) TileDeck[i].SetStatus(Tile.TileStatus.BLOCK);
            else TileDeck[i].SetStatus(Tile.TileStatus.WALK);
        }

        TileDeck.Shuffle<Tile>();


        tileDeck = new List<Tile>();

        for(int i  = 0; i < TILE_DECK_SIZE; i++)
        {
            // TODO variable for tiledeck position
            tileDeck.Add(Instantiate(placement, new Vector3(-20,0), Quaternion.identity));
            if (i < (TILE_DECK_SIZE/3))
            {

                tileDeck[i].SetStatus(Tile.TileStatus.BLOCK);
            }
            else
            {
                tileDeck[i].SetStatus(Tile.TileStatus.WALK);
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
#endregion


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
        if (subtract > CurrMovesLeft)
            return false;

        CurrMovesLeft -= subtract;
        return true;
    }

    // Checks tile that are walkable around the attacker tiles and makes them selectable.
    public bool PossibleAttackerMoves (Tile currTile)
    {
        bool canMove = false;
        
        foreach(Tile tile in BoardArray)
        {
            if(Mathf.Abs(tile.Position[0] - currTile.Position[0]) + Mathf.Abs(tile.Position[1] - currTile.Position[1]) <= 1)
            {
                if ((tile.Status == Tile.TileStatus.WALK || tile.Status == Tile.TileStatus.BASE1 || tile.Status == Tile.TileStatus.BASE2)
                && !tile.Attacker && !tile.IsSelected)
                {
                    tile.ToggleSelectable();
                    canMove = true;
                }
            }
        }
        return canMove;
    }

#region Board Helpers
    // Makes tiles around the current player's attackers selectable for a tile to be placed on them.
    public void PossibleTilePlacements(int playerID)
    {
        foreach(Tile currTile in BoardArray)
        {
            if(currTile.Attacker && currTile.Attacker.team == playerID)
            {
                foreach(Tile adjacentTile in BoardArray)
                {
                    if (adjacentTile.Status == Tile.TileStatus.EMPTY)
                    {
                        if (adjacentTile.Position[0] > -1 && adjacentTile.Position[0] < BOARD_WIDTH)
                        {
                            if (adjacentTile.Position[0] == currTile.Position[0] + 1 && adjacentTile.Position[1] == currTile.Position[1] && !adjacentTile.IsSelected)
                                adjacentTile.ToggleSelectable();

                            if (adjacentTile.Position[0] == currTile.Position[0] - 1 && adjacentTile.Position[1] == currTile.Position[1] && !adjacentTile.IsSelected)
                                adjacentTile.ToggleSelectable();
                        }

                        if (adjacentTile.Position[1] > -1 && adjacentTile.Position[1] < BOARD_HEIGHT)
                        {
                            if (adjacentTile.Position[1] == currTile.Position[1] + 1 && adjacentTile.Position[0] == currTile.Position[0] && !adjacentTile.IsSelected)
                                adjacentTile.ToggleSelectable();

                            if (adjacentTile.Position[1] == currTile.Position[1] - 1 && adjacentTile.Position[0] == currTile.Position[0] && !adjacentTile.IsSelected)
                                adjacentTile.ToggleSelectable();
                        }
                    }
                }
            }
        }
    }

    // Allows tiles that are not base tiles around the attackers to be breakable.
    public void ShowBreakableTiles(int playerID)
    {
        foreach(Tile currTile in BoardArray)
        {
            if(currTile.Attacker && currTile.Attacker.team == playerID)
            {
                foreach(Tile adjacentTile in BoardArray)
                {
                    if (adjacentTile.Status != Tile.TileStatus.BASE1 || adjacentTile.Status != Tile.TileStatus.BASE2)
                    {
                        if (adjacentTile.Position[0] > -1 && adjacentTile.Position[0] < BOARD_WIDTH)
                        {
                            if (adjacentTile.Position[0] == currTile.Position[0] + 1 && adjacentTile.Position[1] == currTile.Position[1] && !adjacentTile.IsSelected)
                                adjacentTile.isBreakable = true;

                            if (adjacentTile.Position[0] == currTile.Position[0] - 1 && adjacentTile.Position[1] == currTile.Position[1] && !adjacentTile.IsSelected)
                                adjacentTile.isBreakable = true;
                        }

                        if (adjacentTile.Position[1] > -1 && adjacentTile.Position[1] < BOARD_HEIGHT)
                        {
                            if (adjacentTile.Position[1] == currTile.Position[1] + 1 && adjacentTile.Position[0] == currTile.Position[0] && !adjacentTile.IsSelected)
                                adjacentTile.isBreakable = true;

                            if (adjacentTile.Position[1] == currTile.Position[1] - 1 && adjacentTile.Position[0] == currTile.Position[0] && !adjacentTile.IsSelected)
                                adjacentTile.isBreakable = true;
                        }
                    }
                }
            }
        }
    }

    public void RestartBoard()
    {
        // TODO: Rewrite this for current board setup
        // foreach(Tile tile in BoardArray)
        // {
        //     if (tile.Attacker)
        //     {
        //         Destroy(tile.Attacker.gameObject);
        //         tile.SetAttacker(null);
        //     }
        //     if (tile.Status != Tile.TileStatus.EMPTY)
        //     {
        //         tile.SetStatus(Tile.TileStatus.EMPTY);
        //         tile.Team = 0;
        //     }
        // }
        // // SetupBoardLayout();
        // turn = 0;
        // curr_player = 1;
    }

    // Clears the board, toggles off all selected tiles and sets breakable tiles to non-breakable.
    public void ClearBoard()
    {
        foreach(Tile tile in BoardArray)
        {
            if (tile.IsSelected)
                tile.ToggleSelectable();
            if (tile.isBreakable)
                tile.isBreakable = false;
        }
    }
#endregion


    public List<PlayerHand> GetHands()
    {
        return handHandlers;
    }

    // public bool IsPaused()
    // {
    //     return isPaused;
    // }

    // public void IsPaused(bool isPaused)
    // {
    //     this.isPaused = isPaused;
    // }

}