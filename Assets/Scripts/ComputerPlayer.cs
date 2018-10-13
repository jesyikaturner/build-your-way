using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AIState{
	wait = 0,
	placeTile = 1,
	destroyTile = 2,
	movePiece = 3,
}

public class ComputerPlayer : MonoBehaviour {

	BoardManager bm;
	public AIState currentState = AIState.wait;
	public int rand, number;
	public PlaceScript nTile;

	public PlaceScript compSelectPlace, newCompSelectPlace;

	void Start () 
	{
		bm = transform.GetComponent<BoardManager>();
	}

	void Update () {
		if(bm.player==2){
			switch(currentState){

			case AIState.wait:
				currentState = AIState.placeTile;

				break;

			case AIState.placeTile:
				rand = Random.Range(2,6);
				if(!compSelectPlace){
					GrabHandTile(bm.playerHands[rand]);
				}else{
					if(SelectNewPlace()){
						MoveTile();
						currentState = AIState.wait;
					}else{
						compSelectPlace = null;
						currentState = AIState.destroyTile;
					}
				}
				break;

			case AIState.destroyTile:
				if(SelectBlock()){
					DestroyBlock();
					currentState = AIState.wait;
				}
				else
					currentState = AIState.wait;
				break;

			case AIState.movePiece:
				Debug.Log ("Computer is moving an attacker.");
				if(SelectAttacker())
				{
					if(SelectNewTile())
					{
						MoveAttacker(compSelectPlace,newCompSelectPlace);
						currentState = AIState.wait;
					}
					//Debug.Break();
				}
				else
					currentState = AIState.wait;
				break;

			default:
				Debug.Log ("AI machine is wrong");
				break;
			}
		}else{
			currentState = AIState.wait;
			compSelectPlace = null;
			newCompSelectPlace = null;
		}
	}

	void GrabHandTile(PlaceScript handTile)
	{
		compSelectPlace = handTile;
		ShowPlaceableTiles();
		Debug.Log ("Computer selected hand tile.");
	}

	bool SelectNewPlace()
	{
		Debug.Log ("Computer is attempting to select an empty place.");
		if(bm.availableTiles.Count > 0)
		{
			rand = Random.Range(0,bm.availableTiles.Count);
			if(!bm.availableTiles[rand].GetTile())
			{
				newCompSelectPlace = bm.availableTiles[rand];
				Debug.Log ("Computer selected empty place at " + newCompSelectPlace.xPos + ", " + newCompSelectPlace.zPos);
				return true;
			}

		}
		Debug.Log ("Computer failed to select an empty place.");
		return false;
	}
	 
	void MoveTile()
	{
		Debug.Log ("Computer is moving the tile to new location.");
		newCompSelectPlace.SetTile(compSelectPlace.GetTile());
		newCompSelectPlace.GetTile().transform.position = new Vector3 (newCompSelectPlace.transform.position.x,bm.pieceOffset,newCompSelectPlace.transform.position.z);
		compSelectPlace.SetTile(null);
		compSelectPlace = null;
		bm.moves--;
		bm.clearBoard();
		Debug.Log ("Computer has moved the tile to new location.");
	}

	bool SelectBlock()
	{
		bool canSelect = false;
		if(!compSelectPlace)
		{
			ShowDestroyableTiles();
			Debug.Log ("<color=yellow>COMP ===> Selecting a block tile.</color>");
			if(bm.availableTiles.Count > 0){
				rand = Random.Range(0,bm.availableTiles.Count);
				if(bm.availableTiles[rand].GetTile() && !bm.availableTiles[rand].GetTile().isWalkable)
				{
					compSelectPlace = bm.availableTiles[rand];
					Debug.Log ("<color=green>COMP ===> Selected a block tile.</color>");
					canSelect = true;
				}
			}
		}
		return canSelect;
	}
	void DestroyBlock()
	{
		Debug.Log ("Computer is destroying a block tile.");
		bm.tileDeck.Enqueue(compSelectPlace.GetTile());
		compSelectPlace.GetTile ().gameObject.SetActive(false);
		compSelectPlace.SetTile (null);
		bm.moves-=2;
		bm.clearBoard();
		compSelectPlace = null;
		Debug.Log ("Computer has destroyed a block tile.");
	}

