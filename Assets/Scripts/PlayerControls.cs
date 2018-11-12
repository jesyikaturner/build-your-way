using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour, IPlayer {

    // Public Variables for the Inspector
    [SerializeField]
    private PlaceScript selected;

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

    private void MouseControls()
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
                    if(temp.GetAttacker() == null)
                    {
                        if (temp.isBreakable)
                            timer += Time.deltaTime;

                        if (temp.BreakAnimation(timer) != 0)
                        {
                            DestroyTile(temp);
                            timer = 0f;
                            temp.isBreaking = false;
                            boardManager.ClearBoard();
                        }
                    }

                }
			}
		}

	}

    public bool MoveTile(PlaceScript place)
	{
        if (selected && selected.GetAttacker())
            return false;

		if(playerID == place.playerHand)
		{
			// if the player doesn't have anything selected from their hand
            // make what they clicked on, the selected tile.
			if(!selected)
			{
				selected = place;
                selected.ToggleSelectable();
                boardManager.PossibleTilePlacements();
                Debug.Log("Player has selected a hand tile.");
                // PLAY SOUND
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
                    // PLAY SOUND
                }
            }
		}
		else
		{
            // Move the tile from the player's hand.
            if (place.GetState("EMPTY") && place.isSelected)
            {
                place.SetState(selected.GetState());
                selected.SetState("EMPTY");
                boardManager.ClearBoard();
                boardManager.SubtractMove(1);
                selected.ToggleSelectable();
                Debug.Log(string.Format("Tile x:{0}, z:{1} --> x:{2}, z:{3}", selected.xPos, selected.zPos, place.xPos, place.zPos));
                selected = null;

                // PLAY SOUND
            }

        }
        return true;
	}

    /*
     * MOVE ATTACKER
     */
    public bool MoveAttacker(PlaceScript place)
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
                // PLAY SOUND
            }
            else
            {
                if(selected == place)
                {
                    boardManager.ClearBoard();
                    selected.GetAttacker().ToggleSelect();
                    selected = null;
                    // PLAY SOUND
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
                    // PLAY SOUND
                }
            }
        }
        return true;
	}


    public bool DestroyTile(PlaceScript place)
    {
        if(place.GetState("WALK"))
        {
            if(boardManager.SubtractMove(1))
                place.SetState("EMPTY");
        }
        if(place.GetState("BLOCK"))
        {
            if(boardManager.SubtractMove(2))
                place.SetState("EMPTY");
        }
        return true;
    }

}
