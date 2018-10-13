using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {

	private BoardManager boardManager;
	public PlaceScript selected;
    public float timer = 0;

    // Use this for initialization
    void Start () 
	{
		boardManager = transform.GetComponent<BoardManager>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		MouseControls();
	}

	void MouseControls()
	{
		RaycastHit hit;
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast (mouseRay,out hit))
		{
			if(hit.collider.gameObject.GetComponent<PlaceScript>())
			{
				PlaceScript temp = hit.collider.gameObject.GetComponent<PlaceScript>();
                boardManager.ShowBreakableTiles();
                if (Input.GetMouseButtonUp(0))
				{
					MoveTile(temp);
                    MoveAttacker(temp);
                    timer = 0;
                }

                // I mean this is not the best way to do this...
                // If the mouse button is held down then it increases the timer
                // and progresses through the animation.
                if (Input.GetMouseButton(0))
                {
                    if (temp.isBreakable)
                    {
                        timer += Time.deltaTime;
                        if (timer > 0f)
                        {
                            if (temp.blockType == 1)
                                temp.blockType = 5;
                            if (temp.blockType == 2)
                                temp.blockType = 7;
                        }
                    }
                    if (timer > 1f)
                    {
                        if (temp.blockType == 5)
                            temp.blockType = 6;
                        if (temp.blockType == 7)
                            temp.blockType = 8;
                    }
                    if (timer > 2f)
                    {
                        DestroyTile(temp);
                        timer = 0f;
                    }
                }
			}
		}
	}

	void MoveTile(PlaceScript place)
	{
		if(boardManager.curr_player == place.playerHand)
		{
			// if the player doesn't have anything selected from their hand
            // make what they clicked on, the selected tile.
			if(!selected)
			{
				selected = place;
				selected.blockType *= -1;
				Debug.Log ("Player has selected a hand tile.");
                boardManager.PossibleTilePlacements();
            }
			else
			{
				// deselect tile.
				if(selected == place)
				{
					selected.blockType *= -1;
                    boardManager.ClearBoard();
                    selected = null;
					Debug.Log ("Player has deselected a hand tile.");
				}
			}
		}
		else
		{
			// Move the tile from the player's hand.
			if(selected && selected.GetAttacker() == null)
			{
				if(place.blockType == -5)
				{
					place.blockType = -selected.blockType;
					selected.blockType = 0;
					Debug.Log(string.Format("Tile x:{0}, z:{1} --> x:{2}, z:{3}", selected.xPos, selected.zPos, place.xPos, place.zPos));
                    boardManager.ClearBoard();
                    boardManager.SubtractMove(1);
                    selected = null;
				}
			}
		}
	}

	void MoveAttacker(PlaceScript place)
	{
        if (place.GetAttacker() && boardManager.curr_player == place.GetAttacker().team)
        {
            if (!selected)
            {
                selected = place;
                // TODO: change colour of attacker
                boardManager.PossibleAttackerMoves(place);
            }
            else
            {
                if(selected == place)
                {
                    boardManager.ClearBoard();
                    // TODO: set colour of attacker to default
                    selected = null;
                }
            }
        }
        else
        {
            // Move the attacker.
            if (selected && selected.GetAttacker() != null)
            {
                // if place is a walkable tile or a base tile, then set the attacker to place
                if (place.blockType == -1 || place.blockType == -3 || place.blockType == -4)
                {
                    place.SetAttacker(selected.GetAttacker());
                    selected.GetAttacker().transform.position = new Vector3(place.transform.position.x, boardManager.pieceOffset, place.transform.position.z);
                    // TODO: set colour of attacker to default
                    selected.SetAttacker(null);
                    boardManager.ClearBoard();
                    boardManager.SubtractMove(1);
                    selected = null;
                }
            }
        }
	}


    void DestroyTile(PlaceScript place)
    {
        if(place.blockType == 6)
        {
            boardManager.SubtractMove(1);
            place.blockType = 0;
        }
        if(place.blockType == 8)
        {
            boardManager.SubtractMove(2);
            place.blockType = 0;
        }
        boardManager.ClearBoard();
    }

}
