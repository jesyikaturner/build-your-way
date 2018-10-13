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
        CheckForWinners();
	}

	public void SubtractMove(int subtract)
	{
		if(subtract <= 2)
			curr_moves -= subtract;
		else
			Debug.LogError("Trying to do more than 2 moves at once.");
	}

	void SwitchPlayer()
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
			if(p.blockType == 0)
			{
                Debug.Log("GAME --> DRAW NEW TILE");
                if (Random.Range(0,100) <= 35)
					p.blockType = 2;
				else
					p.blockType = 1;
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
                    PlaceScript handTile = CreatePlaceTile(x, z, -2, 0, "Player 1 Hand");
					handTile.playerHand = 1;
					playerHands[counter] = handTile;
					counter++;
				}
				if(z > height - 4 && x > width - 2)
				{
                    PlaceScript handTile = CreatePlaceTile(x, z, 2, 0, "Player 2 Hand");
					handTile.playerHand = 2;
					playerHands[counter] = handTile;
					counter++;
				}

                // Creates an Empty Board
                PlaceScript placeTile = CreatePlaceTile(x, z, 0, 0, "Place");
				placeTile.transform.parent = transform.GetChild (0).transform;
				placeTile.xPos = x;
				placeTile.zPos = z;
				boardArray[x, z] = placeTile;
			}
		}
	}

    private PlaceScript CreatePlaceTile(int x, int z, int offset, int blockType, string name)
    {
        PlaceScript placeTile = Instantiate(placement, new Vector3(x + offset * boardSpacing, 0, z * boardSpacing), Quaternion.identity);
        placeTile.name = name + ": " + z + ", " + x;
        return placeTile;
    }

    // Creates an attacker and assigns a team to it
    AttackerScript CreateAttacker(int team, int x, int z, string name)
    {
        AttackerScript attacker = Instantiate(playerAttacker, new Vector3(x, pieceOffset, z), Quaternion.identity);
        attacker.team = team;
        attacker.name = name;
        return attacker;
    }

	//applies a preset layout to the board
	void BoardLayout(int[,] layout, int layoutLength)
	{
		for (int i = 0; i < layoutLength; i++)
		{
			foreach(PlaceScript p in boardArray)
			{
				if(p.zPos == layout[i,0] && p.xPos == layout[i,1])
				{
					if(i < 3)
						p.blockType = 3;
					if(i > 2)
						p.blockType = 4;
					if(i > 5)
						p.blockType = 2;
				}
			}
		}

		foreach(PlaceScript p in boardArray)
		{
			if(p.blockType == 3)
			{
				p.SetAttacker(CreateAttacker(1,p.xPos,p.zPos,"Player 1 Attacker"));
			}
			if(p.blockType == 4)
			{
				p.SetAttacker(CreateAttacker(2,p.xPos,p.zPos, "Player 2 Attacker"));
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
                if((cell.blockType == 1 || cell.blockType == 3 || cell.blockType == 4) && !cell.GetAttacker())
                {

                    cell.blockType *= -1;
                    canMove = true;
                }
            }
        }
        return canMove;
    }

    public void ClearBoard()
    {
        foreach(PlaceScript place in boardArray){
            if (place.blockType == -1 || place.blockType == -3 || place.blockType == -4)
                place.blockType *= -1;
            if (place.blockType == -5)
                place.blockType = 0;
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
                            if(boardArray[x + 1,z].blockType == 0)
                                boardArray[x + 1, z].blockType = -5;
                        }
                        if (x > 0 )
                        {
                            if (boardArray[x - 1, z].blockType == 0)
                                boardArray[x - 1, z].blockType = -5;
                        }
                        if (z < boardHeight - 1)
                        {
                            if (boardArray[x, z + 1].blockType == 0)
                                boardArray[x, z + 1].blockType = -5;
                        }
                        if (z > 0)
                        {
                            if (boardArray[x, z - 1].blockType == 0)
                                boardArray[x, z - 1].blockType = -5;
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
                            if (boardArray[x + 1, z].blockType == 1 || boardArray[x + 1, z].blockType == 2)
                                boardArray[x + 1, z].isBreakable = true;
                        }
                        if (x > 0)
                        {
                            if (boardArray[x - 1, z].blockType == 1 || boardArray[x - 1, z].blockType == 2)
                                boardArray[x - 1, z].isBreakable = true; 
                        }
                        if (z < boardHeight - 1)
                        {
                            if (boardArray[x, z + 1].blockType == 1 || boardArray[x, z + 1].blockType == 2)
                                boardArray[x, z + 1].isBreakable = true;
                        }
                        if (z > 0)
                        {
                            if (boardArray[x, z - 1].blockType == 1 || boardArray[x, z - 1].blockType == 2)
                                boardArray[x, z - 1].isBreakable = true;
                        }
                    }
                }
            }
        }
    }

	void CheckForWinners()
	{
        int p1_winCounter = 0;
        int p2_winCounter = 0;

        foreach (PlaceScript p in boardArray)
		{
			if(p.blockType == 3)
			{
				if(p.GetAttacker())
				{
					if(p.GetAttacker().team == 2)
						p1_winCounter++;
					else
						p1_winCounter = 0;
				}
			}
			if(p.blockType == 4)
			{
				if(p.GetAttacker())
				{
					if(p.GetAttacker().team == 2)
						p1_winCounter++;
					else
						p1_winCounter = 0;
				}
			}

		}
		if(p1_winCounter > 3)
			Debug.Log("Player 1 Wins!");
		if(p2_winCounter > 3)
			Debug.Log("Player 2 Wins!");

	}
		

}
