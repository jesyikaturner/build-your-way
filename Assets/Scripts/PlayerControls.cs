using UnityEngine;
using System.Collections;


public enum PlayerStates
{
	startTurn = 0,
	playing = 1,
	updateBoard = 2,
	endTurn = 3,

}


public class PlayerControls : MonoBehaviour {

	BoardManager bm;
	public PlaceScript selectedPlace;
	public bool isDestroying;

	public PlayerStates currState = PlayerStates.startTurn;

	public float destroyTimer;

	// Use this for initialization
	void Start () 
	{
		bm = transform.GetComponent<BoardManager>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(currState)
		{
		case PlayerStates.startTurn:
			bm.ShowDestroyableTiles();
			currState = PlayerStates.playing;
			break;

		case PlayerStates.playing:
			if(bm.is1P)
			{
				if(bm.player == 1)
				{
					Controls();
				}
			}
			else
			{
				Controls();
			}
		
			break;

		case PlayerStates.updateBoard:
			bm.clearBoard();
			bm.ShowDestroyableTiles();
			currState = PlayerStates.playing;
			break;

		case PlayerStates.endTurn:
			bm.clearBoard();
			currState = PlayerStates.startTurn;
			break;

		default:
			Debug.Log("Player states is fucked");
			break;


		}

	}

	void Controls ()
	{
		RaycastHit hit;
		
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			
			// -- Tap: quick touch & release
			// ------------------------------------------------
			if (touch.phase == TouchPhase.Began && touch.tapCount == 1)
			{
				
				Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
				
				if (Physics.Raycast(ray,out hit))
				{
					if(hit.collider.gameObject.GetComponent<PlaceScript>())
					{
						PlaceScript temp = hit.collider.gameObject.GetComponent<PlaceScript>();
						if(!isDestroying)
						{
							MoveTile (temp);
							MoveAttacker(temp);
						}

						if(temp.GetTile () && temp.GetTile().team == 0 && !temp.GetAttacker())
						{
							destroyTimer+=Time.deltaTime;
							//DestroyTile(temp);
						}
					}

					/*
					if(hit.collider.gameObject.name == "Delete")
					{
						if(!selectedPlace)
						{
							if(!isDestroying)
							{
								bm.ShowDestroyableTiles();
								isDestroying = true;
							}
							else
							{
								bm.clearBoard();
								isDestroying = false;
							}
						}
					}
					*/
				}
				
			}
		}

		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast (mouseRay,out hit))
		{
			if(hit.collider.gameObject.GetComponent<PlaceScript>())
			{
				PlaceScript temp = hit.collider.gameObject.GetComponent<PlaceScript>();
			
				if(Input.GetMouseButton(0))
				{
					if(temp.canBeDestroyed && selectedPlace == null)
					{
						destroyTimer+=Time.deltaTime;
						if (destroyTimer > 0){
							if(temp.GetTile().isWalkable)
								temp.GetTile().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = bm.walkBreak1;
							else
								temp.GetTile().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = bm.blockBreak1;
						}
						if(destroyTimer > 1)
						{
							if(temp.GetTile().isWalkable)
								temp.GetTile().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = bm.walkBreak2;
							else
								temp.GetTile().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = bm.blockBreak2;
						}
						if(destroyTimer > 2)
						{
							DestroyTile(temp);
							destroyTimer = 0;
						}

					}
				}
				else
				{
					destroyTimer = 0;
				}

				if(Input.GetMouseButtonDown(0))
				{

					if(!isDestroying)
					{
						MoveTile (temp);
						MoveAttacker(temp);
					}


				}
				
				/*
				if(hit.collider.gameObject.name == "Delete")
				{
					if(!selectedPlace)
					{
						if(!isDestroying)
						{
							bm.ShowDestroyableTiles();
							isDestroying = true;
						}
						else
						{
							bm.clearBoard();
							isDestroying = false;
						}
					}
				}*/
				
			}
		}
	}


	// Selects/ Deselects Hand Tile. Moves Hand Tile to available place on gameboard.
	void MoveTile(PlaceScript place)
	{
		if(bm.player == place.playerHand)
		{
			//	Selecting a hand tile for movement. Changes the sprite to its selected version.
			//	Shows all places that the player can move the tile to. 
			if(!selectedPlace)
			{
				selectedPlace = place;
				if(selectedPlace.GetTile().isWalkable)
					selectedPlace.GetTile().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = bm.s_Walk;
				else
					selectedPlace.GetTile().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = bm.s_Block;
				bm.ShowPlaceableTiles();
				Debug.Log ("Player " +bm.player + " has selected a hand tile.");
			}
			else
			{
			 	//	If the player selects the same tile that’s already selected. It deselects the tile and 
			 	//	clears any changes made to the gameboard.
				if(selectedPlace == place)
				{
					if(selectedPlace.GetTile().isWalkable)
						selectedPlace.GetTile().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = bm.walk;
					else
						selectedPlace.GetTile().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = bm.block;
					selectedPlace = null;
					bm.clearBoard();
					Debug.Log ("Player " +bm.player + " has deselected a hand tile.");
				}
			}
		}
		else
		{
			if(selectedPlace)
			{
				if(place.canBePlaced)
				{
					place.SetTile(selectedPlace.GetTile());
					place.GetTile().transform.position = new Vector3 (place.transform.position.x,bm.pieceOffset,place.transform.position.z);
					if(place.GetTile().isWalkable)
						place.GetTile().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = bm.walk;
					else
						place.GetTile().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = bm.block;
					selectedPlace.SetTile(null);
					selectedPlace = null;
					bm.moves--;
					currState = PlayerStates.updateBoard;
					//bm.clearBoard();
					Debug.Log ("Player " +bm.player + " has placed a tile.");
				}
			}
		}
	}

	void MoveAttacker(PlaceScript place)
	{
		if(place.GetAttacker() && bm.player == place.GetAttacker().team)
		{
			if(!selectedPlace)
			{
				selectedPlace = place;
				selectedPlace.GetAttacker().gameObject.GetComponent<Renderer>().material.color = Color.black;
				if(bm.ShowPossibleAttackerMove(place)){}
				Debug.Log ("Player " +bm.player + " has selected an attacker.");
			}
			else
			{
				if(selectedPlace == place)
				{
					selectedPlace.GetAttacker().gameObject.GetComponent<Renderer>().material.color = Color.white;
					selectedPlace = null;
					bm.clearBoard();
					Debug.Log ("Player " +bm.player + " has deselected an attacker.");
				}
			}
		}
		else
		{
			if(selectedPlace)
			{
				if(place.canMove)
				{
					place.SetAttacker(selectedPlace.GetAttacker());
					place.GetAttacker().transform.position = new Vector3 (place.transform.position.x,0.3f,place.transform.position.z);
					place.GetAttacker().gameObject.GetComponent<Renderer>().material.color = Color.white;
					selectedPlace.SetAttacker(null);
					selectedPlace = null;
					bm.moves--;
					currState = PlayerStates.updateBoard;
					//bm.clearBoard();
					Debug.Log ("Player " +bm.player + " has moved an attacker.");
				}
			}
		}
	}

	void DestroyTile(PlaceScript place)
	{

		if(place.canBeDestroyed)
		{
			if(place.GetTile().isWalkable)
				bm.moves--;
			else
				bm.moves-=2;

			place.GetTile().transform.parent = bm.transform.GetChild(1).transform;
			bm.tileDeck.Enqueue(place.GetTile());
			place.GetTile ().gameObject.SetActive(false);
			place.SetTile (null);
			isDestroying = false;
			bm.clearBoard();
		}
		
	}
}