	bool SelectAttacker()
	{
		bool hasPossible = false;
		if(!compSelectPlace)
		{
			for (int i = 0; i < bm.compAttackers.Count; i++)
			{
				if(ShowPossibleAttackerMove(bm.compAttackers[i]))
				{
					compSelectPlace = bm.compAttackers[i];
					bm.compAttackers.RemoveAt(i);
					return true;
				}
			}
		}
		return hasPossible;
	}

	bool SelectNewTile()
	{
		bool selectedNewTile = false;
		if(bm.availableTiles.Count > 0)
		{
			if(bm.availableTiles.Count > 1)
			{
				for(int i = 0; i < bm.availableTiles.Count; i++)
				{
					float temp = Vector3.Distance(bm.availableTiles[i].transform.position, bm.playerBaseTiles[i].transform.position);
					if(Vector3.Distance(bm.availableTiles[i].transform.position, bm.playerBaseTiles[i].transform.position) < temp)
					{
						newCompSelectPlace = bm.availableTiles[i];
					}

				}
			}
			else
				newCompSelectPlace = bm.availableTiles[0];
			selectedNewTile = true;
		}
		return selectedNewTile;

	}

	void MoveAttacker(PlaceScript currPlace, PlaceScript newPlace)
	{
		newPlace.SetAttacker(currPlace.GetAttacker());
		newPlace.GetAttacker().transform.position = new Vector3 (newPlace.transform.position.x,0.3f,newPlace.transform.position.z);
		newPlace.GetAttacker().gameObject.GetComponent<Renderer>().material.color = Color.white;
		bm.compAttackers.Add (newPlace);
		currPlace.SetAttacker(null);
		currPlace = null;
		bm.moves--;
		bm.clearBoard();
	}



	void ShowPlaceableTiles()
	{
		for (int x = 0;x < bm.boardWidth; x++)
		{
			for (int z = 0; z < bm.boardHeight; z++)
			{
				if(bm.GetBoardArray()[x, z].GetAttacker())
				{
					bm.GetBoardArray()[x, z].canMove = false;
					if(bm.player == bm.GetBoardArray()[x, z].GetAttacker().team)
					{
						if(x<bm.boardWidth-1)
						{
							if(!bm.GetBoardArray()[x+1, z].GetTile())
								bm.availableTiles.Add(bm.GetBoardArray()[x+1, z]);
						}
						if(x > 0)
						{
							if(!bm.GetBoardArray()[x-1, z].GetTile())
								bm.availableTiles.Add(bm.GetBoardArray()[x-1, z]);
						}
						if(z<bm.boardHeight-1)
						{
							if(!bm.GetBoardArray()[x, z+1].GetTile())
								bm.availableTiles.Add(bm.GetBoardArray()[x, z+1]);
						}
						if(z > 0)
						{
							if(!bm.GetBoardArray()[x, z-1].GetTile())
								bm.availableTiles.Add(bm.GetBoardArray()[x, z-1]);
							
						}	
					}
				}
			}
		}
	}

	void ShowDestroyableTiles()
	{
		for (int x = 0;x < bm.boardWidth; x++)
		{
			for (int z = 0; z < bm.boardHeight; z++)
			{
				
				if(bm.boardArray[x, z].GetAttacker())
				{
					bm.boardArray[x, z].canMove = false;
					if(bm.player == bm.boardArray[x, z].GetAttacker().team)
					{
						if(bm.boardArray[x,z].name != "Player Hand 1" || bm.boardArray[x,z].name != "Player Hand 2")
						{
							if(x<bm.boardWidth-1)
							{
								if(bm.boardArray[x+1, z].GetTile())
									bm.availableTiles.Add(bm.boardArray[x+1, z]);
							}
							if(x > 0)
							{
								if(bm.boardArray[x-1, z].GetTile())
									bm.availableTiles.Add(bm.boardArray[x-1, z]);
							}
							if(z<bm.boardHeight-1)
							{
								if(bm.boardArray[x, z+1].GetTile())
									bm.availableTiles.Add(bm.boardArray[x, z+1]);
							}
							if(z > 0)
							{
								if(bm.boardArray[x, z-1].GetTile())
									bm.availableTiles.Add(bm.boardArray[x, z-1]);
								
							}	
						}
					}
				}
			}
		}
	}

