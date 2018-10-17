using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {

    // Public Variables for the Inspector
    public PlaceScript selected;

    // Private Variables
    private BoardManager boardManager;
    private int playerID;
    private float timer = 0;

    // Use this for initialization

    public void SetupPlayerControls(BoardManager boardManager, int playerID)
    {
        this.boardManager = boardManager;
        this.playerID = playerID;
    }
	
	// Update is called once per frame
	void Update () 
	{
        if(playerID == boardManager.GetCurrPlayer())
        {
            MouseControls();
        }

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
		if(playerID == place.playerHand)
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
        /*
         * First checks to see if the tile has an attacker on it and if the attacker belongs to the
         * current player playing. Then if the player doesn't have a tile selected, it selects that
         * tile. If the place clicks the same place again, it deselects the tile.
         */ 
        if (place.GetAttacker() && playerID == place.GetAttacker().team)
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
            /*
             * With a tile selected and that selected tile has an attacker, the player then
             * has selected a place for the attacker to move to. First it makes sure that the tile
             * is a valid move, then it moves/ sets the attacker to the new tile.
             */ 
            if (selected && selected.GetAttacker())
            {
                if (place.isSelected)
                {
                    boardManager.SetAttacker(selected, place);
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
