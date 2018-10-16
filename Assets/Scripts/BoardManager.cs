using UnityEngine;
using System.Collections;

public class BoardManager : MonoBehaviour {

	public PlaceScript[,] boardArray;
	public PlaceScript[] playerHands;

	public PlaceScript placement;
	public AttackerScript playerAttacker;

	public int turn = 0;
	public int max_moves = 2;
	private int curr_moves = 2;
	public int curr_player = 1;

	public int boardWidth = 6, boardHeight = 6;
	public float boardSpacing = 1f; // the distance between the tiles
	public float pieceOffset = 0.1f; // height above the tile that pieces rest

	// Use this for initialization
	void Start () {
		boardArray = new PlaceScript[boardWidth,boardHeight];
		playerHands = new PlaceScript[6];

		//B1 - first 3, B2 - next 3, B - remaining 4
		int[,] layoutV1 = {{0,0},{1,0},{2,2},{3,3},{4,5},{5,5},{4,1},{3,2},{2,3},{1,4}}; 

		CreateBoard(boardWidth,boardHeight);
		BoardLayout(layoutV1,10);
	}
	
	// Update is called once per frame
	void Update () {
		SwitchPlayer();
		FillHands();
	}

	public void SubtractMove(int subtract)
	{
		if(subtract <= 2)
			curr_moves -= subtract;
		else
			Debug.LogError("Trying to do more than 2 moves at once.");
	}

	private void SwitchPlayer()
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

	// Fills player hands
	void FillHands()
	{
		foreach(PlaceScript p in playerHands)
		{
            if(p.state == PlaceScript.PlaceState.EMPTY)
            {
                if (Random.Range(0, 100) <= 25)
                    p.SetState("BLOCK");
                else
                    p.SetState("WALK");
            }
		}
	}
		
	void CreateBoard(int width, int height)
	{
		int counter = 0;
		for (int x = 0; x < width; x++)
		{
			for (int z = 0; z<height; z++)
			{
				// Creates Player Hands
				if(z < 3 && x < 1)
				{
                    PlaceScript handTile = CreatePlaceTile(x, z, -2, "Player 1 Hand");
					handTile.playerHand = 1;
					playerHands[counter] = handTile;
					counter++;
				}
				if(z > height - 4 && x > width - 2)
				{
                    PlaceScript handTile = CreatePlaceTile(x, z, 2, "Player 2 Hand");
					handTile.playerHand = 2;
					playerHands[counter] = handTile;
					counter++;
				}

                // Creates an Empty Board
                PlaceScript placeTile = CreatePlaceTile(x, z, 0, "Place");
				placeTile.transform.parent = transform.GetChild (0).transform;
				placeTile.xPos = x;
				placeTile.zPos = z;
				boardArray[x, z] = placeTile;
			}
		}
	}

	//applies a preset layout to the board
	void BoardLayout(int[,] layout, int layoutLength)
	{
        for (int i = 0; i < layoutLength; i++)
        {
            foreach (PlaceScript p in boardArray)
            {
                if (p.zPos == layout[i, 0] && p.xPos == layout[i, 1])
                {
                    if (i < 3)
                        p.SetState("BASE1");
                    if (i > 2)
                        p.SetState("BASE2");
                    if (i > 5)
                        p.SetState("BLOCK");

                    if (p.GetState("BASE1"))
                        p.SetAttacker(CreateAttacker(1, p.xPos, p.zPos, "Player 1 Attacker"));
                    if (p.GetState("BASE2"))
                        p.SetAttacker(CreateAttacker(2, p.xPos, p.zPos, "Player 2 Attacker"));
                }

            }
        }
	}

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

    public void ClearBoard()
    {
        foreach(PlaceScript place in boardArray){
            if (place.isSelected)
                place.ToggleSelectable();
            if (place.isBreakable)
                place.isBreakable = false;
        }
    }

    public void PossibleTilePlacements()
    {
        for(int x = 0; x < boardWidth; x++)
        {
            for (int z = 0; z < boardHeight; z++)
            {
                if (boardArray[x, z].GetAttacker())
                {
                    if(curr_player == boardArray[x, z].GetAttacker().team)
                    {
                        if(x < boardWidth - 1)
                        {
                            if(boardArray[x + 1,z].GetState("EMPTY"))
                                boardArray[x + 1, z].ToggleSelectable();
                        }
                        if (x > 0 )
                        {
                            if (boardArray[x - 1, z].GetState("EMPTY"))
                                boardArray[x - 1, z].ToggleSelectable();
                        }
                        if (z < boardHeight - 1)
                        {
                            if (boardArray[x, z + 1].GetState("EMPTY"))
                                boardArray[x, z + 1].ToggleSelectable();
                        }
                        if (z > 0)
                        {
                            if (boardArray[x, z - 1].GetState("EMPTY"))
                                boardArray[x, z - 1].ToggleSelectable();
                        }
                    }
                }
            }
        }
    }

    public void ShowBreakableTiles()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int z = 0; z < boardHeight; z++)
            {
                if (boardArray[x, z].GetAttacker())
                {
                    if (curr_player == boardArray[x, z].GetAttacker().team)
                    {
                        if (x < boardWidth - 1)
                        {
                            if (boardArray[x + 1, z].GetState("WALK") || boardArray[x + 1, z].GetState("BLOCK"))
                                boardArray[x + 1, z].isBreakable = true;
                        }
                        if (x > 0)
                        {
                            if (boardArray[x - 1, z].GetState("WALK") || boardArray[x - 1, z].GetState("BLOCK"))
                                boardArray[x - 1, z].isBreakable = true; 
                        }
                        if (z < boardHeight - 1)
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

}
