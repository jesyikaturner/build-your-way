using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {

	public PlaceScript[,] boardArray;
	public PlaceScript[] playerHands;
	public Queue<TileScript> tileDeck;
	public Stack<TileScript> tileStack;

	public PlaceScript placement;
	public TileScript tile;
	public AttackerScript attacker1,attacker2;
	public PlayerControls pc;

	public int boardWidth = 6, boardHeight = 6;
	public float boardSpacing = 1.1f; // the distance between the tiles
	public float pieceOffset = 0.1f; // height above the tile that pieces rest

	private int turns = 1;
	public int moves = 2;
	public int player = 1;

	public Sprite walk, block, base1, base2;
	public Sprite s_Walk, s_Block, s_B1, s_B2;
	public Sprite walkBreak1, walkBreak2, blockBreak1, blockBreak2;
	public Material tileWalk, tileBlock;
	public Material baseTeam1, baseTeam2;
	public Material place, s_place;

	public bool moving = false;
	
	public PlaceScript selectedPlace;

	public List<PlaceScript> availableTiles, compAttackers, playerBaseTiles, opponentBaseTiles;

	public bool is1P = false;

	int p1Count = 0, p2Count = 0;
	public bool p1win, p2win;

	void Start () 
	{
		pc = transform.GetComponent<PlayerControls>();
		if(!is1P)
			transform.GetComponent<ComputerPlayer>().enabled = false;
		// Setting the size of the gameboard and player hand arrays.
		boardArray = new PlaceScript[boardWidth,boardHeight];
		playerHands = new PlaceScript[6];

		// Creating tiles and adding them to the stack.
		tileDeck = new Queue<TileScript>();

		FillDeck();

		// Creating the gameboard with the tiles and setting the variables up for each tile. 
		for (int x = 0; x < boardWidth; x++)
		{
			for (int z = 0; z<boardHeight; z++)
			{
				PlaceScript placeTile = (PlaceScript)Instantiate (placement, new Vector3(x * boardSpacing, 0, z * boardSpacing), Quaternion.identity);
				placeTile.transform.parent = transform.GetChild (0).transform;
				placeTile.name = "" + z + ", " + x;
				placeTile.xPos = x;
				placeTile.zPos = z;
				boardArray[x, z] = placeTile;
				BoardVariation1(placeTile,x,z);
			}
		}
		//Creating the player hands and adding them to the player hand array.
		CreatePlayerHands();
	}

	// Creating tiles and filling the Tile Deck
	void FillDeck()
	{
		for (int i = 0; i < 40; i++)
		{
			TileScript newTile = (TileScript)Instantiate(tile,Vector3.zero,Quaternion.identity);
			newTile.transform.parent = transform.GetChild (1).transform;
			int rand = Random.Range(0,100);
			if(rand <= 55)
			{
				newTile.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = walk;
				newTile.gameObject.name = "Walk Tile";
				newTile.isWalkable = true;
			}
			else
			{
				newTile.gameObject.name = "Block Tile";
				newTile.isWalkable = false;
				newTile.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = block;
			}
			newTile.gameObject.SetActive(false);
			tileDeck.Enqueue(newTile);
		}
	}

	// Creating base tiles
	void CreateBases(PlaceScript place, int x, int z, int team)
	{
		TileScript newTile = (TileScript)Instantiate(tile,Vector3.zero,Quaternion.identity);
		AttackerScript attacker = (AttackerScript)Instantiate(attacker1,Vector3.zero,Quaternion.identity);

		if(team == 1)
		{
			attacker.team = 1;
			place.SetAttacker(attacker);
		}
		if(team == 2)
		{
			attacker.team = 2;
		}

		place.SetAttacker(attacker);


		newTile.transform.position = new Vector3(x* boardSpacing,pieceOffset,z* boardSpacing);
		place.SetTile(newTile);
	}

	void CreateTiles(PlaceScript place, int x, int z, bool isBase, bool isWalkable, bool hasAttacker, int team)
	{
		if(isBase)
		{
			TileScript nTile = (TileScript)Instantiate(tile,Vector3.zero,Quaternion.identity);
			nTile.transform.parent = transform.GetChild (2).transform;

			if(team == 1)
			{	
				nTile.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = base1;
				nTile.gameObject.name = "Base Team 1";	
				nTile.team = 1; 
				playerBaseTiles.Add (place);
			}
			else if(team == 2)
			{	
				nTile.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = base2; 
				nTile.gameObject.name = "Base Team 2";	
				nTile.team = 2;
				opponentBaseTiles.Add (place);
			}
			else
			{
				Debug.Log ("Invalid base team");
			}
			nTile.isWalkable = true;
			nTile.transform.position = new Vector3(x* boardSpacing,pieceOffset,z* boardSpacing);
			nTile.transform.parent = transform.GetChild(2).transform;
			place.SetTile(nTile);
		}
		else
		{
			TileScript newTile = tileDeck.Dequeue();
			newTile.transform.parent = transform.GetChild (2).transform;
			
			if(isWalkable)
			{
				newTile.gameObject.name = "Walk Tile";
				newTile.isWalkable = true;
				newTile.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = walk;
			}
			else
			{
				newTile.gameObject.name = "Block Tile";
				newTile.isWalkable = false;
				newTile.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = block;
			}

			newTile.transform.position = new Vector3(x* boardSpacing,pieceOffset,z* boardSpacing);
			newTile.transform.parent = transform.GetChild(2).transform;
			place.SetTile(newTile);
			newTile.gameObject.SetActive(true);
		}
		if(hasAttacker)
		{
			if(team == 1)
			{
				AttackerScript nAttacker1 = (AttackerScript)Instantiate(attacker1,new Vector3 (place.transform.position.x,0.3f,place.transform.position.z),Quaternion.identity);
				nAttacker1.team = 1;
				place.SetAttacker(nAttacker1);
			}
			else if(team == 2)
			{
				AttackerScript nAttacker2 = (AttackerScript)Instantiate(attacker2,new Vector3 (place.transform.position.x,0.3f,place.transform.position.z),Quaternion.identity);
				nAttacker2.team = 2;
				place.SetAttacker(nAttacker2);
				compAttackers.Add(place);
			}
			else
			{
				Debug.Log ("Invalid attacker team");
			}
		}
	}

	void BoardVariation1(PlaceScript place, int x, int z)
	{
		if(z == 0 && x == 0){ CreateTiles(place,x,z,true,false,true,1); }		// player 1
		if(z == 1 && x == 0){ CreateTiles(place,x,z,true,false,true,1); }		// player 1
		if(z == 3 && x == 3){ CreateTiles(place,x,z,true,false,true,1); }		// player 1

		if(z == 2 && x == 2){ CreateTiles(place,x,z,true,false,true,2); }		// player 2
		if(z == 4 && x == 5){ CreateTiles(place,x,z,true,false,true,2); }		// player 2
		if(z == 5 && x == 5){ CreateTiles(place,x,z,true,false,true,2); } 	// player 2
		
		
		if(z == 1 && x == 4){ CreateTiles(place,x,z,false,false,false,0); }
		if(z == 2 && x == 3){ CreateTiles(place,x,z,false,false,false,0); }
		if(z == 3 && x == 2){ CreateTiles(place,x,z,false,false,false,0); }
		if(z == 4 && x == 1){ CreateTiles(place,x,z,false,false,false,0); }
	}

	void CreatePlayerHands()
	{
		for (int i = 0; i < playerHands.Length; i++)
		{
			PlaceScript placeTile = (PlaceScript)Instantiate (placement, new Vector3(-2f, 0, i * boardSpacing), Quaternion.identity);
			placeTile.name = "Player Hand 1";
			placeTile.playerHand = 1;

			if(i > 2)
			{
				placeTile.gameObject.transform.position = new Vector3(7f, 0, i * boardSpacing);
				placeTile.playerHand = 2;
				placeTile.name = "Player Hand 2";

			}

			TileScript nTile = tileDeck.Dequeue();
			nTile.transform.position = new Vector3(placeTile.transform.position.x*boardSpacing,pieceOffset,placeTile.transform.position.z*boardSpacing);
			nTile.transform.parent = transform.GetChild(2).transform;
			nTile.gameObject.SetActive(true);
			placeTile.SetTile(nTile);

			placeTile.transform.parent = transform.GetChild (0).transform;
			playerHands[i] = placeTile;
		}
	}

	public PlaceScript[,] GetBoardArray ()
	{
		return boardArray;
	}


	void autoDrawTiles()
	{
		for(int i = 0; i < playerHands.Length; i++)
		{
			if(!playerHands[i].GetTile())
			{
				Debug.Log ("GAME ===> Drawing new tiles into hands.");
				if(tileDeck.Count > 0){
					TileScript nTile = tileDeck.Dequeue();
					nTile.transform.position = new Vector3(playerHands[i].transform.position.x*boardSpacing,pieceOffset,playerHands[i].transform.position.z*boardSpacing);
					nTile.transform.parent = transform.GetChild(2).transform;
					nTile.gameObject.SetActive(true);
					playerHands[i].SetTile(nTile);
				}
			}	
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(turns % 2 == 0)	
			player = 2;
		else
			player = 1;

		if(moves < 1)
		{
			turns++;
			pc.currState = PlayerStates.endTurn;
			moves = 2;
		}

		CheckAttackersOnBase();

	}

	void CheckAttackersOnBase(){
		for(int i =0; i < playerBaseTiles.Count; i++)
		{
			if(playerBaseTiles[i].GetAttacker())
			{
				if(playerBaseTiles[i].GetAttacker().team == 2)
				{
					p2Count++;
				}
				else
				{
					p2Count = 0;
				}
			}
			else
			{
				p2Count = 0;
			}
		}
		for(int i = 0; i < opponentBaseTiles.Count; i++)
		{
			if(opponentBaseTiles[i].GetAttacker())
			{
				if(opponentBaseTiles[i].GetAttacker().team == 1)
				{
					p1Count++;
				}
				else
				{
					p1Count = 0;
				}
			}
			else
			{
				p1Count = 0;
			}
		}
		if(p2Count > 3)
			p2win = true;
		if(p1Count > 3)
			p1win = true;


	}


	public void ShowPlaceableTiles(){
		for (int xu = 0;xu < boardWidth; xu++)
		{
			for (int zy = 0; zy < boardHeight; zy++)
			{

				if(boardArray[xu, zy].GetAttacker())
				{
					boardArray[xu, zy].canMove = false;
					boardArray[xu, zy].GetComponent<Renderer>().material = place;

					if(player == boardArray[xu, zy].GetAttacker().team)
					{
						if(xu<boardWidth-1)
						{
							if(!boardArray[xu+1, zy].GetTile())
							{
								boardArray[xu+1, zy].GetComponent<Renderer>().material = s_place;
								boardArray[xu+1, zy].canBePlaced = true;
							}
						}
						if(xu > 0)
						{
							if(!boardArray[xu-1, zy].GetTile())
							{
								boardArray[xu-1, zy].GetComponent<Renderer>().material = s_place;
								boardArray[xu-1, zy].canBePlaced = true;
							}
						}
						if(zy<boardHeight-1)
						{
							if(!boardArray[xu, zy+1].GetTile())
							{
								boardArray[xu, zy+1].GetComponent<Renderer>().material = s_place;
								boardArray[xu, zy+1].canBePlaced = true;
							}
						}
						if(zy > 0)
						{
							if(!boardArray[xu, zy-1].GetTile())
							{
								boardArray[xu, zy-1].GetComponent<Renderer>().material = s_place;
								boardArray[xu, zy-1].canBePlaced = true;
							}
						}	

					}
				}
			}
		}
	}

	public bool ShowPossibleAttackerMove(PlaceScript place)
	{
		bool canMoveSomething = false;
		for (var x = 0; x < boardWidth; x++)
		{
			for (var z = 0; z < boardHeight; z++)
			{
				if (Mathf.Abs(boardArray[x, z].xPos - place.xPos) + Mathf.Abs(boardArray[x, z].zPos - place.zPos) <= 1)
				{
					if(boardArray[x, z].GetTile() && !boardArray[x, z].GetAttacker())
					{
						if(boardArray[x, z].GetTile().isWalkable)
						{
							if(boardArray[x, z].GetTile().name == "Walk Tile")
								boardArray[x, z].GetTile().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = s_Walk;
							if (boardArray[x, z].GetTile().name == "Base Team 1")
								boardArray[x, z].GetTile().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = s_B1;
							if (boardArray[x, z].GetTile().name == "Base Team 2")
								boardArray[x, z].GetTile().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = s_B2;
							boardArray[x, z].canMove = true;
							canMoveSomething = true;
							availableTiles.Add(boardArray[x, z]);
						}
					}
				}
			}
		}
		return canMoveSomething;
	}

	public void ShowDestroyableTiles()
	{
		for (int x = 0;x < boardWidth; x++)
		{
			for (int z = 0; z < boardHeight; z++)
			{
				if(boardArray[x, z].GetAttacker())
				{	
					if(player == boardArray[x, z].GetAttacker().team)
					{
						if(x<boardWidth-1)
						{
							if(boardArray[x+1, z].GetTile() && boardArray[x+1, z].GetTile().team == 0 && !boardArray[x+1, z].GetAttacker())
							{
								if(boardArray[x+1, z].GetTile().isWalkable)
									boardArray[x+1, z].canBeDestroyed = true;

								if(!boardArray[x+1, z].GetTile().isWalkable && moves > 1)
									boardArray[x+1, z].canBeDestroyed = true;
							}
						}
						if(x > 0)
						{
							if(boardArray[x-1, z].GetTile() && boardArray[x-1, z].GetTile().team == 0 && !boardArray[x-1, z].GetAttacker())
							{
								if(boardArray[x-1, z].GetTile().isWalkable)
									boardArray[x-1, z].canBeDestroyed = true;

								if(!boardArray[x-1, z].GetTile().isWalkable && moves > 1)
									boardArray[x-1, z].canBeDestroyed = true;
							}
						}
						if(z<boardHeight-1)
						{
							if(boardArray[x, z+1].GetTile() && boardArray[x, z+1].GetTile().team == 0 && !boardArray[x, z+1].GetAttacker())
							{
								if(boardArray[x, z+1].GetTile().isWalkable)
									boardArray[x, z+1].canBeDestroyed = true;

								if(!boardArray[x, z+1].GetTile().isWalkable && moves > 1)
									boardArray[x, z+1].canBeDestroyed = true;
							}
						}
						if(z > 0)
						{
							if(boardArray[x, z-1].GetTile() && boardArray[x, z-1].GetTile().team == 0 && !boardArray[x, z-1].GetAttacker())
							{
								if(boardArray[x, z-1].GetTile().isWalkable)
									boardArray[x, z-1].canBeDestroyed = true;

								if(!boardArray[x, z-1].GetTile().isWalkable && moves > 1)
									boardArray[x, z-1].canBeDestroyed = true;
							}
						}	
						
					}
				}
			}
		}
	}

	public void clearBoard()
	{
		availableTiles.Clear();
		for (int x = 0; x < boardWidth; x++)
		{
			for (int z = 0; z < boardHeight; z++)
			{
				if(boardArray[x, z].GetTile())
				{
					if(boardArray[x, z].GetTile().name == "Walk Tile")
						boardArray[x, z].GetTile().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = walk;
					if(boardArray[x, z].GetTile().name == "Block Tile")
						boardArray[x, z].GetTile().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = block;
					if (boardArray[x, z].GetTile().name == "Base Team 1")
						boardArray[x, z].GetTile().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = base1;
					if (boardArray[x, z].GetTile().name == "Base Team 2")
						boardArray[x, z].GetTile().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = base2;
				}
				boardArray[x, z].GetComponent<Renderer>().material = place;
				boardArray[x, z].canBePlaced = false;
				boardArray[x, z].canMove = false;
				boardArray[x, z].canBeDestroyed = false;
			}
		}
		autoDrawTiles();
		//CheckAttackersOnBase();
	}
	


}
