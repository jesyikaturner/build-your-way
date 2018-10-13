using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class old_BoardManager: MonoBehaviour {
	/*
	public PlaceScript placement; //tiles
	public TileScript tile; //pieces
	public AttackerScript attacker;


	//Attacker models
	public AttackerScript attacker_player1;
	public AttackerScript attacker_player2;

	public int boardWidth = 6;
	public int boardHeight = 6;
	public float boardSpacing = 1.1f; // the distance between the tiles
	public float pieceOffset = 0.1f; // hight above the tile that pieces rest
	
	public Material unselected;
	public Material tileWalk;
	public Material tileBlock;
	public Material player1Texture;
	public Material player2Texture;

	//Selection Materials
	public Material selected;
	public Material selectedTileWalk;
	public Material baseTile_p1;
	public Material baseTile_p2;
	public Material selctedBaseTile_p1;
	public Material selctedBaseTile_p2;

	public Material originalMat; // leave blank;
	
	// i want to put all the materials into an array. itll make it look neater.
	
	public PlaceScript selectedTile; //--> needed for the player hand.
	public PlaceScript selectedAttacker; //--> I think this needs to be placescript too...
	
	public PlaceScript[,] boardArray;
	public List<PlaceScript> handTiles;

	public Vector3[] attackerPos;
	public Vector2[] startPiecePos;

	public bool placeBlock = false;
	public bool moveAttacker = false;
	public bool destroyBlock = false;
	public bool canPlace = false;
	public bool canBreak = false;
	
	public int moves = 2;
	public int turns = 1;

	public int player = 0;

	public bool isPlaying = true;


	public bool p1_winGame = false;
	public bool p2_winGame = false;

	public bool p1Check_1 = false;
	public bool p1Check_2 = false;
	public bool p1Check_3 = false;
	
	public bool p2Check_1 = false;
	public bool p2Check_2 = false;
	public bool p2Check_3 = false;

	// Use this for initialization
	void Start () {
		boardArray = new PlaceScript[boardWidth,boardHeight];
		handTiles = new List<PlaceScript>(6);
		attackerPos = new Vector3[3];

		player = 1;
		//attackerPos = new Vector2[6];


		for (int x = 0; x < boardWidth; x++){
			for (int z = 0; z<boardHeight; z++){
				//creating the placetiles.
				PlaceScript placeTile = (PlaceScript)Instantiate (placement, new Vector3(x * boardSpacing, 0, z * boardSpacing), Quaternion.identity);
				placeTile.renderer.material = unselected;

				//creating the attackers and placing them on a base tile.
				//player 1
				if(z==0 && x ==0){
					TileScript newTile = (TileScript)Instantiate(tile,new Vector3(x* boardSpacing,pieceOffset,z* boardSpacing),Quaternion.identity);
					newTile.renderer.material = baseTile_p1;
					newTile.isBase = true;
					newTile.teamBase = 1;
					placeTile.tile = newTile;
					
					AttackerScript newAttacker = (AttackerScript) Instantiate (attacker_player1, new Vector3(x * boardSpacing, .3f, z * boardSpacing), Quaternion.identity);
					newAttacker.renderer.material = player1Texture;
					newAttacker.team = 1;
					placeTile.attacker = newAttacker;
				}
				if(z==1 && x ==0){
					TileScript newTile = (TileScript)Instantiate(tile,new Vector3(x* boardSpacing,pieceOffset,z* boardSpacing),Quaternion.identity);
					newTile.renderer.material = baseTile_p1;
					newTile.isBase = true;
					newTile.teamBase = 1;
					placeTile.tile = newTile;
					
					AttackerScript newAttacker = (AttackerScript) Instantiate (attacker_player1, new Vector3(x * boardSpacing, .3f, z * boardSpacing), Quaternion.identity);
					newAttacker.renderer.material = player1Texture;
					newAttacker.team = 1;
					placeTile.attacker = newAttacker;
				}

				if(z==3 && x == 3){
					TileScript newTile = (TileScript)Instantiate(tile,new Vector3(x* boardSpacing,pieceOffset,z* boardSpacing),Quaternion.identity);
					newTile.renderer.material = baseTile_p1;
					newTile.isBase = true;
					newTile.teamBase = 1;
					placeTile.tile = newTile;
					
					AttackerScript newAttacker = (AttackerScript) Instantiate (attacker_player1, new Vector3(x * boardSpacing, .3f, z * boardSpacing), Quaternion.identity);
					newAttacker.renderer.material = player1Texture;
					newAttacker.team = 1;
					placeTile.attacker = newAttacker;
				}

				//player 2
				if(z==2 && x == 2){
					TileScript newTile = (TileScript)Instantiate(tile,new Vector3(x* boardSpacing,pieceOffset,z* boardSpacing),Quaternion.identity);
					newTile.renderer.material = baseTile_p2;
					newTile.isBase = true;
					newTile.teamBase = 2;
					placeTile.tile = newTile;
					
					AttackerScript newAttacker = (AttackerScript) Instantiate (attacker_player2, new Vector3(x * boardSpacing, .3f, z * boardSpacing), Quaternion.identity);
					newAttacker.renderer.material = player2Texture;
					newAttacker.team = 2;
					placeTile.attacker = newAttacker;
				}
				if(z==4 && x == 5){
					TileScript newTile = (TileScript)Instantiate(tile,new Vector3(x* boardSpacing,pieceOffset,z* boardSpacing),Quaternion.identity);
					newTile.renderer.material = baseTile_p2;
					newTile.isBase = true;
					newTile.teamBase = 2;
					placeTile.tile = newTile;
					
					AttackerScript newAttacker = (AttackerScript) Instantiate (attacker_player2, new Vector3(x * boardSpacing, .3f, z * boardSpacing), Quaternion.identity);
					newAttacker.renderer.material = player2Texture;
					newAttacker.team = 2;
					placeTile.attacker = newAttacker;
				}
				if(z==5 && x == 5){
					TileScript newTile = (TileScript)Instantiate(tile,new Vector3(x* boardSpacing,pieceOffset,z* boardSpacing),Quaternion.identity);
					newTile.renderer.material = baseTile_p2;
					newTile.isBase = true;
					newTile.teamBase = 2;
					placeTile.tile = newTile;
					
					AttackerScript newAttacker = (AttackerScript) Instantiate (attacker_player2, new Vector3(x * boardSpacing, .3f, z * boardSpacing), Quaternion.identity);
					newAttacker.renderer.material = player2Texture;
					newAttacker.team = 2;
					placeTile.attacker = newAttacker;
				}

				//placing blocks
				if(z==3&&x==2){
					TileScript newTile = (TileScript)Instantiate(tile,new Vector3(x* boardSpacing,pieceOffset,z* boardSpacing),Quaternion.identity);
					newTile.renderer.material = tileBlock;
					newTile.isWalkable = false;
					placeTile.tile = newTile;
				}
				if(z==4&&x==1){
					TileScript newTile = (TileScript)Instantiate(tile,new Vector3(x* boardSpacing,pieceOffset,z* boardSpacing),Quaternion.identity);
					newTile.renderer.material = tileBlock;
					newTile.isWalkable = false;
					placeTile.tile = newTile;
				}
				if(z==2&&x==3){
					TileScript newTile = (TileScript)Instantiate(tile,new Vector3(x* boardSpacing,pieceOffset,z* boardSpacing),Quaternion.identity);
					newTile.renderer.material = tileBlock;
					newTile.isWalkable = false;
					placeTile.tile = newTile;
				}
				if(z==1&&x==4){
					TileScript newTile = (TileScript)Instantiate(tile,new Vector3(x* boardSpacing,pieceOffset,z* boardSpacing),Quaternion.identity);
					newTile.renderer.material = tileBlock;
					newTile.isWalkable = false;
					placeTile.tile = newTile;
				}


				placeTile.xPos = x;
				placeTile.zPos = z;
				boardArray[x, z] = placeTile;
			}
		}

		for (int i = 0; i < 3; i++){
			PlaceScript placeTile = (PlaceScript)Instantiate (placement, new Vector3(-2.2f, 0, i * boardSpacing), Quaternion.identity);
			placeTile.isP1Hand = true;
			placeTile.renderer.material = unselected;
			int random = Random.Range(1,4);
			if(random == 1){
				TileScript newTile = (TileScript)Instantiate(tile,new Vector3(-2.2f,pieceOffset,i* boardSpacing),Quaternion.identity);
				newTile.isWalkable = false;
				newTile.renderer.material = tileBlock;
				placeTile.tile = newTile;
			}else{
				TileScript newTile = (TileScript)Instantiate(tile,new Vector3(-2.2f,pieceOffset,i* boardSpacing),Quaternion.identity);
				newTile.isWalkable = true;
				newTile.renderer.material = tileWalk;
				placeTile.tile = newTile;
			}
			handTiles.Add (placeTile);
			//handTiles[i] = placeTile;
		}

		for(int o = 3; o < 6; o++){
			PlaceScript placeTile = (PlaceScript)Instantiate (placement, new Vector3(7.7f, 0, o * boardSpacing), Quaternion.identity);
			placeTile.isP2Hand = true;
			placeTile.renderer.material = unselected;
			int random = Random.Range(1,4);
			if(random == 1){
				TileScript newTile = (TileScript)Instantiate(tile,new Vector3(7.7f,pieceOffset,o* boardSpacing),Quaternion.identity);
				newTile.isWalkable = false;
				newTile.renderer.material = tileBlock;
				placeTile.tile = newTile;
			}else{
				TileScript newTile = (TileScript)Instantiate(tile,new Vector3(7.7f,pieceOffset,o* boardSpacing),Quaternion.identity);
				newTile.isWalkable = true;
				newTile.renderer.material = tileWalk;
				placeTile.tile = newTile;
			}
			handTiles.Add (placeTile);
		}
		
	}

	void OnGUI(){
		
		if(GUI.Button(new Rect(50,150,100,50),"Destroy Tile")){
			if(!selectedAttacker && !selectedTile){
				if(destroyBlock)
					destroyBlock = false;
				else
					destroyBlock = true;
			}
		}

		//GUI.Label(new Rect(160,Screen.height-75,150,50),"Moves: " +moves);
			
	}
	
	// Update is called once per frame
	void Update () {
		if(!destroyBlock && !placeBlock && !moveAttacker)	clearBoard();

		if(p2Check_1 && p2Check_2 && p2Check_3)	p2_winGame = true;
		if(p1Check_1 && p1Check_2 && p1Check_3)	p1_winGame = true;

		if(destroyBlock)	showBreakableTiles();
		if(placeBlock)		showPlaceableTiles();


		if(turns % 2 == 0)	
			player = 2;
		else
			player = 1;

		if(moves == 0){
			turns++;
			moves = 2;
		}

	}

	// checks the attackers on the base tiles. if there are 3 attackera on the opponents base tiles then the player wins
	public void checkAttackersOnBase(){
		for (int x = 0;x<boardWidth;x++){
			for (int z = 0;z<boardHeight;z++){
				if(boardArray[0,0].attacker){
					if(boardArray[0,0].attacker.team == 2)
						if(!p2Check_1)	p2Check_1 = true;
				}else{
					if(p2Check_1)	p2Check_1 = false;
				}
				if(boardArray[0,1].attacker){
					if(boardArray[0,1].attacker.team == 2)
						if(!p2Check_2)	p2Check_2 = true;
				}else{
					if(p2Check_2)	p2Check_2 = false;
				}
				if(boardArray[3,3].attacker){
					if(boardArray[3,3].attacker.team == 2)
						if(!p2Check_3)	p2Check_3 = true;
				}else{
					if(p2Check_3)	p2Check_3 = false;
				}
				
				if(boardArray[5,5].attacker){
					if(boardArray[5,5].attacker.team == 1)
						if(!p1Check_1)	p1Check_1 = true;
				}else{
					if(p1Check_1)	p1Check_1 = false;
				}
				if(boardArray[5,4].attacker){
					if(boardArray[5,4].attacker.team == 1)
						if(!p1Check_2)	p1Check_2 = true;
				}else{
					if(p1Check_2)	p1Check_2 = false;
				}
				if(boardArray[2,2].attacker){
					if(boardArray[2,2].attacker.team == 1)
						if(!p1Check_3)	p1Check_3 = true;
				}else{
					if(p1Check_3)	p1Check_3 = false;
				}
			}
		}
	}


	// auto draws tiles when player moves the tile onto the board.
	public void autoDrawTiles(){
		for(int i = 0; i < 6; i++){
			if(!handTiles[i].tile){
				//print("is working");
				int random = Random.Range(1,4);
				TileScript newTile = (TileScript)Instantiate(tile, handTiles[i].transform.position, Quaternion.identity);
				if(random == 1){
					newTile.isWalkable = false;
					newTile.renderer.material = tileBlock;
				}else{
					newTile.renderer.material = tileWalk;
				}
				newTile.transform.position = new Vector3(handTiles[i].transform.position.x,pieceOffset,handTiles[i].transform.position.z);
				handTiles[i].tile = newTile;
			}
			
		}
	}

	// shows breakable tiles
	public void showBreakableTiles(){
		for (int xu = 0;xu < boardWidth; xu++){
			for (int zy = 0; zy < boardHeight; zy++){
				if(boardArray[xu, zy].attacker){
					if (player == 1){
						if(boardArray[xu, zy].attacker.team == 1){
							if(xu<boardWidth-1){
								if(boardArray[xu+1, zy].tile && !boardArray[xu+1, zy].tile.isBase){
									if(boardArray[xu+1, zy].tile.isWalkable){
										if(!boardArray[xu+1, zy].attacker){
											boardArray[xu+1, zy].tile.renderer.material = selected;
											boardArray[xu+1, zy].canBeBroken = true;
										}
									}else{
										if(moves == 2){
											boardArray[xu+1, zy].tile.renderer.material = selected;
											boardArray[xu+1, zy].canBeBroken = true;
										}
									}
								}
							}
							if(xu > 0){
								if(boardArray[xu-1, zy].tile && !boardArray[xu-1, zy].tile.isBase){
									if(boardArray[xu-1, zy].tile.isWalkable){
										if(!boardArray[xu-1, zy].attacker){
											boardArray[xu-1, zy].tile.renderer.material = selected;
											boardArray[xu-1, zy].canBeBroken = true;
										}
									}else{
										if(moves == 2){
											boardArray[xu-1, zy].tile.renderer.material = selected;
											boardArray[xu-1, zy].canBeBroken = true;
										}
									}
								}
							}
							if(zy<boardHeight-1){
								if(boardArray[xu, zy+1].tile && !boardArray[xu, zy+1].tile.isBase){
									if(boardArray[xu, zy+1].tile.isWalkable){
										if(!boardArray[xu, zy+1].attacker){
											boardArray[xu, zy+1].tile.renderer.material = selected;
											boardArray[xu, zy+1].canBeBroken = true;
										}
									}else{
										if(moves == 2){
											boardArray[xu, zy+1].tile.renderer.material = selected;
											boardArray[xu, zy+1].canBeBroken = true;
										}
									}
								}
							}
							if(zy > 0){
								if(boardArray[xu, zy-1].tile && !boardArray[xu, zy-1].tile.isBase){
									if(boardArray[xu, zy-1].tile.isWalkable){
										if(!boardArray[xu, zy-1].attacker){
											boardArray[xu, zy-1].tile.renderer.material = selected;
											boardArray[xu, zy-1].canBeBroken = true;
										}
									}else{
										if(moves == 2){
											boardArray[xu, zy-1].tile.renderer.material = selected;
											boardArray[xu, zy-1].canBeBroken = true;
										}
									}
								}
							}
							canBreak = true;
						}
					}
					if (player == 2){
						if(boardArray[xu, zy].attacker.team == 2){
							if(xu<boardWidth-1){
								if(boardArray[xu+1, zy].tile && !boardArray[xu+1, zy].tile.isBase){
									if(boardArray[xu+1, zy].tile.isWalkable){
										if(!boardArray[xu+1, zy].attacker){
											boardArray[xu+1, zy].tile.renderer.material = selected;
											boardArray[xu+1, zy].canBeBroken = true;
										}
									}else{
										if(moves == 2){
											boardArray[xu+1, zy].tile.renderer.material = selected;
											boardArray[xu+1, zy].canBeBroken = true;
										}
									}
								}
							}
							if(xu > 0){
								if(boardArray[xu-1, zy].tile && !boardArray[xu-1, zy].tile.isBase){
									if(boardArray[xu-1, zy].tile.isWalkable){
										if(!boardArray[xu-1, zy].attacker){
											boardArray[xu-1, zy].tile.renderer.material = selected;
											boardArray[xu-1, zy].canBeBroken = true;
										}
									}else{
										if(moves == 2){
											boardArray[xu-1, zy].tile.renderer.material = selected;
											boardArray[xu-1, zy].canBeBroken = true;
										}
									}
								}
							}
							if(zy<boardHeight-1){
								if(boardArray[xu, zy+1].tile && !boardArray[xu, zy+1].tile.isBase){
									if(boardArray[xu, zy+1].tile.isWalkable){
										if(!boardArray[xu, zy+1].attacker){
											boardArray[xu, zy+1].tile.renderer.material = selected;
											boardArray[xu, zy+1].canBeBroken = true;
										}
									}else{
										if(moves == 2){
											boardArray[xu, zy+1].tile.renderer.material = selected;
											boardArray[xu, zy+1].canBeBroken = true;
										}
									}
								}
							}
							if(zy > 0){
								if(boardArray[xu, zy-1].tile && !boardArray[xu, zy-1].tile.isBase){
									if(boardArray[xu, zy-1].tile.isWalkable){
										if(!boardArray[xu, zy-1].attacker){
											boardArray[xu, zy-1].tile.renderer.material = selected;
											boardArray[xu, zy-1].canBeBroken = true;
										}
									}else{
										if(moves == 2){
											boardArray[xu, zy-1].tile.renderer.material = selected;
											boardArray[xu, zy-1].canBeBroken = true;
										}
									}
								}
							}
							canBreak = true;
						}
					}
				}
			}
		}
	}

	public void showPlaceableTiles(){
		for (int xu = 0;xu < boardWidth; xu++){
			for (int zy = 0; zy < boardHeight; zy++){
				if(boardArray[xu, zy].attacker){
					if(player == 1){
						if(boardArray[xu, zy].attacker.team == 1){
							if(xu<boardWidth-1){
								if(!boardArray[xu+1, zy].tile){
									boardArray[xu+1, zy].renderer.material = selected;
									boardArray[xu+1, zy].canBePlaced = true;
									canPlace = true;
								}
							}
							if(xu > 0){
								if(!boardArray[xu-1, zy].tile){
									boardArray[xu-1, zy].renderer.material = selected;
									boardArray[xu-1, zy].canBePlaced = true;
									canPlace = true;
								}
							}
							if(zy<boardHeight-1){
								if(!boardArray[xu, zy+1].tile){
									boardArray[xu, zy+1].renderer.material = selected;
									boardArray[xu, zy+1].canBePlaced = true;
									canPlace = true;
								}
							}
							if(zy > 0){
								if(!boardArray[xu, zy-1].tile){
									boardArray[xu, zy-1].renderer.material = selected;
									boardArray[xu, zy-1].canBePlaced = true;
									canPlace = true;
								}
							}	
						}
					}
					if (player == 2){
						if(boardArray[xu, zy].attacker.team == 2){
							if(xu<boardWidth-1){
								if(!boardArray[xu+1, zy].tile){
									boardArray[xu+1, zy].renderer.material = selected;
									boardArray[xu+1, zy].canBePlaced = true;
									canPlace = true;
								}
							}
							if(xu > 0){
								if(!boardArray[xu-1, zy].tile){
									boardArray[xu-1, zy].renderer.material = selected;
									boardArray[xu-1, zy].canBePlaced = true;
									canPlace = true;
								}
							}
							if(zy<boardHeight-1){
								if(!boardArray[xu, zy+1].tile){
									boardArray[xu, zy+1].renderer.material = selected;
									boardArray[xu, zy+1].canBePlaced = true;
									canPlace = true;
								}
							}
							if(zy > 0){
								if(!boardArray[xu, zy-1].tile){
									boardArray[xu, zy-1].renderer.material = selected;
									boardArray[xu, zy-1].canBePlaced = true;
									canPlace = true;
								}
							}	
						}
					}
				}
			}
		}
	}

	void clearBoard(){
		checkAttackersOnBase();
		autoDrawTiles();
		
		for (int x4 = 0; x4 < boardWidth; x4++){
			for (int z4 = 0; z4 < boardHeight; z4++){
				//there is a tile
				if(boardArray[x4, z4].tile){
					//the tile has an attacker. itll set the tile underneath the attacker to the tilewalk texture.
					if(boardArray[x4, z4].attacker)
						boardArray[x4, z4].tile.renderer.material = tileWalk;
					//underneath the tile's, the placement is set to unselected.
					boardArray[x4, z4].renderer.material = unselected;
					// if the tile is a base tile, itll set it back to its original texture.
					if(boardArray[x4, z4].tile.isBase && boardArray[x4, z4].tile.teamBase == 1)
						boardArray[x4, z4].tile.renderer.material = baseTile_p1;
					if(boardArray[x4, z4].tile.isBase && boardArray[x4, z4].tile.teamBase == 2)
						boardArray[x4, z4].tile.renderer.material = baseTile_p2;
					if(!boardArray[x4,z4].tile.isWalkable)
						boardArray[x4, z4].tile.renderer.material = tileBlock;
					if(boardArray[x4,z4].tile.isWalkable && !boardArray[x4,z4].tile.isBase)
						boardArray[x4, z4].tile.renderer.material = tileWalk;
				}else{
					boardArray[x4, z4].renderer.material = unselected;
					boardArray[x4, z4].canBePlaced = false;
					boardArray[x4, z4].canBeBroken = true;
				}
			}
		}
	}


*/


}
