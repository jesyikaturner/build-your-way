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
                    temp.isBreaking = false;
                }

                // If the mouse button is held down then it increases the timer
                // and progresses through the animation.
                if (Input.GetMouseButton(0))
                {
                    if (temp.isBreakable)
                        timer += Time.deltaTime;

                    if (temp.BreakAnimation(timer) != 0)
                    {
                        DestroyTile(temp);
                        timer = 0f;
                        temp.isBreaking = false;
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
                selected.ToggleSelectable();
				Debug.Log ("Player has selected a hand tile.");
                boardManager.PossibleTilePlacements();
            }
			else
			{
				// deselect tile.
				if(selected == place)
				{
                    selected.ToggleSelectable();
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
				if(place.GetState("EMPTY"))
				{
                    place.SetState(selected.GetState());
					selected.SetState("EMPTY");
					Debug.Log(string.Format("Tile x:{0}, z:{1} --> x:{2}, z:{3}", selected.xPos, selected.zPos, place.xPos, place.zPos));
                    boardManager.ClearBoard();
                    boardManager.SubtractMove(1);
                    selected.ToggleSelectable();
                    selected = null;
				}
			}
		}
	}

    /*
     * MOVE ATTACKER
     */ 
	void MoveAttacker(PlaceScript place)
	{
        if (place.GetAttacker() && boardManager.curr_player == place.GetAttacker().team)
        {
            if (!selected)
            {
                selected = place;
                selected.GetAttacker().ToggleSelect();
                boardManager.PossibleAttackerMoves(selected);
            }
            else
            {
                if(selected == place)
                {
                    boardManager.ClearBoard();
                    selected.GetAttacker().ToggleSelect();
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
                if (place.isSelected)
                {
                    place.SetAttacker(selected.GetAttacker());
                    selected.GetAttacker().transform.position = new Vector3(place.transform.position.x, boardManager.pieceOffset, place.transform.position.z);
                    selected.GetAttacker().ToggleSelect();
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
        if(place.GetState("WALK"))
        {
            boardManager.SubtractMove(1);
            place.SetState("EMPTY");
        }
        if(place.GetState("BLOCK"))
        {
            boardManager.SubtractMove(2);
            place.SetState("EMPTY");
        }
        boardManager.ClearBoard();
    }

}
