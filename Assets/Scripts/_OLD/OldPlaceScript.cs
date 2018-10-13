using UnityEngine;
using System.Collections;

public class OldPlaceScript : MonoBehaviour {
    /*
	public int xPos, zPos;
	public int playerHand;
	private TileScript _tile;
	private AttackerScript _attacker;
	public bool canBePlaced, canMove, canBeDestroyed;

	public BoardManager bm;

	void Start(){
		bm = GameObject.Find ("BoardManager").GetComponent<BoardManager>();
	}

	public void SetTile(TileScript tile){	_tile = tile;	}
	public TileScript GetTile(){	return _tile;	}

	public void SetAttacker(AttackerScript attacker){	_attacker = attacker;	}
	public AttackerScript GetAttacker(){	return _attacker;	}

	/*
	void OnMouseDown(){

		bm.SelectAttacker(this);
		bm.SelectTile(this);

	}
	
	void Update() {

	}
			
			
			
			

	/*
	public int xPos;
	public int zPos;

	public TileScript tile;
	public AttackerScript attacker;
	
	public GameObject board;
	public BoardManager boardScript;
	
	public int xAPos;
	public int zAPos;
	
	public bool clearBoard = false;
	public bool showAttackers = false;
	public bool canBePlaced = false;
	
	public bool canBeBroken = false;

	// if the placement is a hand placement
	public bool isP1Hand = false;
	public bool isP2Hand = false;


	//private Material originalMat;
	// Use this for initialization
	void Start () {
		board = GameObject.Find("BoardManager");
		boardScript = board.GetComponent<BoardManager>();
	}
	
	void OnMouseDown(){
		if(boardScript.isPlaying){
			//placing blocks adjacent to the attackers
			if(!boardScript.selectedAttacker && !boardScript.destroyBlock){
				if(boardScript.player == 1){
					if(this.tile){
						if(this.isP1Hand){
							if(boardScript.selectedTile==null){
								boardScript.selectedTile = this;
								boardScript.originalMat = boardScript.selectedTile.tile.renderer.material;
								boardScript.selectedTile.tile.renderer.material = boardScript.selected;
								boardScript.placeBlock = true;
							}else{
								if(boardScript.selectedTile == this){
									boardScript.selectedTile.tile.renderer.material = boardScript.originalMat;
									boardScript.selectedTile = null;
									boardScript.placeBlock = false;
								}	
							}
						}
					}
				}
				if(boardScript.player == 2){
					if(this.tile){
						if(this.isP2Hand){
							if(boardScript.selectedTile==null){
								boardScript.selectedTile = this;
								boardScript.originalMat = boardScript.selectedTile.tile.renderer.material;
								boardScript.selectedTile.tile.renderer.material = boardScript.selected;
								boardScript.placeBlock = true;
							}else{
								if(boardScript.selectedTile == this){
									boardScript.selectedTile.tile.renderer.material = boardScript.originalMat;
									boardScript.selectedTile = null;
									boardScript.placeBlock = false;
								}	
							}
						}
					}
				}
			}

			if(boardScript.placeBlock){
				if(this.tile == null){
					if(boardScript.canPlace){
						if(this.canBePlaced){
							this.tile = boardScript.selectedTile.tile;
							boardScript.selectedTile.tile.renderer.material = boardScript.originalMat;
							boardScript.selectedTile.tile = null;
							this.tile.transform.position = new Vector3(transform.position.x,boardScript.pieceOffset,transform.position.z);
							boardScript.originalMat = null;
							boardScript.selectedTile = null;
							boardScript.moves--;
							boardScript.placeBlock = false;
						}
					}
				}
			}


			// moving the attackers
		if(!boardScript.destroyBlock && !boardScript.selectedTile){

			if(!boardScript.selectedAttacker){
				if(this.attacker){
					if(boardScript.player == 1){
						if(this.attacker.team == 1){
							boardScript.selectedAttacker = this;
							boardScript.moveAttacker = true;
						}
					}
					if(boardScript.player == 2){
						if(this.attacker.team == 2){
							boardScript.selectedAttacker = this;
							boardScript.moveAttacker = true;
						}
					}
					for (var x = 0; x < boardScript.boardWidth; x++){
						for (var z = 0; z < boardScript.boardHeight; z++){
							if (Mathf.Abs(boardScript.boardArray[x, z].xPos - xPos) + Mathf.Abs(boardScript.boardArray[x, z].zPos - zPos) <= 1){
								if(boardScript.boardArray[x, z].tile){
								   if(boardScript.boardArray[x, z].tile.isWalkable){
										if(boardScript.boardArray[x, z].tile.isBase){
											if(boardScript.boardArray[x, z].tile.teamBase == 1)
												boardScript.boardArray[x, z].tile.renderer.material = boardScript.selctedBaseTile_p1;
											if(boardScript.boardArray[x, z].tile.teamBase == 2)
												boardScript.boardArray[x, z].tile.renderer.material = boardScript.selctedBaseTile_p2;
											boardScript.boardArray[x, z].tile.canMove = true;
										}else{
											boardScript.boardArray[x, z].tile.renderer.material = boardScript.selectedTileWalk;
											boardScript.boardArray[x, z].tile.canMove = true;
										}
									}else{
										boardScript.boardArray[x, z].tile.canMove = false;
									}
								}
							}else{
								if(boardScript.boardArray[x, z].tile)
									boardScript.boardArray[x, z].tile.canMove = false;
							}
						}
					}
				}
			}else{
				if(boardScript.selectedAttacker == this){
					for (var x2 = 0; x2 < boardScript.boardWidth; x2++){
						for (var z2 = 0; z2 < boardScript.boardHeight; z2++){
							if(boardScript.boardArray[x2, z2].tile){
								if(boardScript.boardArray[x2, z2].tile.isWalkable && !boardScript.boardArray[x2, z2].tile.isBase){
									boardScript.boardArray[x2, z2].tile.renderer.material = boardScript.tileWalk;
									boardScript.boardArray[x2, z2].tile.canMove = false;
								}
							}
						}
					}
					boardScript.moveAttacker = false;
					boardScript.selectedAttacker = null;
				}else{
					if(!this.attacker){
						if(this.tile){
							if(this.tile.canMove && !this.isP1Hand && !this.isP2Hand){
								this.attacker = boardScript.selectedAttacker.attacker;
								boardScript.selectedAttacker.attacker = null;
								Vector3 targetPos = this.transform.position;
								attacker.transform.position = new Vector3(targetPos.x,.3f,targetPos.z);
								for (var x3 = 0; x3 < boardScript.boardWidth; x3++){
									for (var z3 = 0; z3 < boardScript.boardHeight; z3++){
										if(boardScript.boardArray[x3, z3].tile){
											if(boardScript.boardArray[x3, z3].tile.isWalkable && !boardScript.boardArray[x3, z3].tile.isBase){
												boardScript.boardArray[x3, z3].tile.renderer.material = boardScript.tileWalk;
												boardScript.boardArray[x3, z3].tile.canMove = false;
											}
										}
									}	
								}
								boardScript.moves--;
								boardScript.moveAttacker = false;
								boardScript.selectedAttacker = null;
							}
						}
					}

				}
			}
		}

			// destroy blocks
			if(boardScript.destroyBlock){
				if(tile != null){;
					if(this.attacker == null){
						if(this.canBeBroken){
							if(this.tile.isWalkable){
								Destroy(this.tile.gameObject);
								boardScript.moves--;
								boardScript.destroyBlock = false;
							}else if(!this.tile.isWalkable && boardScript.moves == 2){
								Destroy(this.tile.gameObject);
								boardScript.moves -=2;
								boardScript.destroyBlock = false;
							}
						}
					}
				}
			}
		}

	}

	// Update is called once per frame
	void Update () {

	}




*/


}
