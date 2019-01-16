using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour, IPlayer {

    // Public Variables for the Inspector
    [SerializeField]
    private Tile selected;

    // Private Variables
    private BoardManager boardManager;
    private SoundManager soundManager;
    private int playerID;
    private float timer = 0;

    // Use this for initialization

    public void SetupPlayerControls(SoundManager soundManager, BoardManager boardManager, int playerID)
    {
        this.boardManager = boardManager;
        this.soundManager = soundManager;
        this.playerID = playerID;
    }
	
	// Update is called once per frame
	void Update () 
	{
        if(!boardManager.IsPaused && playerID == boardManager.GetCurrPlayer())
        {
            MouseControls();
            //KeyboardControls();
        }
	}

    private void MouseControls()
	{
		RaycastHit hit;
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Tile temp = null;

		if(Physics.Raycast (mouseRay,out hit))
		{
			if(hit.collider.gameObject.GetComponent<Tile>())
				temp = hit.collider.gameObject.GetComponent<Tile>();
		}

        boardManager.ShowBreakableTiles();
        if (Input.GetMouseButtonUp(0))
        {
            if (!temp)
                return;
            MoveTile(temp);
            MoveAttacker(temp);
            timer = 0;
            temp.isBreaking = false;
        }

        // If the mouse button is held down then it increases the timer
        // and progresses through the animation.
        if (Input.GetMouseButton(0))
        {
            if (!temp)
                return;
            if (temp.GetAttacker() == null)
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

    private void KeyboardControls()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(!boardManager.IsPaused)
                boardManager.IsPaused = true;
            else
                boardManager.IsPaused = false;
        }
    }

    public bool MoveTile(Tile place)
	{
        if (place == null)
            return false;

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
                soundManager.PlaySound("SELECT");
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
                    soundManager.PlaySound("SELECT");
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
                if(boardManager.SubtractMove(1))
                {
                    // PLAY SUCCESS SOUND
                    soundManager.PlaySound("SELECT");
                }
                else
                {
                    // PLAY ERROR SOUND
                    soundManager.PlaySound("ERROR");
                }
                selected.ToggleSelectable();
                Debug.Log(string.Format("Tile x:{0}, z:{1} --> x:{2}, z:{3}", selected.xPos, selected.zPos, place.xPos, place.zPos));
                selected = null;
            }

        }
        return true;
	}

    /*
     * MOVE ATTACKER
     */
    public bool MoveAttacker(Tile place)
	{
        if (place == null)
            return false;
        /*
         * First checks to see if the tile has an attacker on it and if the attacker belongs to the
         * current player playing. Then if the player doesn't have a tile selected, it selects that
         * tile. If the place clicks the same place again, it deselects the tile.
         */ 
        if (place.GetAttacker() && playerID == place.GetAttacker().Team)
        {
            if (!selected)
            {
                selected = place;
                selected.GetAttacker().ToggleSelect();
                boardManager.PossibleAttackerMoves(selected);
                // PLAY SOUND
                soundManager.PlaySound("SELECT");
            }
            else
            {
                if(selected == place)
                {
                    boardManager.ClearBoard();
                    selected.GetAttacker().ToggleSelect();
                    selected = null;
                    // PLAY SOUND
                    soundManager.PlaySound("SELECT");
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
                    if (boardManager.SubtractMove(1))
                    {
                        // PLAY SUCCESS SOUND
                        soundManager.PlaySound("SELECT");
                    }
                    else
                    {
                        // PLAY ERROR SOUND
                        soundManager.PlaySound("ERROR");
                    }
                    selected = null;
                }
            }
        }
        return true;
	}


    public bool DestroyTile(Tile place)
    {
        if(place.GetState("WALK"))
        {
            if (boardManager.SubtractMove(1))
            {
                place.SetState("EMPTY");
                // PLAY SUCCESS SOUND
                soundManager.PlaySound("SELECT");
            }
            else
            {
                // PLAY ERROR SOUND
                soundManager.PlaySound("ERROR");
            }

        }
        if(place.GetState("BLOCK"))
        {
            if (boardManager.SubtractMove(2))
            {
                place.SetState("EMPTY");
                // PLAY SUCCESS SOUND
                soundManager.PlaySound("SELECT");
            }
            else
            {
                // PLAY ERROR SOUND
                soundManager.PlaySound("ERROR");
            }
        }
        return true;
    }

}
