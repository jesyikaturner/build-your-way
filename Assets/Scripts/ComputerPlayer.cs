using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Very basic random AI Computer Player.
 */ 
public class ComputerPlayer : MonoBehaviour {

    // Public Variables for the Inspector
    [SerializeField]
    private PlaceScript selected;

    // Constants
    private const float moveDelay = 1f;

    // Private Variables
    private BoardManager boardManager;
    private PlayerHandHandler compHand;
    private List<PlaceScript> possibleMoves;
    private List<PlaceScript> goalTiles;
    private int playerID;
    private float timer = 0;

    public void SetupComputerControls(BoardManager boardManager, int playerID)
    {
        this.boardManager = boardManager;
        this.playerID = playerID;

        possibleMoves = new List<PlaceScript>();
        goalTiles = new List<PlaceScript>();
        compHand = boardManager.GetHands()[1];

        // Getting the tiles that the attackers need to move to.
        foreach(PlaceScript cell in boardManager.GetBoardArray())
        {
            if (cell.GetState("BASE1"))
            {
                goalTiles.Add(cell);
            }
        }
    }

    void Update ()
    {
        if(boardManager.GetCurrPlayer() == playerID)
        {
            timer += Time.deltaTime;
            if (timer > moveDelay)
            {
                if (!DestroyTile())
                    if (!PlaceTile())
                        MoveAttacker();
                timer = 0;
            }

        }  
    }

    /* Goes through the steps of random selecting, add the possible 
     * moves to a list then random selecting one of those for the tile to be moved to.
     */
    private bool PlaceTile()
    {
        possibleMoves.Clear();
        selected = compHand.GetPlayerHand()[Random.Range(0, compHand.GetPlayerHand().Length)];

        boardManager.PossibleTilePlacements();

        foreach(PlaceScript cell in boardManager.GetBoardArray())
        {
            if(cell.isSelected)
            {
                possibleMoves.Add(cell);
            }
        }

        if (possibleMoves.Count == 0)
            return false;

        PlaceScript selectedMove = GetClosestPosition(possibleMoves);
        if (selectedMove)
            selectedMove.SetState(selected.GetState());
        else
            possibleMoves[Random.Range(0, possibleMoves.Count)].SetState(selected.GetState());

        boardManager.SubtractMove(1);


        selected.SetState("EMPTY");
        selected = null;
        boardManager.ClearBoard();
        return true;
    }

    private PlaceScript GetClosestPosition(List<PlaceScript> possiblePlaces)
    {
        PlaceScript selectedPlace = null ;
        float distance = float.MaxValue;
        foreach (PlaceScript cell in goalTiles)
        {
            foreach(PlaceScript place in possiblePlaces)
            {
                if(Vector3.Distance(cell.transform.position,place.transform.position) < distance)
                {
                    selectedPlace = place;
                    distance = Vector3.Distance(cell.transform.position, place.transform.position);
                }
            }
        }
        return selectedPlace;
    }

    /*
     * Same as the move tile, except checking which tiles have the p2 attacker,
     * selecting the tiles
     */ 
    private bool MoveAttacker()
    {
        possibleMoves.Clear();
        foreach (PlaceScript cell in boardManager.GetBoardArray())
        {
            if(cell.GetAttacker() && cell.GetAttacker().team == playerID)
            {
                // make sure its not already on the goal tiles.
                if(!cell.GetState("BASE1"))
                    possibleMoves.Add(cell);
            }
        }
        if(possibleMoves.Count == 0)
            return false;

        PlaceScript selectedMove = GetClosestPosition(possibleMoves);
        if (selectedMove)
            selected = selectedMove;
        else
            selected = possibleMoves[Random.Range(0, possibleMoves.Count)];

        possibleMoves.Clear();
        boardManager.PossibleAttackerMoves(selected);

        foreach (PlaceScript cell in boardManager.GetBoardArray())
        {
            if(cell.isSelected)
            {
                possibleMoves.Add(cell);
            }
        }

        if (possibleMoves.Count == 0)
            return false;

        selectedMove = GetClosestPosition(possibleMoves);
        PlaceScript place;
        if (selectedMove)
            place = selectedMove;
        else
            place = possibleMoves[Random.Range(0, possibleMoves.Count)];

        selected.GetAttacker().ToggleSelect();
        boardManager.SetAttacker(selected, place);
        boardManager.ClearBoard();
        boardManager.SubtractMove(1);
        selected = null;
        return true;
    }

    public bool DestroyTile()
    {
        possibleMoves.Clear();
        boardManager.ShowBreakableTiles();
        foreach(PlaceScript cell in boardManager.GetBoardArray())
        {
            if (cell.isBreakable)
            {
                possibleMoves.Add(cell);
            }
        }

        if (possibleMoves.Count == 0)
            return false;

        PlaceScript selectedMove = GetClosestPosition(possibleMoves);
        if (selectedMove)
            selected = selectedMove;
        else
            selected = possibleMoves[Random.Range(0, possibleMoves.Count)];

        PlaceScript place = selected;

        if (place.GetState("WALK"))
            return false;

        if (place.GetState("BLOCK"))
        {
            boardManager.SubtractMove(2);
            place.SetState("EMPTY");
        }
        boardManager.ClearBoard();

        return true;
    }
}