	bool ShowPossibleAttackerMove(PlaceScript place)
	{
		bool possibleMoves = false;
		for (var x = 0; x < bm.boardWidth; x++)
		{
			for (var z = 0; z < bm.boardHeight; z++)
			{
				if (Mathf.Abs(bm.boardArray[x, z].xPos - place.xPos) + Mathf.Abs(bm.boardArray[x, z].zPos - place.zPos) <= 1)
				{
					if(bm.boardArray[x, z].GetTile() && !bm.boardArray[x, z].GetAttacker())
					{
						if(bm.boardArray[x, z].GetTile().isWalkable)
						{
							bm.availableTiles.Add(bm.boardArray[x, z]);
							possibleMoves = true;
						}
					}
				}
			}
		}
		return possibleMoves;
	}
	
			/*
			switch(currentState){
			case AIState.wait:
				bool attackerMove = false;
				rand = Random.Range(0,5);
				if(rand <=2){
					Debug.Log ("Will move attacker");
					for(int i = 0; i < 3; i++){
						if(bm.ShowPossibleAttackerMove(bm.compAttackers[i])){
							attackerMove = true;
						}
					}
				}
				if(attackerMove){
					Debug.Log ("canMoveSomething");
					currentState = AIState.movePiece;

				}else{
					currentState = AIState.placeTile;
				}
				//currentState = AIState.placeTile;
				break;
			case AIState.placeTile:
				rand = Random.Range(2,6);
				if(!bm.selectedPlace){
					CompSelectTile(bm.playerHands[rand]);
				}else{
					CompMoveTile();
					currentState = AIState.wait;
				}

				break;
			case AIState.movePiece:
				if(!bm.selectedPlace){
					for(int x = 0; x < bm.compAttackers.Count; x++){
						for(int y = 0; y < bm.availableTiles.Count; y++){
							if(Mathf.Abs(bm.availableTiles[y].xPos - bm.compAttackers[x].xPos) + Mathf.Abs(bm.availableTiles[y].zPos - bm.compAttackers[x].zPos) <= 1){
								number = x;
								nTile = bm.availableTiles[y];
								break;
							}
						}
					}
					bm.selectedPlace = bm.compAttackers[number];
					bm.compAttackers.RemoveAt(number);
				}else{
					bm.compAttackers.Add (nTile);
					CompMoveAttacker(bm.selectedPlace,nTile);
					Debug.Log ("moved the attacker");
					currentState = AIState.wait;
				}

				break;

			default:
				Debug.Log ("AI state is fucked.");
				break;



			}
		}else{
			currentState = AIState.wait;
		}*/


	void CompMoveAttacker(PlaceScript currPlace, PlaceScript newPlace){
		newPlace.SetAttacker(currPlace.GetAttacker());
		newPlace.GetAttacker().transform.position = new Vector3 (newPlace.transform.position.x,0.3f,newPlace.transform.position.z);
		newPlace.GetAttacker().gameObject.GetComponent<Renderer>().material.color = Color.white;
		bm.selectedPlace.SetAttacker(null);
		bm.selectedPlace = null;
		nTile = null;
		bm.moves--;
		bm.clearBoard();
	}
	
	//if attacker piece cant move, look in hand for walkable tile. if there's no walkable tile, place block tile.
	//if cant place tile, destroy a block tile

	void CompSelectTile(PlaceScript place){
		Debug.Log ("Selected Tile");
		bm.selectedPlace = place;
		bm.selectedPlace.GetTile().gameObject.GetComponent<Renderer>().material.color = Color.red;
		bm.ShowPlaceableTiles();
	}
	void CompMoveTile(){
		if(bm.availableTiles.Count > 0){
			rand = Random.Range(0,bm.availableTiles.Count);
			if(!bm.availableTiles[rand].GetTile()){
				PlaceScript newTile = bm.availableTiles[rand];
				newTile.SetTile(bm.selectedPlace.GetTile());
				newTile.GetTile().transform.position = new Vector3 (newTile.transform.position.x,bm.pieceOffset,newTile.transform.position.z);
				newTile.GetTile().gameObject.GetComponent<Renderer>().material.color = Color.white;
				bm.selectedPlace.SetTile(null);
				bm.selectedPlace = null;
				bm.moves--;
				bm.clearBoard();

			}
		}
	}

}
