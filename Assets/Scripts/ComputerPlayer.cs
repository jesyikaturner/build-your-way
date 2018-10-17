using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Very basic random AI Computer Player.
 */ 
public class ComputerPlayer : MonoBehaviour {

    // Public Variables for the Inspector
    public PlaceScript selected;

    // Constants
    private const float moveDelay = 1f;

    // Private Variables
    private BoardManager boardManager;
    private List<PlaceScript> possibleMoves;
    private int playerID;
    private float timer = 0;

    public void SetupComputerControls(BoardManager boardManager, int playerID)
    {
        this.boardManager = boardManager;
        this.playerID = playerID;

        possibleMoves = new List<PlaceScript>();
    }

    void Update ()
    {
        if(boardManager.GetCurrPlayer() == playerID)
        {
            timer += Time.deltaTime;
            if (timer > moveDelay)
            {
                if (!PlaceTile())
                    if (!MoveAttacker())
                        DestroyTile();
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
        selected = boardManager.GetPlayerHands()[Random.Range(0,boardManager.GetPlayerHands().Length)];

        while(selected.playerHand != playerID)
        {
            selected = boardManager.GetPlayerHands()[Random.Range(0, boardManager.GetPlayerHands().Length)];
        }

        boardManager.PossibleTilePlacements();

        foreach(PlaceScript cell in boardManager.GetBoardArray())
        {
            if(cell.isSelected)
            {
                possibleMoves.Add(cell);
            }
        }

        if (possibleMoves.Count > 0)
        {
            possibleMoves[Random.Range(0, possibleMoves.Count)].SetState(selected.GetState());
            boardManager.SubtractMove(1);
        }
        else
            return false;
        selected.SetState("EMPTY");
        selected = null;
        boardManager.ClearBoard();
        return true;
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
                possibleMoves.Add(cell);
            }
        }
        if(possibleMoves.Count == 0)
            return false;

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

        PlaceScript place = possibleMoves[Random.Range(0, possibleMoves.Count)];
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

        PlaceScript place = possibleMoves[Random.Range(0, possibleMoves.Count)];

        if (place.GetState("WALK"))
        {
            boardManager.SubtractMove(1);
            place.SetState("EMPTY");
        }
        if (place.GetState("BLOCK"))
        {
            boardManager.SubtractMove(2);
            place.SetState("EMPTY");
        }
        boardManager.ClearBoard();

        return true;
    }
}
